using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using TodoApi.BusinessLogic.Services;
using TodoApi.BusinessLogic.Interfaces;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IDataAccess;
using TodoApi.Domain.Domain;
using TodoApi.BusinessLogic.Exceptions;

namespace TodoApi.Tests.BusinessLogic;

public class TodoServiceTests
{
    private static (TodoService service, Mock<IGenericRepository<Todo>> repoMock, Mock<ITodoListInternalService> todoListServiceMock) CreateSut()
    {
        var repoMock = new Mock<IGenericRepository<Todo>>();
        var todoListServiceMock = new Mock<ITodoListInternalService>();
        var service = new TodoService(repoMock.Object, todoListServiceMock.Object);
        return (service, repoMock, todoListServiceMock);
    }

    [Fact]
    public async Task Create_WhenTodoListExists_AddsTodoAndReturnsDto()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        long listId = 2;

        todoListServiceMock.Setup(s => s.Exists(listId)).Returns(Task.CompletedTask);

        repoMock
            .Setup(r => r.Add(It.IsAny<Todo>()))
            .Returns(Task.CompletedTask)
            .Callback<Todo>(t => t.Id = 42);

        var dto = new CreateTodoDto { Description = "New todo" };

        var result = await service.Create(listId, dto);

        repoMock.Verify(r => r.Add(It.Is<Todo>(t => t.Description == "New todo" && t.TodoListId == listId)), Times.Once);
        Assert.Equal(42, result.Id);
        Assert.Equal("New todo", result.Title);
    }

    [Fact]
    public async Task Delete_WhenFound_CallsRepositoryDelete()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var todo = new Todo { Id = 5, Description = "x" };

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Todo, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(todo);

        repoMock
            .Setup(r => r.Delete(It.IsAny<Todo>()))
            .Returns(Task.CompletedTask);

        await service.Delete(5);

        repoMock.Verify(r => r.Delete(It.Is<Todo>(t => t.Id == 5)), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Todo, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync((Todo?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Delete(100));
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsMappedDto()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var todo = new Todo { Id = 7, Description = "Do it", IsCompleted = true };
        var list = new TodoList { Id = 3, Name = "L", Todos = new List<Todo> { todo } };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(3, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        var result = await service.GetById(3, 7);

        Assert.Equal(7, result.Id);
        Assert.Equal("Do it", result.Title);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task GetById_WhenTodoNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var list = new TodoList { Id = 3, Name = "L", Todos = new List<Todo>() };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(3, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetById(3, 999));
    }

    [Fact]
    public async Task MarkAsCompleted_WhenFound_SetsCompletedAndUpdates()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var todo = new Todo { Id = 11, Description = "t", IsCompleted = false };
        var list = new TodoList { Id = 4, Name = "L", Todos = new List<Todo> { todo } };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(4, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        repoMock.Setup(r => r.Update(It.IsAny<Todo>())).Returns(Task.CompletedTask);

        await service.MarkAsCompleted(4, 11);

        repoMock.Verify(r => r.Update(It.Is<Todo>(t => t.Id == 11 && t.IsCompleted)), Times.Once);
    }

    [Fact]
    public async Task MarkAsCompleted_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var list = new TodoList { Id = 4, Name = "L", Todos = new List<Todo>() };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(4, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        await Assert.ThrowsAsync<NotFoundException>(() => service.MarkAsCompleted(4, 77));
    }

    [Fact]
    public async Task Update_WhenFound_UpdatesDescriptionAndReturnsDto()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var todo = new Todo { Id = 21, Description = "old" };
        var list = new TodoList { Id = 6, Name = "L", Todos = new List<Todo> { todo } };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(6, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        repoMock.Setup(r => r.Update(It.IsAny<Todo>())).Returns(Task.CompletedTask);

        var result = await service.Update(6, 21, new UpdateTodoDto { Description = "new" });

        repoMock.Verify(r => r.Update(It.Is<Todo>(t => t.Id == 21 && t.Description == "new")), Times.Once);
        Assert.Equal(21, result.Id);
        Assert.Equal("new", result.Title);
    }

    [Fact]
    public async Task Update_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock) = CreateSut();
        var list = new TodoList { Id = 6, Name = "L", Todos = new List<Todo>() };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(6, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Update(6, 999, new UpdateTodoDto { Description = "x" }));
    }
}