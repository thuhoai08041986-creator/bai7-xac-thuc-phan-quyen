using System.ComponentModel.DataAnnotations;
namespace DishManagement_BIT240098.Models;
public class Dish_BIT240098 {
 public int Id {get;set;}
 [Required(ErrorMessage="Required")] public string Name {get;set;} = "";
 [Range(typeof(decimal),"0.01","999999999999",ErrorMessage="Positive")] public decimal Price {get;set;}
 [Range(1,int.MaxValue,ErrorMessage="Positive")] public int PreparationTime {get;set;}
 public bool IsAvailable {get;set;} = true;
 public string? Description {get;set;}
 [Range(1,int.MaxValue,ErrorMessage="Required")] public int DishCategoryId {get;set;}
 public DishCategory_BIT240098? DishCategory {get;set;}
 public ICollection<DishImage_BIT240098> Images {get;set;} = new List<DishImage_BIT240098>();
}
