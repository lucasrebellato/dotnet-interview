using System.ComponentModel.DataAnnotations;

namespace TodoApi.IBusinessLogic.Dtos.Request;

public class CreateTodoListDto
{
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }
}
