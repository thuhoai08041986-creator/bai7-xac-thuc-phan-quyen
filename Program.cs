using System.Text;
using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.Models;
using DishManagement_BIT240098.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher<AppUser_BIT240098>,PasswordHasher<AppUser_BIT240098>>();
var jwtKey=builder.Configuration["Jwt:Key"];
if(string.IsNullOrWhiteSpace(jwtKey)||Encoding.UTF8.GetByteCount(jwtKey)<32)throw new InvalidOperationException("Configure Jwt:Key (32+ bytes) via dotnet user-secrets.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>{
 o.TokenValidationParameters=new TokenValidationParameters {ValidateIssuer=true,ValidIssuer=builder.Configuration["Jwt:Issuer"],ValidateAudience=true,ValidAudience=builder.Configuration["Jwt:Audience"],ValidateIssuerSigningKey=true,IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),ValidateLifetime=true,ClockSkew=TimeSpan.FromSeconds(10)};
});
builder.Services.AddAuthorization();
var app=builder.Build();
if(!app.Environment.IsDevelopment()){app.UseExceptionHandler("/Dishes/Error");app.UseHsts();}
app.UseHttpsRedirection();app.UseStaticFiles();app.UseRouting();app.UseAuthentication();app.UseAuthorization();
app.MapControllers();app.MapControllerRoute(name:"default",pattern:"{controller=Home}/{action=Index}/{id?}");
app.Run();
