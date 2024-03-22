namespace DB.Models;

public class TaskOfProject
{
	public int Id { get; set; }
	public string Title { get; set; }
	public bool IsVeryImportant { get; set; } = false;
	public string? Description { get; set; }
	public bool IsExpired { get; set; } = false; // Просрочена
	// Del '?'
	public DateTime? StartDate { get; set; }
	// Del '?'
	public DateTime? DeadLine { get; set; }

	// Del ?
	public int? ProjectId { get; set; }
	public Project? Project { get; set; }

	//public IEnumerable<AspNetUser>? Executors { get; set; }
	//public IEnumerable<AspNetUser>? CoExecutors { get; set; }
	//public IEnumerable<AspNetUser>? Observers { get; set; }
}