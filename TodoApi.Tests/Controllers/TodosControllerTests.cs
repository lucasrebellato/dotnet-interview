using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IBusinessLogic.IServices;

#nullable disable
namespace TodoApi.Tests.Controllers;

public class TodosControllerTests
{
    private static TodosController CreateController(Mock<ITodoService> serviceMock)
        => new TodosController(serviceMock.Object);

    [Fact]
    public async Task Create_WhenCalled_ReturnsCreatedAtActionWithItem()
    {
        var serviceMock = new Mock<ITodoService>();
        var todoResponse = new TodoResponseDto { Id = 10, Title = "T1", IsCompleted = false };

        serviceMock
            .Setup(s => s.Create(2, It.IsAny<CreateTodoDto>()))
            .ReturnsAsync(todoResponse);

        var controller = CreateController(serviceMock);

        var result = await controller.Create(2, new CreateTodoDto { Description = 1 });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<TodoResponseDto>(created.Value);
        Assert.Equal(10, value.Id);
        Assert.Equal("T1", value.Title);
        serviceMock.Verify(s => s.Create(2, It.IsAny<CreateTodoDto>()), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCalled_ReturnsOkWithItem()
    {
        var serviceMock = new Mock<ITodoService>();
        var todoResponse = new TodoResponseDto { Id = 5, Title = "Task", IsCompleted = true };

        serviceMock
            .Setup(s => s.GetById(2, 5))
            .ReturnsAsync(todoResponse);

        var controller = CreateController(serviceMock);

        var result = await controller.GetById(2, 5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<TodoResponseDto>(ok.Value);
        Assert.Equal(5, value.Id);
        Assert.Equal("Task", value.Title);
        serviceMock.Verify(s => s.GetById(2, 5), Times.Once);
    }

    [Fact]
    public async Task Update_WhenCalled_ReturnsOkWithUpdatedItem()
    {
        var serviceMock = new Mock<ITodoService>();
        var updated = new TodoResponseDto { Id = 7, Title = "Updated", IsCompleted = false };

        serviceMock
            .Setup(s => s.Update(3, 7, It.IsAny<UpdateTodoDto>()))
            .ReturnsAsync(updated);

        var controller = CreateController(serviceMock);

        var result = await controller.Update(3, 7, new UpdateTodoDto { Description = "Updated" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<TodoResponseDto>(ok.Value);
        Assert.Equal(7, value.Id);
        Assert.Equal("Updated", value.Title);
        serviceMock.Verify(s => s.Update(3, 7, It.IsAny<UpdateTodoDto>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenCalled_ReturnsNoContent_AndCallsServiceDelete()
    {
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.Delete(9)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        var result = await controller.Delete(1, 9);

        Assert.IsType<NoContentResult>(result);
        serviceMock.Verify(s => s.Delete(9), Times.Once);
    }

    [Fact]
    public async Task MarkAsCompleted_WhenCalled_ReturnsNoContent_AndCallsService()
    {
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.MarkAsCompleted(4, 12)).Returns(Task.CompletedTask);

        var controller = CreateController(serviceMock);

        var result = await controller.MarkAsCompleted(4, 12);

        Assert.IsType<NoContentResult>(result);
        serviceMock.Verify(s => s.MarkAsCompleted(4, 12), Times.Once);
    }
}