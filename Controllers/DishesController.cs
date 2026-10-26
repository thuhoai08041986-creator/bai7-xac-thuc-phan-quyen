using DishManagement_BIT240098.Data;
using DishManagement_BIT240098.Models;
using DishManagement_BIT240098.Services;
using DishManagement_BIT240098.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace DishManagement_BIT240098.Controllers;
public class DishesController(AppDbContext db,LanguageService language):Controller {
 string T(string k)=>language.T(HttpContext,k);
 public async Task<IActionResult> Index(DishFilterViewModel f) {
  f.Categories=await db.Categories.AsNoTracking().OrderBy(c=>c.Name).ToListAsync();
  if(f.MinPrice<0 || f.MaxPrice<0 || f.MinPrice.HasValue && f.MaxPrice.HasValue && f.MinPrice>f.MaxPrice) ModelState.AddModelError("PriceRange",T("InvalidPrice"));
  var q=db.Dishes.AsNoTracking().Include(d=>d.DishCategory).Include(d=>d.Images).AsQueryable();
  if(!string.IsNullOrWhiteSpace(f.Search)) q=q.Where(d=>d.Name.Contains(f.Search.Trim()));
  if(f.CategoryId.HasValue) q=q.Where(d=>d.DishCategoryId==f.CategoryId.Value);
  if(f.Availability=="available") q=q.Where(d=>d.IsAvailable);
  else if(f.Availability=="unavailable") q=q.Where(d=>!d.IsAvailable);
  if(ModelState.IsValid) {if(f.MinPrice.HasValue) q=q.Where(d=>d.Price>=f.MinPrice);if(f.MaxPrice.HasValue) q=q.Where(d=>d.Price<=f.MaxPrice);}
  q=f.Sort switch {"price_asc"=>q.OrderBy(d=>d.Price).ThenBy(d=>d.Id),"price_desc"=>q.OrderByDescending(d=>d.Price).ThenBy(d=>d.Id),"time_asc"=>q.OrderBy(d=>d.PreparationTime).ThenBy(d=>d.Id),_=>q.OrderBy(d=>d.Id)};
  f.Dishes=ModelState.IsValid ? await q.ToListAsync() : new();return View(f);
 }
 static List<string> ParseImageUrls(string? imageUrls) => (imageUrls ?? "")
  .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
  .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
 void ValidateImages(List<string> urls, int thumbnailIndex) {
  if(urls.Count > 15) ModelState.AddModelError("imageUrls", T("TooManyImages"));
  if(urls.Any(x => x.Length > 2048 || !Uri.TryCreate(x, UriKind.Absolute, out var u) ||
    (u.Scheme != Uri.UriSchemeHttps && u.Scheme != Uri.UriSchemeHttp)))
   ModelState.AddModelError("imageUrls", T("InvalidUrl"));
  if(urls.Count > 0 && (thumbnailIndex < 0 || thumbnailIndex >= urls.Count))
   ModelState.AddModelError("thumbnailIndex", T("InvalidThumbnail"));
 }
 public async Task<IActionResult> Details(int id) {var d=await db.Dishes.AsNoTracking().Include(x=>x.DishCategory).Include(x=>x.Images).FirstOrDefaultAsync(x=>x.Id==id);if(d==null)return NotFound(T("NotFound"));return View(d);}
 async Task Categories(int? selected=null) {ViewBag.CategoryOptions=new SelectList(await db.Categories.OrderBy(x=>x.Name).ToListAsync(),"Id","Name",selected);}
 public async Task<IActionResult> Create(){await Categories();return View(new Dish_BIT240098());}
 async Task ValidateDish(Dish_BIT240098 d,int? exceptId=null) {
  if(string.IsNullOrWhiteSpace(d.Name)) ModelState.AddModelError(nameof(d.Name),T("Required"));
  if(d.Name?.Length>150) ModelState.AddModelError(nameof(d.Name),T("CategoryNameTooLong"));
  if(d.Price<=0) ModelState.AddModelError(nameof(d.Price),T("Positive"));
  if(d.PreparationTime<=0) ModelState.AddModelError(nameof(d.PreparationTime),T("Positive"));
  if(!await db.Categories.AnyAsync(c=>c.Id==d.DishCategoryId))ModelState.AddModelError(nameof(d.DishCategoryId),T("InvalidCategory"));
  if(!string.IsNullOrWhiteSpace(d.Name)) {d.Name=d.Name.Trim();if(await db.Dishes.AnyAsync(x=>x.DishCategoryId==d.DishCategoryId&&x.Name==d.Name&&(!exceptId.HasValue||x.Id!=exceptId.Value)))ModelState.AddModelError(nameof(d.Name),T("Duplicate"));}
 }
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Create([Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")]Dish_BIT240098 d, string? imageUrls, int thumbnailIndex = 0) {
  var urls = ParseImageUrls(imageUrls); ViewBag.ImageUrls = imageUrls; ViewBag.ThumbnailIndex = thumbnailIndex;
  ValidateImages(urls, thumbnailIndex); await ValidateDish(d);if(!ModelState.IsValid){LocalizeErrors();await Categories(d.DishCategoryId);return View(d);}
  db.Dishes.Add(d);
  for(int i=0;i<urls.Count;i++) d.Images.Add(new DishImage_BIT240098 { ImageUrl=urls[i], IsThumbnail=i==thumbnailIndex });
  try{await db.SaveChangesAsync();}catch(DbUpdateException){ModelState.AddModelError(nameof(d.Name),T("Duplicate"));LocalizeErrors();await Categories(d.DishCategoryId);return View(d);}
  TempData["Notice"]="Saved";return RedirectToAction(nameof(Details),new{id=d.Id});
 }
 public async Task<IActionResult> Edit(int id){var d=await db.Dishes.Include(x=>x.Images).FirstOrDefaultAsync(x=>x.Id==id);if(d==null)return NotFound(T("NotFound"));await Categories(d.DishCategoryId);return View(d);}
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Edit(int id,[Bind("Id,Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")]Dish_BIT240098 d, string? imageUrls, int thumbnailIndex = 0) {
  var urls = ParseImageUrls(imageUrls); ViewBag.ImageUrls = imageUrls; ViewBag.ThumbnailIndex = thumbnailIndex;
  ValidateImages(urls, thumbnailIndex);
  if(id!=d.Id)return BadRequest(T("InvalidRequest"));var original=await db.Dishes.Include(x=>x.Images).FirstOrDefaultAsync(x=>x.Id==id);if(original==null)return NotFound(T("NotFound"));
  await ValidateDish(d,id);if(!ModelState.IsValid){LocalizeErrors();await Categories(d.DishCategoryId);d.Images=original.Images.ToList();return View(d);}
  original.Name=d.Name;original.Price=d.Price;original.PreparationTime=d.PreparationTime;original.IsAvailable=d.IsAvailable;original.Description=d.Description;original.DishCategoryId=d.DishCategoryId;
  if(urls.Count>0) {
   // New images can replace the existing thumbnail when requested.
   if(original.Images.Any(x=>x.IsThumbnail))
    foreach(var img in original.Images.Where(x=>x.IsThumbnail)) img.IsThumbnail=false;
   for(int i=0;i<urls.Count;i++) original.Images.Add(new DishImage_BIT240098 {ImageUrl=urls[i],IsThumbnail=i==thumbnailIndex});
  }
  try{await db.SaveChangesAsync();}catch(DbUpdateException){ModelState.AddModelError(nameof(d.Name),T("Duplicate"));LocalizeErrors();await Categories(d.DishCategoryId);d.Images=original.Images.ToList();return View(d);}
  TempData["Notice"]="Saved";return RedirectToAction(nameof(Details),new{id});
 }
 [HttpPost][ValidateAntiForgeryToken]public async Task<IActionResult> Delete(int id){var d=await db.Dishes.FindAsync(id);if(d==null)return NotFound(T("NotFound"));db.Dishes.Remove(d);await db.SaveChangesAsync();TempData["Notice"]="Deleted";return RedirectToAction(nameof(Index));}
 void LocalizeErrors(){foreach(var entry in ModelState.Values){if(entry.Errors.Count==0)continue;var messages=entry.Errors.Select(x=>x.ErrorMessage).ToList();entry.Errors.Clear();foreach(var m in messages)entry.Errors.Add(T(m=="Required"||m=="Positive"?m:m.Contains("required",StringComparison.OrdinalIgnoreCase)?"Required":m.Contains("valid",StringComparison.OrdinalIgnoreCase)?"InvalidNumber":m));}}
 public IActionResult Error()=>View("Error");
}
