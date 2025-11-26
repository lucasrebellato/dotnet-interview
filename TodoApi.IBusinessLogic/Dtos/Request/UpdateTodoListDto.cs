using System.ComponentModel.DataAnnotations;

namespace TodoApi.IBusinessLogic.Dtos.Request;

public class UpdateTodoListDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}
