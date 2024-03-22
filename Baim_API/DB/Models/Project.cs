namespace DB.Models; 
public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; }
	public byte[]? DesignTheme { get; set; }
	// Del '?'
	public byte[]? Avatar { get; set; }
    // Del '?' 
    // TeamMembers
    //public IEnumerable<AspNetUser>? TeamMembers { get; set; }
    //// Admins 
    //public IEnumerable<AspNetUser>? ProjectAdmins { get; set; }
    // Tasks
    public IEnumerable<TaskOfProject>? TasksOfProject { get; set; }
}