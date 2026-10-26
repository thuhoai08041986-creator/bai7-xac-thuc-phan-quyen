using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DishManagement_BIT240098.Models;
using Microsoft.IdentityModel.Tokens;
namespace DishManagement_BIT240098.Services;
public class JwtTokenService(IConfiguration config) {
 public string Create(AppUser_BIT240098 user) {
  var key=config["Jwt:Key"] ?? throw new InvalidOperationException("Set Jwt:Key using user-secrets (at least 32 bytes).");
  if(Encoding.UTF8.GetByteCount(key)<32) throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");
  var claims=new[]{new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),new Claim(ClaimTypes.Name,user.Username),new Claim(ClaimTypes.Role,user.Role),new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())};
  var jwt=new JwtSecurityToken(issuer:config["Jwt:Issuer"],audience:config["Jwt:Audience"],claims:claims,expires:DateTime.UtcNow.AddMinutes(30),signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256));
  return new JwtSecurityTokenHandler().WriteToken(jwt);
 }
}
