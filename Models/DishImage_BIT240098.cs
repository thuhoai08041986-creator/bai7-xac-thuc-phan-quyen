using System.ComponentModel.DataAnnotations;
namespace DishManagement_BIT240098.Models;
public class DishImage_BIT240098 {
 public int Id {get;set;}
 [Required(ErrorMessage="Required")][Url(ErrorMessage="InvalidUrl")] public string ImageUrl {get;set;} = "";
 public bool IsThumbnail {get;set;}
 public int DishId {get;set;}
 public Dish_BIT240098? Dish {get;set;}
}
