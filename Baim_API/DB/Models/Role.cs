using static DB.Services.Classes.Enums;
namespace DB.Models;
public class Role
{
	public int Id { get; set; }
	public string RoleName { get; set; } = "Observer"; 
}