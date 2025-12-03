using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.User
{
    public class GetAllLoggedInUsersHandler : IRequestHandler<GetAllLoggedInUsersRequest, (ProblemDetails?, PaginatedUserResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetAllLoggedInUsersHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedUserResponse?)> Handle(GetAllLoggedInUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _db.Users.Skip((request.pageNumber - 1) * request.pageSize).OrderByDescending(q => q.CreatedAt).Where(q=>q.RefreshTokenExpiryTime >= DateTime.Now).ToListAsync();
            var list = new List<UserResponse>();
            foreach(var user in users)
            {
                var item = new UserResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Latitude = user.Location.Coordinate.Y,
                    Longitude = user.Location.Coordinate.X,
                    Address = user.Address,
                    PostalCode = user.PostalCode,
                };
                if (!string.IsNullOrEmpty(user.Pfp))
                {
                    item.Pfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                }
                list.Add(item);
            }
            var total = await _db.Users.CountAsync();
            return (null, new PaginatedUserResponse
            {
                Total = total,
                Users = list
            });
        }
    }
}
