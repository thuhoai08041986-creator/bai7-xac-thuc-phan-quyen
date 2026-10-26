using DishManagement_BIT240098.Models;
namespace DishManagement_BIT240098.ViewModels;
public class HomeViewModel {
 public int TotalDishes {get;set;}
 public int ActiveDishes {get;set;}
 public int InactiveDishes {get;set;}
 public int TotalCategories {get;set;}
 public List<Dish_BIT240098> Latest {get;set;} = new();
}
