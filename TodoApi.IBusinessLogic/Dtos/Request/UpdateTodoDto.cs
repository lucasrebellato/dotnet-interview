using System.ComponentModel.DataAnnotations;

namespace TodoApi.IBusinessLogic.Dtos.Request;
public class UpdateTodoDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Title { get; set; }

    [Required(AllowEmptyStrings = false)]
    public required string Description { get; set; }
}
