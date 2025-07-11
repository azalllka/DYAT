namespace DYAT.Web.Areas.Admin.Models;

public class UserViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public bool IsLocked { get; set; }
    public List<string> Roles { get; set; }
} 