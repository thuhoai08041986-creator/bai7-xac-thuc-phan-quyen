
using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DishManagement_BIT240098.Controllers
{
    [ApiController]
    [Route("api/dev")]
    public class DevSeedController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<AppUser_BIT240098> _hasher;

        public DevSeedController(
            IWebHostEnvironment env,
            IConfiguration config,
            AppDbContext db,
            IPasswordHasher<AppUser_BIT240098> hasher)
        {
            _env = env;
            _config = config;
            _db = db;
            _hasher = hasher;
        }

        public record SeedRequest(
            string Secret,
            string AdminPassword,
            string UserPassword,
            bool ResetExisting = false
        );

        [HttpPost("seed")]
        public async Task<IActionResult> Seed(
            [FromBody] SeedRequest request)
        {
            // Chi cho phep trong Development
            if (!_env.IsDevelopment())
                return NotFound();

            // Kiem tra ma bi mat
            var secret = _config["Seed:Secret"];

            if (string.IsNullOrWhiteSpace(secret) ||
                string.IsNullOrWhiteSpace(request.Secret))
            {
                return Unauthorized("Thieu ma xac thuc.");
            }

            var expected = Encoding.UTF8.GetBytes(secret);
            var provided = Encoding.UTF8.GetBytes(request.Secret);

            if (expected.Length != provided.Length ||
                !CryptographicOperations.FixedTimeEquals(
                    expected, provided))
            {
                return Unauthorized("Ma xac thuc khong dung.");
            }

            // Kiem tra mat khau
            if (string.IsNullOrWhiteSpace(request.AdminPassword) ||
                request.AdminPassword.Length < 12)
            {
                return BadRequest(
                    "Mat khau Admin phai co it nhat 12 ky tu.");
            }

            if (string.IsNullOrWhiteSpace(request.UserPassword) ||
                request.UserPassword.Length < 12)
            {
                return BadRequest(
                    "Mat khau User phai co it nhat 12 ky tu.");
            }

            var accounts = new[]
            {
                (Username: "admin",
                 Role: "Admin",
                 Password: request.AdminPassword),

                (Username: "user",
                 Role: "User",
                 Password: request.UserPassword)
            };

            var results = new List<string>();

            foreach (var account in accounts)
            {
                var existing = await _db.Users
                    .FirstOrDefaultAsync(
                        x => x.Username == account.Username);

                if (existing == null)
                {
                    var newUser = new AppUser_BIT240098
                    {
                        Username = account.Username,
                        Role = account.Role
                    };

                    newUser.PasswordHash =
                        _hasher.HashPassword(
                            newUser, account.Password);

                    _db.Users.Add(newUser);

                    results.Add(
                        $"Da tao tai khoan {account.Username}");
                }
                else if (request.ResetExisting)
                {
                    existing.Role = account.Role;

                    existing.PasswordHash =
                        _hasher.HashPassword(
                            existing, account.Password);

                    results.Add(
                        $"Da dat lai mat khau {account.Username}");
                }
                else
                {
                    results.Add(
                        $"Tai khoan {account.Username} da ton tai");
                }
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Xu ly tai khoan thanh cong",
                results
            });
        }
    }
}
