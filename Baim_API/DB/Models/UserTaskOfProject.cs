namespace DB.Models;
public class UserTaskOfProject
{
	public int Id { get; set; }

	public string UserId { get; set; }
	public AspNetUser User { get; set; }

	public int TaskOfProjectId { get; set; }
	public TaskOfProject TaskOfProject { get; set; }

	//// Del '?'
	//public string? ExecutorId { get; set; }
	//public AspNetUser? Executor { get; set; }

	////// Del '?'
	//public string? CoExecutorId { get; set; }
	//public AspNetUser? CoExecutor { get; set; }

	////// Del '?'
	//public string? ObserverId { get; set; }
	//public AspNetUser? Observer { get; set; }
}