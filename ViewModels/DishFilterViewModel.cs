using DishManagement_BIT240098.Models;
namespace DishManagement_BIT240098.ViewModels;
public class DishFilterViewModel {
 public string? Search {get;set;}
 public int? CategoryId {get;set;}
 public string? Availability {get;set;}
 public decimal? MinPrice {get;set;}
 public decimal? MaxPrice {get;set;}
 public string? Sort {get;set;}
 public List<Dish_BIT240098> Dishes {get;set;} = new();
 public List<DishCategory_BIT240098> Categories {get;set;} = new();
}
