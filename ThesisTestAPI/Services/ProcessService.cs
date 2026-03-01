using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Services
{
    public class ProcessService
    {
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notifService;
        private readonly BlobStorageService _blobStorageService;
        public ProcessService(ThesisDbContext db,  NotificationService notifService, BlobStorageService blobStorageService)
        {
            _db = db;
            _notifService = notifService;
            _blobStorageService = blobStorageService;
        }
        
        public async Task<PaginatedProcessesResponse> GetSellerProcesses(GetSellerProcessesRequest request)
        {
            var processes = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.Seller).Include(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q => q.Author).Skip((request.pageNumber - 1) * request.pageSize).Where(q => q.Request.Seller.OwnerId == request.UserId).OrderByDescending(q => q.CreatedAt).ToListAsync();
            var list = new List<ProcessResponse>();
            foreach (var process in processes)
            {
                var item = new ProcessResponse
                {
                    ProcessId = process.ProcessId,
                    Description = process.Description,
                    Status = process.Status,
                    Title = process.Title
                };

                var seller = process.Request.Seller;
                var user = process.Request.RequestNavigation.Author;
                var sellerPic = "";
                var pfp = "";
                if (!string.IsNullOrEmpty(user.Pfp))
                {
                    pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                }
                if (!string.IsNullOrEmpty(seller.SellerPicture))
                {
                    sellerPic = await _blobStorageService.GetTemporaryImageUrl(seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
                }
                item.User = new Models.User.UserResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Pfp = pfp
                };
                item.Seller = new Models.Producer.SellerResponse
                {
                    SellerId = seller.SellerId,
                    SellerName = seller.SellerName,
                    SellerPicture = sellerPic,
                };
                list.Add(item);
            }
            var total = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.Seller).Where(q => q.Request.Seller.OwnerId == request.UserId).CountAsync();
            return new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            };
        }
        
        public async Task<ProcessResponse> GetProcess(GetProcessRequest request)
        {
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.Seller).Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).ThenInclude(q=>q.Author).Where(q => q.ProcessId == request.ProcessId).FirstOrDefaultAsync();
            var seller = process.Request.Seller;
            var user = process.Request.RequestNavigation.Author;
            var sellerPic = "";
            var pfp = "";
            if (!string.IsNullOrEmpty(user.Pfp))
            {
                pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
            }
            if (!string.IsNullOrEmpty(seller.SellerPicture))
            {
                sellerPic = await _blobStorageService.GetTemporaryImageUrl(seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
            }

            return new ProcessResponse
            {
                ProcessId = request.ProcessId,
                Title = process.Title,
                Description = process.Description,
                Status = process.Status,
                Seller = new Models.Producer.SellerResponse
                {
                    SellerId = seller.SellerId,
                    SellerName = seller.SellerName,
                    SellerPicture = sellerPic,
                },
                User = new Models.User.UserResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Pfp = pfp
                }
            };
        }
        
        public async Task<PaginatedProcessesResponse> GetProcesses(GetProcessesRequest request)
        {
            var query = _db.Processes
                .Where(p => p.Request.RequestNavigation.AuthorId == request.UserId)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();

            var processes = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(p => new
                {
                    p.ProcessId,
                    p.Description,
                    p.Status,
                    p.Title,
                    Seller = p.Request.Seller,
                    User = p.Request.RequestNavigation.Author
                })
                .ToListAsync();

            var list = new List<ProcessResponse>();

            foreach (var process in processes)
            {
                var sellerPic = string.IsNullOrEmpty(process.Seller.SellerPicture)
                    ? ""
                    : await _blobStorageService.GetTemporaryImageUrl(
                        process.Seller.SellerPicture,
                        Enum.BlobContainers.SELLERPICTURE);

                var pfp = string.IsNullOrEmpty(process.User.Pfp)
                    ? ""
                    : await _blobStorageService.GetTemporaryImageUrl(
                        process.User.Pfp,
                        Enum.BlobContainers.PFP);

                list.Add(new ProcessResponse
                {
                    ProcessId = process.ProcessId,
                    Description = process.Description,
                    Status = process.Status,
                    Title = process.Title,
                    User = new Models.User.UserResponse
                    {
                        UserId = process.User.UserId,
                        UserName = process.User.UserName,
                        Pfp = pfp
                    },
                    Seller = new Models.Producer.SellerResponse
                    {
                        SellerId = process.Seller.SellerId,
                        SellerName = process.Seller.SellerName,
                        SellerPicture = sellerPic
                    }
                });
            }

            return new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            };
        }
        
        public async Task<CompleteProcessResponse?> GetCompleteRequest(GetCompleteRequest request)
        {
            var completeRequest = await _db.CompleteProcessRequests.Where(q => q.ProcessId == request.ProcessId && (q.Status == RequestStatuses.PENDING || q.Status == RequestStatuses.ACCEPTED)).OrderByDescending(q=>q.CreatedAt).FirstOrDefaultAsync();
            if(completeRequest != null)
            {
                return new CompleteProcessResponse
                {
                    CompleteProcessRequestId = completeRequest.RequestId,
                    Status = completeRequest.Status,
                };
            }

            return null;
        }
        
        public async Task<PaginatedProcessesResponse> GetAllProcesses(GetAllProcessesRequest request)
        {
            var processes = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.Seller).Include(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q => q.Author).Skip((request.pageNumber - 1) * request.pageSize).OrderByDescending(q => q.CreatedAt).ToListAsync();
            var list = new List<ProcessResponse>();
            foreach (var process in processes)
            {
                var item = new ProcessResponse
                {
                    ProcessId = process.ProcessId,
                    Description = process.Description,
                    Status = process.Status,
                    Title = process.Title
                };
                var seller = process.Request.Seller;
                var user = process.Request.RequestNavigation.Author;
                var sellerPic = "";
                var pfp = "";
                if (!string.IsNullOrEmpty(user.Pfp))
                {
                    pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                }
                if (!string.IsNullOrEmpty(seller.SellerPicture))
                {
                    sellerPic = await _blobStorageService.GetTemporaryImageUrl(seller.SellerPicture, Enum.BlobContainers.SELLERPICTURE);
                }
                item.User = new Models.User.UserResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Pfp = pfp
                };
                item.Seller = new Models.Producer.SellerResponse
                {
                    SellerId = seller.SellerId,
                    SellerName = seller.SellerName,
                    SellerPicture = sellerPic,
                };
                list.Add(item);
            }
            var total = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.RequestNavigation).CountAsync();
            return new PaginatedProcessesResponse
            {
                Total = total,
                Processes = list
            };
        }
        
        public async Task<CompleteProcessResponse> CompleteRequest(RespondCompleteProcess request)
        {
            var completeRequest = await _db.CompleteProcessRequests.Where(q => q.RequestId == request.CompleteProcessRequestId).FirstOrDefaultAsync();
            var process = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.RequestNavigation).ThenInclude(q=>q.Author).Where(q => q.ProcessId == completeRequest.ProcessId).FirstOrDefaultAsync();
            if (request.Response == RequestStatuses.ACCEPTED)
            {
                var ownerId = await _db.Sellers.Where(q => q.SellerId == process.Request.SellerId).Select(q => q.OwnerId).FirstOrDefaultAsync();
                var ownerWallet = await _db.Wallets.Where(q => q.UserId == ownerId).FirstOrDefaultAsync();
                var steps = await _db.Steps.Include(q => q.Transaction).Where(q => q.ProcessId == process.ProcessId).ToListAsync();
                long totalToSend = 0;
                foreach (var step in steps)
                {
                    if (step.Transaction != null)
                    {
                        totalToSend += step.Transaction.AmountMinor;
                    }
                }
                process.Status = ProcessStatuses.COMPLETED;
                ownerWallet.BalanceMinor += totalToSend;
                completeRequest.Status = RequestStatuses.ACCEPTED;
                completeRequest.UpdatedAt = DateTimeOffset.Now;
            }
            else if (request.Response == RequestStatuses.DECLINED)
            {
                completeRequest.Status = RequestStatuses.DECLINED;
                completeRequest.UpdatedAt = DateTimeOffset.Now;
            }
            await _db.SaveChangesAsync();
            
            var receiverId = completeRequest.Process.Request.RequestNavigation.Author.UserId;
            await _notifService.SendNotification($"Completion request for {process.Title} has been {request.Response}", receiverId);
            
            return new CompleteProcessResponse
            {
                CompleteProcessRequestId = completeRequest.RequestId, 
                Status = completeRequest.Status
            };
        }
        
        public async Task<CompleteProcessResponse> CreateCompleteRequest(CreateCompleteProcessRequest request)
        {
            var completeRequest = new CompleteProcessRequest
            {
                RequestId = Guid.NewGuid(),
                ProcessId = request.ProcessId,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatuses.PENDING,
            };
            _db.CompleteProcessRequests.Add(completeRequest);
            await _db.SaveChangesAsync();
            
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).Where(q => q.ProcessId == request.ProcessId).FirstOrDefaultAsync();
            await _notifService.SendNotification($"Seller wants to complete process {process.Title}", process.Request.RequestNavigation.AuthorId);
            return new CompleteProcessResponse
            {
                CompleteProcessRequestId = request.ProcessId,
                Status = RequestStatuses.PENDING,
            };
        }
        
        public async Task<ProcessResponse> CreateProcess(CreateProcessRequest request)
        {
            var orderRequest = await _db.Requests.Include(q => q.Seller).Include(q => q.RequestNavigation).Where(q => q.RequestId == request.RequestId).FirstOrDefaultAsync();
            var sellerId = orderRequest.Seller.OwnerId;
            var userId = orderRequest.RequestNavigation.AuthorId;
            var process = new ThesisTestAPI.Entities.Process
            {
                ProcessId = Guid.NewGuid(),
                RequestId = request.RequestId,
                Description = request.Description,
                Title = request.Title,
                CreatedAt = DateTimeOffset.Now,
                Status = ProcessStatuses.CREATED
            };
            var existingConvo = await _db.Conversations.Include(q => q.ConversationMembers)
                .Where(c =>
                    c.ConversationMembers.Any(m => m.UserId == sellerId) &&
                    c.ConversationMembers.Any(m => m.UserId == userId)
                ).FirstOrDefaultAsync();
            if(existingConvo == null)
            {
                var conversation = new ThesisTestAPI.Entities.Conversation
                {
                    ConversationId = Guid.NewGuid(),
                    ConversationName = process.Title,
                    IsGroup = 0,
                    CreatedAt = DateTimeOffset.Now
                };
                var sellerMember = new ConversationMember
                {
                    MemberId = Guid.NewGuid(),
                    ConversationId = conversation.ConversationId,
                    UserId = sellerId,
                    JoinedAt = DateTimeOffset.Now,
                };
                var userMember = new ConversationMember
                {
                    MemberId = Guid.NewGuid(),
                    ConversationId = conversation.ConversationId,
                    UserId = userId,
                    JoinedAt = DateTimeOffset.Now,
                };
                _db.Conversations.Add(conversation);
                _db.ConversationMembers.Add(userMember);
                _db.ConversationMembers.Add(sellerMember);
            }

            _db.Processes.Add(process);
            await _db.SaveChangesAsync();
            
            var receiverId = orderRequest.RequestNavigation.AuthorId;
            await _notifService.SendNotification("New Process has been created by seller", receiverId);
            return new ProcessResponse
            {
                ProcessId = process.ProcessId
            };
        }
    }
}
