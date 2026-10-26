using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.Models;
using DishManagement_BIT240098.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DishManagement_BIT240098.Controllers;
[ApiController][Route("api/auth")]
public class AuthApiController(AppDbContext db,IPasswordHasher<AppUser_BIT240098> hasher,JwtTokenService jwt):ControllerBase {
 public record LoginRequest(string Username,string Password);
 [AllowAnonymous][HttpPost("login")]
 public async Task<IActionResult> Login([FromBody]LoginRequest request) {
  if(string.IsNullOrWhiteSpace(request.Username)||string.IsNullOrWhiteSpace(request.Password))return BadRequest(new{message="Username and password required"});
  var user=await db.Users.SingleOrDefaultAsync(x=>x.Username==request.Username);
  if(user==null||hasher.VerifyHashedPassword(user,user.PasswordHash,request.Password)==PasswordVerificationResult.Failed)return Unauthorized(new{message="Invalid credentials"});
  return Ok(new{accessToken=jwt.Create(user),tokenType="Bearer",expiresIn=1800,role=user.Role,username=user.Username});
 }
 [Authorize][HttpGet("me")] public IActionResult Me()=>Ok(new{username=User.Identity?.Name,role=User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,message="Authorized"});
 [Authorize(Roles="Admin")][HttpGet("admin")]public IActionResult Admin()=>Ok(new{message="Admin access granted"});
 [Authorize(Roles="User,Admin")][HttpGet("user")]public IActionResult UserArea()=>Ok(new{message="User access granted"});
}
