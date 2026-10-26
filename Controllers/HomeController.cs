using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DishManagement_BIT240098.Controllers;
public class HomeController(AppDbContext db):Controller {
 public async Task<IActionResult> Index() {
  var vm=new HomeViewModel {
   TotalDishes=await db.Dishes.CountAsync(),
   ActiveDishes=await db.Dishes.CountAsync(x=>x.IsAvailable),
   InactiveDishes=await db.Dishes.CountAsync(x=>!x.IsAvailable),
   TotalCategories=await db.Categories.CountAsync(),
   Latest=await db.Dishes.AsNoTracking().Include(x=>x.DishCategory).Include(x=>x.Images).OrderByDescending(x=>x.Id).Take(6).ToListAsync()
  };
  return View(vm);
 }
}
