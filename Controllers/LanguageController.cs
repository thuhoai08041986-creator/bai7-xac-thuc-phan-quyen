using Microsoft.AspNetCore.Mvc;
namespace DishManagement_BIT240098.Controllers;
public class LanguageController:Controller {
 [HttpPost][ValidateAntiForgeryToken] public IActionResult Change(string lang,string? returnUrl) {
  Response.Cookies.Append("dish_lang",lang=="vi"?"vi":"en",new CookieOptions{Expires=DateTimeOffset.UtcNow.AddYears(1),HttpOnly=true,SameSite=SameSiteMode.Lax,Secure=Request.IsHttps});
  return LocalRedirect(Url.IsLocalUrl(returnUrl)?returnUrl!:Url.Action("Index","Dishes")!);
 }
}
