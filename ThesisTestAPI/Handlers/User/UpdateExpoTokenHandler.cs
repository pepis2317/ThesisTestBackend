using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Handlers.User
{
    public class UpdateExpoTokenHandler : IRequestHandler<UpdateExpoTokenRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public UpdateExpoTokenHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, string?)> Handle(UpdateExpoTokenRequest request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.Where(q => q.UserId == request.UserId).FirstOrDefaultAsync();
            user.ExpoPushToken = request.ExpoPushToken;
            await _db.SaveChangesAsync();
            return (null, "success");
        }
    }
}
