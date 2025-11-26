using Microsoft.AspNetCore.Mvc;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IBusinessLogic.Interfaces;

namespace TodoApi.Controllers;

[Route("api/todolists")]
[ApiController]
public class TodoListsController : ControllerBase
{
    private readonly ITodoListService _todoListService;

    public TodoListsController(ITodoListService todoListService)
    {
        _todoListService = todoListService;
    }

    [HttpGet]
    public async Task<ActionResult<IList<TodoListResponseDto>>> GetAll()
    {
        List<TodoListResponseDto> response = await _todoListService.GetAll();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListResponseDto>> GetById(long id)
    {
        TodoListResponseDto reponse = await _todoListService.GetById(id);
        return Ok(reponse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListResponseDto>> Update(long id, UpdateTodoListDto payload)
    {
        TodoListResponseDto response = await _todoListService.Update(id, payload);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TodoListResponseDto>> Create(CreateTodoListDto payload)
    {
        TodoListResponseDto reponse = await _todoListService.Create(payload);
        return CreatedAtAction(nameof(Create), reponse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _todoListService.Delete(id);
        return NoContent();
    }
}
