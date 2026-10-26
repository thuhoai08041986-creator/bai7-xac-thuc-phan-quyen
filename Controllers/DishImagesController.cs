using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.Models;
using DishManagement_BIT240098.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DishManagement_BIT240098.Controllers;
public class DishImagesController(AppDbContext db,LanguageService language):Controller {
 string T(string k)=>language.T(HttpContext,k);
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Create(int dishId,string imageUrl,bool isThumbnail=false){
  if(!await db.Dishes.AnyAsync(d=>d.Id==dishId))return NotFound(T("NotFound"));
  if(!Uri.TryCreate(imageUrl,UriKind.Absolute,out var uri)||uri.Scheme!="https"&&uri.Scheme!="http") {TempData["Error"]="InvalidUrl";return RedirectToAction("Details","Dishes",new{id=dishId});}
  await using var tx=await db.Database.BeginTransactionAsync();
  var hasThumb=await db.Images.AnyAsync(x=>x.DishId==dishId&&x.IsThumbnail);
  if(isThumbnail||!hasThumb){await db.Images.Where(x=>x.DishId==dishId&&x.IsThumbnail).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.IsThumbnail,false));}
  db.Images.Add(new DishImage_BIT240098{DishId=dishId,ImageUrl=imageUrl.Trim(),IsThumbnail=isThumbnail||!hasThumb});await db.SaveChangesAsync();await tx.CommitAsync();TempData["Notice"]="Saved";return RedirectToAction("Details","Dishes",new{id=dishId});
 }
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> SetThumbnail(int id){var image=await db.Images.FindAsync(id);if(image==null)return NotFound(T("NotFound"));await using var tx=await db.Database.BeginTransactionAsync();await db.Images.Where(x=>x.DishId==image.DishId&&x.IsThumbnail).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.IsThumbnail,false));image.IsThumbnail=true;await db.SaveChangesAsync();await tx.CommitAsync();TempData["Notice"]="Saved";return RedirectToAction("Details","Dishes",new{id=image.DishId});}
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Delete(int id){var image=await db.Images.FindAsync(id);if(image==null)return NotFound(T("NotFound"));var dishId=image.DishId;var wasThumb=image.IsThumbnail;db.Images.Remove(image);await db.SaveChangesAsync();if(wasThumb){var next=await db.Images.Where(x=>x.DishId==dishId).OrderBy(x=>x.Id).FirstOrDefaultAsync();if(next!=null){next.IsThumbnail=true;await db.SaveChangesAsync();}}TempData["Notice"]="Deleted";return RedirectToAction("Details","Dishes",new{id=dishId});}
}
