using System.ComponentModel.DataAnnotations;
namespace DishManagement_BIT240098.Models;
public class DishCategory_BIT240098 {
 public int Id {get;set;}
 [Required(ErrorMessage="Required")] public string Name {get;set;} = "";
 public string? Description {get;set;}
 public ICollection<Dish_BIT240098> Dishes {get;set;} = new List<Dish_BIT240098>();
}
