using Microsoft.AspNetCore.Mvc;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.IServices;

namespace TodoApi.Controllers;

[Route("api/todolists/{todoListId}/todo")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly IBackgroundJobService _backgroundJobService;

    public TodosController(ITodoService todoService, IBackgroundJobService backgroundJobService)
    {
        _todoService = todoService;
        _backgroundJobService = backgroundJobService;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromRoute] long todoListId, [FromBody] CreateTodoDto payload)
    {
        var response = await _todoService.Create(todoListId, payload);
        return CreatedAtAction(nameof(Create), response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById([FromRoute] long todoListId, [FromRoute] long id)
    {
        var response = await _todoService.GetById(todoListId, id);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] long todoListId, [FromRoute] long id, [FromBody] UpdateTodoDto payload)
    {
        var response = await _todoService.Update(todoListId, id, payload);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] long todoListId, [FromRoute] long id)
    {
        await _todoService.Delete(id);
        return NoContent();
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult> MarkAsCompleted([FromRoute] long todoListId, [FromRoute] long id)
    {
        await _todoService.MarkAsCompleted(todoListId, id);
        return NoContent();
    }

    [HttpPost("complete-all")]
    public ActionResult MarkAllAsCompleted([FromRoute] long todoListId)
    {
        _backgroundJobService.EnqueueMarkAllTodosCompleted(todoListId);
        return Accepted();
    }
}
