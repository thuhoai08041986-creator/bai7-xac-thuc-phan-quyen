namespace DishManagement_BIT240098.Models;
public class AppUser_BIT240098 {
 public int Id {get;set;}
 public string Username {get;set;} = "";
 public string PasswordHash {get;set;} = "";
 public string Role {get;set;} = "User";
}
