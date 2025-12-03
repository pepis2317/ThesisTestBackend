using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using ThesisTestAPI.Models.MessageAttachments;

namespace ThesisTestAPI.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobStorageService(IConfiguration configuration)
        {
            string connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
        public async Task<AttachmentDTO> GenerateSasUriAsync(
            Guid attachmentId,
            string blobName,
            string fileName,
            string mimeType,
            string containerName,
            TimeSpan? lifetime = null)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobName);

            lifetime ??= TimeSpan.FromMinutes(10);

            // Get file properties (includes size)
            try
            {
                var properties1 = await blob.GetPropertiesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            var properties = await blob.GetPropertiesAsync();
            var sizeBytes = properties.Value.ContentLength;

            // Build SAS
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.Add(lifetime.Value)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            sasBuilder.ContentType = mimeType;
            sasBuilder.ContentDisposition =
                $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}";

            if (!_blobServiceClient.CanGenerateAccountSasUri)
                throw new InvalidOperationException(
                    "BlobServiceClient must be created with a Storage Account key to generate SAS URLs."
                );

            var sasUri = blob.GenerateSasUri(sasBuilder);

            return new AttachmentDTO
            {
                AttachmentId = attachmentId,
                DownloadUrl = sasUri.ToString(),
                SizeBytes = sizeBytes,
                ExpiresAt = sasBuilder.ExpiresOn,
                MimeType = mimeType,
                FileName = fileName
            };
        }
        public async Task<string?> GetTemporaryImageUrl(string? fileName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!(await blobClient.ExistsAsync())) return null;

            var now = DateTimeOffset.UtcNow;

            var sas = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                // Buffer for clock skew:
                StartsOn = now.AddMinutes(-5),
                // A bit longer so it won't expire mid-view:
                ExpiresOn = now.AddMinutes(15)
            };

            sas.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sas).ToString();
        }
        public async Task<bool> DeleteFileAsync(string fileName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                return true;
            }
            return false;
        }
        public async Task<string> UploadImageFreeAspectAsync(
                Stream imageStream,
                string fileName,
                string contentType,
                string containerName,
                int? maxWidth = null,
                int? maxHeight = null,
                long jpegQuality = 85L)
        {
            try
            {
                // ensure we start at 0
                if (imageStream.CanSeek) imageStream.Position = 0;

                using var sourceImage = Image.FromStream(imageStream);

                // Determine target size (preserve aspect)
                var (targetW, targetH) = ComputeFitSize(
                    sourceImage.Width,
                    sourceImage.Height,
                    maxWidth,
                    maxHeight
                );

                using var outputStream = new MemoryStream();

                // If no resizing needed and we can just pass through, re-encode as requested format anyway
                // (If you truly want byte-for-byte passthrough, copy the incoming stream instead.)
                using (var canvas = new Bitmap(targetW, targetH))
                using (var g = Graphics.FromImage(canvas))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage(sourceImage, 0, 0, targetW, targetH);

                    // Choose encoder based on contentType (default to JPEG)
                    if (string.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase))
                    {
                        canvas.Save(outputStream, ImageFormat.Png);
                    }
                    else if (string.Equals(contentType, "image/webp", StringComparison.OrdinalIgnoreCase))
                    {
                        // System.Drawing doesn't natively support WebP. Fall back to JPEG.
                        var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        var encParams = new EncoderParameters(1);
                        encParams.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
                        canvas.Save(outputStream, jpgEncoder, encParams);
                        contentType = "image/jpeg";
                        // Optionally also adjust fileName extension here.
                    }
                    else
                    {
                        // JPEG
                        var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        var encParams = new EncoderParameters(1);
                        encParams.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
                        canvas.Save(outputStream, jpgEncoder, encParams);
                        if (!contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
                            contentType = "image/jpeg";
                    }
                }

                outputStream.Position = 0;

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(fileName);

                var headers = new BlobHttpHeaders { ContentType = contentType };

                // If you want overwrite semantics, use the UploadAsync(stream, overwrite: true) overload,
                // or pass BlobUploadOptions. Here we also set headers.
                await blobClient.UploadAsync(outputStream, new BlobUploadOptions { HttpHeaders = headers });

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading image: {ex}");
            }
        }

        // --- helpers ---

        private static (int width, int height) ComputeFitSize(int srcW, int srcH, int? maxW, int? maxH)
        {
            // No limits at all -> original size
            if (maxW is null && maxH is null)
                return (srcW, srcH);

            // Treat missing side as infinitely large
            double limitW = maxW ?? double.PositiveInfinity;
            double limitH = maxH ?? double.PositiveInfinity;

            // If already within bounds, keep original
            if (srcW <= limitW && srcH <= limitH)
                return (srcW, srcH);

            // Scale down by the tighter constraint
            double scale = Math.Min(limitW / srcW, limitH / srcH);
            int w = Math.Max(1, (int)Math.Round(srcW * scale));
            int h = Math.Max(1, (int)Math.Round(srcH * scale));
            return (w, h);
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, string containerName, int targetSize)
        {
            try
            {
                using var compressedStream = CompressImage(imageStream, 40);
                using var image = Image.FromStream(compressedStream);

                int width = image.Width;
                int height = image.Height;

                int size = Math.Min(width, height);
                int x = (width - size) / 2;
                int y = (height - size) / 2;

                using var croppedImage = new Bitmap(size, size);
                using (var graphics = Graphics.FromImage(croppedImage))
                {
                    graphics.DrawImage(image, new Rectangle(0, 0, size, size), new Rectangle(x, y, size, size), GraphicsUnit.Pixel);
                }

                // Resize to target size
                using var resizedImage = new Bitmap(targetSize, targetSize);
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(croppedImage, 0, 0, targetSize, targetSize);
                }

                using var memoryStream = new MemoryStream();
                resizedImage.Save(memoryStream, ImageFormat.Jpeg);
                memoryStream.Position = 0;

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(fileName);
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };
                await blobClient.UploadAsync(memoryStream, blobHttpHeaders);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading image: {ex}");
            }
        }
        private Stream CompressImage(Stream imageStream, int quality)
        {
            var outputStream = new MemoryStream();

            // Load the image from the input stream
            using (var bmp = new Bitmap(imageStream))
            {
                // Get the JPEG encoder
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                // Set the quality parameter
                var encoder = Encoder.Quality;
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(encoder, quality);

                // Save the compressed image to the output stream
                bmp.Save(outputStream, jpgEncoder, encoderParameters);
            }

            // Reset the stream position to the beginning
            outputStream.Position = 0;

            return outputStream;
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public async Task<string> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string? contentType,
            string containerName,
            string? folder = null,
            bool overwrite = false)
        {
            var safeName = SanitizeFileName(fileName);
            var blobName = string.IsNullOrWhiteSpace(folder)
                ? $"{Guid.NewGuid()}_{safeName}"
                : $"{folder.TrimEnd('/')}/{Guid.NewGuid()}_{safeName}";
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(blobName);
            var resolvedContentType = ResolveContentType(safeName, contentType);
            byte[]? md5 = null;
            if (fileStream.CanSeek)
            {
                // Compute hash and then reset stream position
                using var md5Algo = MD5.Create();
                var originalPos = fileStream.Position;
                md5 = md5Algo.ComputeHash(fileStream);
                fileStream.Position = originalPos;
            }
            var headers = new BlobHttpHeaders
            {
                ContentType = resolvedContentType,
                // Keep original filename on download
                ContentDisposition = $"attachment; filename=\"{safeName}\"; filename*=UTF-8''{Uri.EscapeDataString(safeName)}",
                ContentHash = md5 
            };
            var options = new BlobUploadOptions
            {
                HttpHeaders = headers
            };
            await blob.UploadAsync(fileStream, options, cancellationToken: default);
            return blobName;
        }
        private static string ResolveContentType(string fileName, string? provided)
        {
            if (!string.IsNullOrWhiteSpace(provided)) return provided;

            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(fileName, out var detected))
                return detected;

            // Fallback if unknown
            return "application/octet-stream";
        }

        // Basic filename sanitizer; tweak as needed for your rules
        private static string SanitizeFileName(string fileName)
        {
            // strip path parts if client sent a full path
            var nameOnly = Path.GetFileName(fileName);

            // remove disallowed characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(nameOnly.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());

            // optional: trim very long names
            return cleaned.Length > 140 ? cleaned[^140..] : cleaned;
        }
    }
}
