namespace TodoApi.IBusinessLogic.Dtos.Request;
public class UpdateTodoDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
}
