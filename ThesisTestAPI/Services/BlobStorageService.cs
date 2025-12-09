using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using SixLabors.ImageSharp.Formats.Jpeg;
using ThesisTestAPI.Models.MessageAttachments;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Rectangle = SixLabors.ImageSharp.Rectangle;
using Size = SixLabors.ImageSharp.Size;

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
            int jpegQuality = 85)
        {
            try
            {
                if (imageStream.CanSeek)
                    imageStream.Position = 0;

                // Load image using ImageSharp
                using var image = await Image.LoadAsync(imageStream);

                // Determine target size
                var (targetW, targetH) = ComputeFitSize(
                    image.Width,
                    image.Height,
                    maxWidth,
                    maxHeight
                );

                // Resize if needed
                if (targetW != image.Width || targetH != image.Height)
                {
                    image.Mutate(x =>
                        x.Resize(new ResizeOptions
                        {
                            Size = new Size(targetW, targetH),
                            Mode = ResizeMode.Max,
                            Sampler = KnownResamplers.Bicubic
                        })
                    );
                }

                var outputStream = new MemoryStream();

                // Pick encoder based on contentType
                IImageEncoder encoder = contentType.ToLower() switch
                {
                    "image/png" => new PngEncoder(),
                    "image/webp" => new WebpEncoder { Quality = jpegQuality },
                    _ => new JpegEncoder { Quality = jpegQuality } // default to jpeg
                };

                // If format not supported natively → fallback to JPEG
                if (encoder is WebpEncoder && !Configuration.Default.ImageFormats.Contains(WebpFormat.Instance))
                {
                    encoder = new JpegEncoder { Quality = jpegQuality };
                    contentType = "image/jpeg";
                }

                // Save as re-encoded image
                await image.SaveAsync(outputStream, encoder);
                outputStream.Position = 0;

                // Upload to Azure Blob Storage
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(fileName);

                var headers = new BlobHttpHeaders { ContentType = contentType };

                await blobClient.UploadAsync(outputStream, new BlobUploadOptions
                {
                    HttpHeaders = headers
                });

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading image: {ex.Message}", ex);
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

        public async Task<string> UploadImageAsync(
            Stream imageStream,
            string fileName,
            string contentType,
            string containerName,
            int targetSize)
        {
            try
            {
                if (imageStream.CanSeek)
                    imageStream.Position = 0;

                // Load original image
                using var image = await Image.LoadAsync(imageStream);

                // ---- 1. COMPRESS (JPEG QUALITY ≈ 40) ----
                // Re-encode to JPEG with lower quality into a temp stream
                var tempCompressed = new MemoryStream();
                var compressEncoder = new JpegEncoder { Quality = 40 };
                await image.SaveAsJpegAsync(tempCompressed, compressEncoder);
                tempCompressed.Position = 0;

                // Reload as ImageSharp image after compression
                using var compressedImage = await Image.LoadAsync(tempCompressed);

                int width = compressedImage.Width;
                int height = compressedImage.Height;

                // ---- 2. CROP TO CENTER SQUARE ----
                int size = Math.Min(width, height);
                int x = (width - size) / 2;
                int y = (height - size) / 2;

                compressedImage.Mutate(m => m.Crop(new Rectangle(x, y, size, size)));

                // ---- 3. RESIZE TO TARGET SIZE ----
                compressedImage.Mutate(m =>
                    m.Resize(new ResizeOptions
                    {
                        Size = new Size(targetSize, targetSize),
                        Mode = ResizeMode.Stretch, // it's already square
                        Sampler = KnownResamplers.Bicubic
                    })
                );

                // ---- 4. ENCODE AS JPEG ----
                var outputStream = new MemoryStream();
                await compressedImage.SaveAsJpegAsync(outputStream, new JpegEncoder
                {
                    Quality = 85 // or change as needed
                });

                outputStream.Position = 0;

                // ---- 5. UPLOAD TO AZURE BLOB STORAGE ----
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(fileName);

                var headers = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blobClient.UploadAsync(outputStream, new BlobUploadOptions
                {
                    HttpHeaders = headers
                });

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading image: {ex.Message}", ex);
            }
        }

        private async Task<Stream> CompressImage(Stream imageStream, int quality)
        {
            imageStream.Position = 0;

            using var image = await SixLabors.ImageSharp.Image.LoadAsync(imageStream);

            var outputStream = new MemoryStream();

            var encoder = new JpegEncoder
            {
                Quality = quality // same as Encoder.Quality
            };

            await image.SaveAsJpegAsync(outputStream, encoder);

            outputStream.Position = 0;
            return outputStream;
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
