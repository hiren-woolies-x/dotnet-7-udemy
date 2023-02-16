using System.Security.Cryptography;
using System.Text;
using Api.Data;
using Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            this._context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(string userName, string password)
        {
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = userName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key,
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}