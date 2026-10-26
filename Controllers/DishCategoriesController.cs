using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.Models;
using DishManagement_BIT240098.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DishManagement_BIT240098.Controllers;
public class DishCategoriesController(AppDbContext db,LanguageService language):Controller {
 string T(string k)=>language.T(HttpContext,k);
 public async Task<IActionResult> Index()=>View(await db.Categories.AsNoTracking().Select(c=>new CategoryRow{Id=c.Id,Name=c.Name,Description=c.Description,Count=c.Dishes.Count}).OrderBy(c=>c.Name).ToListAsync());
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Create([Bind("Name,Description")]DishCategory_BIT240098 c){if(!ModelState.IsValid){TempData["Error"]="Required";return RedirectToAction(nameof(Index));}db.Categories.Add(c);await db.SaveChangesAsync();TempData["Notice"]="Saved";return RedirectToAction(nameof(Index));}
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Edit(int id,string name,string? description){var c=await db.Categories.FindAsync(id);if(c==null)return NotFound(T("NotFound"));if(string.IsNullOrWhiteSpace(name)){TempData["Error"]="Required";return RedirectToAction(nameof(Index));}c.Name=name.Trim();c.Description=description;await db.SaveChangesAsync();TempData["Notice"]="Saved";return RedirectToAction(nameof(Index));}
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Delete(int id){var c=await db.Categories.FindAsync(id);if(c==null)return NotFound(T("NotFound"));if(await db.Dishes.AnyAsync(d=>d.DishCategoryId==id)){TempData["Error"]="CategoryInUse";return RedirectToAction(nameof(Index));}db.Categories.Remove(c);try{await db.SaveChangesAsync();}catch(DbUpdateException){TempData["Error"]="CategoryInUse";return RedirectToAction(nameof(Index));}TempData["Notice"]="Deleted";return RedirectToAction(nameof(Index));}
 public class CategoryRow {public int Id{get;set;}public string Name{get;set;}="";public string? Description{get;set;}public int Count{get;set;}}
}
