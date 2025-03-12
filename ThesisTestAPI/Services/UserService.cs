using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI.Services
{
    public class UserService
    {
        private readonly ThesisDbContext _db;
        public UserService(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<List<User>> Get()
        {
            var users = await _db.Users.Select(q => new User
            {
                UserId = q.UserId,
                UserName = q.UserName,
                Email = q.Email,
                Password = q.Password,
                Phone = q.Phone,
                Rating = q.Rating
            }).ToListAsync();
            return users;
        }
    }
}
