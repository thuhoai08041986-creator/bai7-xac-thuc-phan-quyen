using Microsoft.AspNetCore.Mvc;
namespace DishManagement_BIT240098.Controllers;
public class AuthPageController:Controller { [HttpGet]public IActionResult Login()=>View(); }
