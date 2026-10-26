using DishManagement_BIT240098.Models;
using Microsoft.EntityFrameworkCore;
namespace DishManagement_BIT240098.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options) {
 public DbSet<AppUser_BIT240098> Users => Set<AppUser_BIT240098>();
 public DbSet<Dish_BIT240098> Dishes => Set<Dish_BIT240098>();
 public DbSet<DishCategory_BIT240098> Categories => Set<DishCategory_BIT240098>();
 public DbSet<DishImage_BIT240098> Images => Set<DishImage_BIT240098>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<AppUser_BIT240098>(e=>{e.ToTable("BIT240098_Users");e.HasKey(x=>x.Id);e.Property(x=>x.Id).HasColumnName("BIT240098_Id");e.Property(x=>x.Username).HasColumnName("BIT240098_Username").HasMaxLength(100).IsRequired();e.HasIndex(x=>x.Username).IsUnique();e.Property(x=>x.PasswordHash).HasColumnName("BIT240098_PasswordHash").IsRequired();e.Property(x=>x.Role).HasColumnName("BIT240098_Role").HasMaxLength(20).IsRequired();});
  b.Entity<DishCategory_BIT240098>(e=>{e.ToTable("BIT240098_DishCategories"); e.HasKey(x=>x.Id); e.Property(x=>x.Id).HasColumnName("BIT240098_Id"); e.Property(x=>x.Name).HasColumnName("BIT240098_Name").HasMaxLength(150).IsRequired(); e.Property(x=>x.Description).HasColumnName("BIT240098_Description");});
  b.Entity<Dish_BIT240098>(e=>{e.ToTable("BIT240098_Dishes"); e.HasKey(x=>x.Id); e.Property(x=>x.Id).HasColumnName("BIT240098_Id");e.Property(x=>x.Name).HasColumnName("BIT240098_Name").HasMaxLength(150).IsRequired();e.Property(x=>x.Price).HasColumnName("BIT240098_Price").HasColumnType("decimal(18,2)");e.Property(x=>x.PreparationTime).HasColumnName("BIT240098_PreparationTime");e.Property(x=>x.IsAvailable).HasColumnName("BIT240098_IsAvailable");e.Property(x=>x.Description).HasColumnName("BIT240098_Description");e.Property(x=>x.DishCategoryId).HasColumnName("BIT240098_DishCategoryId");e.HasOne(x=>x.DishCategory).WithMany(x=>x.Dishes).HasForeignKey(x=>x.DishCategoryId).OnDelete(DeleteBehavior.Restrict);e.HasIndex(x=>new{x.DishCategoryId,x.Name}).IsUnique();});
  b.Entity<DishImage_BIT240098>(e=>{e.ToTable("BIT240098_DishImages");e.HasKey(x=>x.Id);e.Property(x=>x.Id).HasColumnName("BIT240098_Id");e.Property(x=>x.ImageUrl).HasColumnName("BIT240098_ImageUrl").HasMaxLength(2048).IsRequired();e.Property(x=>x.IsThumbnail).HasColumnName("BIT240098_IsThumbnail");e.Property(x=>x.DishId).HasColumnName("BIT240098_DishId");e.HasOne(x=>x.Dish).WithMany(x=>x.Images).HasForeignKey(x=>x.DishId).OnDelete(DeleteBehavior.Cascade);e.HasIndex(x=>x.DishId).IsUnique().HasFilter("[BIT240098_IsThumbnail] = 1");});
 }
}
