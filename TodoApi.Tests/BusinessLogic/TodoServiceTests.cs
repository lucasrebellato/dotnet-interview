using System.Linq.Expressions;
using Moq;
using TodoApi.BusinessLogic.Services;
using TodoApi.BusinessLogic.Interfaces;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IDataAccess;
using TodoApi.Domain.Domain;
using TodoApi.IBusinessLogic.Exceptions;
using TodoApi.IBusinessLogic.INotifier;

namespace TodoApi.Tests.BusinessLogic;

public class TodoServiceTests
{
    private static (TodoService service, Mock<IGenericRepository<Todo>> repoMock, Mock<ITodoListInternalService> todoListServiceMock, Mock<ITodoNotifier> notifierMock) CreateSut()
    {
        var repoMock = new Mock<IGenericRepository<Todo>>();
        var todoListServiceMock = new Mock<ITodoListInternalService>();
        var notifierMock = new Mock<ITodoNotifier>();
        var service = new TodoService(repoMock.Object, todoListServiceMock.Object, notifierMock.Object);
        return (service, repoMock, todoListServiceMock, notifierMock);
    }

    [Fact]
    public async Task Create_WhenTodoListExists_AddsTodoAndReturnsDto()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        long listId = 2;

        todoListServiceMock.Setup(s => s.Exists(listId)).Returns(Task.CompletedTask);

        repoMock
            .Setup(r => r.Add(It.IsAny<Todo>()))
            .Returns(Task.CompletedTask)
            .Callback<Todo>(t => t.Id = 42);

        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var dto = new CreateTodoDto { Title = "New todo", Description = "Details" };

        var result = await service.Create(listId, dto);

        repoMock.Verify(r => r.Add(It.Is<Todo>(t => t.Title == "New todo" && t.Description == "Details" && t.TodoListId == listId)), Times.Once);
        Assert.Equal(42, result.Id);
        Assert.Equal("New todo", result.Title);
    }

    [Fact]
    public async Task Delete_WhenFound_CallsRepositoryDelete_AndSaves()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var todo = new Todo { Id = 5, Title = "t", Description = "x", TodoListId = 1 };
        var list = new TodoList { Id = 1, Name = "L", Todos = new List<Todo> { todo } };

        // The service obtains the todo via ITodoListInternalService.GetByIdWithIncludes(...)
        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(1, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        repoMock
            .Setup(r => r.Delete(It.IsAny<Todo>()))
            .Returns(Task.CompletedTask);

        await service.Delete(1, 5);

        repoMock.Verify(r => r.Delete(It.Is<Todo>(t => t.Id == 5)), Times.Once);
        // Delete does not call SaveChangesAsync in current implementation
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(1, It.IsAny<string[]>()))
            .ReturnsAsync((TodoList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Delete(1, 100));
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsMappedDto()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var todo = new Todo { Id = 7, Title = "Do it", Description = "Do it details", IsCompleted = true };
        var list = new TodoList { Id = 3, Name = "L", Todos = new List<Todo> { todo } };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(3, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        var result = await service.GetById(3, 7);

        Assert.Equal(7, result.Id);
        Assert.Equal("Do it", result.Title);
        Assert.True(result.IsCompleted);
        notifierMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetById_WhenTodoNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var list = new TodoList { Id = 3, Name = "L", Todos = new List<Todo>() };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(3, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetById(3, 999));
        notifierMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task MarkAsCompleted_WhenFound_SetsCompleted_UpdatesAndSaves()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var todo = new Todo { Id = 11, Title = "t", Description = "t desc", IsCompleted = false, TodoListId = 4 };
        var list = new TodoList { Id = 4, Name = "L", Todos = new List<Todo> { todo } };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(4, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        repoMock.Setup(r => r.Update(It.IsAny<Todo>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await service.MarkAsCompleted(4, 11);

        repoMock.Verify(r => r.Update(It.Is<Todo>(t => t.Id == 11 && t.IsCompleted)), Times.Once);
    }

    [Fact]
    public async Task MarkAsCompleted_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var list = new TodoList { Id = 4, Name = "L", Todos = new List<Todo>() };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(4, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        await Assert.ThrowsAsync<NotFoundException>(() => service.MarkAsCompleted(4, 77));
        notifierMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_WhenFound_UpdatesDescriptionAndReturnsDto_AndSaves()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var todo = new Todo { Id = 21, Title = "old", Description = "old desc", TodoListId = 6 };
        var list = new TodoList { Id = 6, Name = "L", Todos = new List<Todo> { todo } };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(6, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        repoMock.Setup(r => r.Update(It.IsAny<Todo>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await service.Update(6, 21, new UpdateTodoDto { Title = "new", Description = "new desc" });

        repoMock.Verify(r => r.Update(It.Is<Todo>(t => t.Id == 21 && t.Title == "new" && t.Description == "new desc")), Times.Once);
    }

    [Fact]
    public async Task Update_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var list = new TodoList { Id = 6, Name = "L", Todos = new List<Todo>() };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(6, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Update(6, 999, new UpdateTodoDto { Title = "x", Description = "x" }));
        notifierMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task MarkAllAsCompleted_WhenListHasTodos_UpdatesFlags_SavesAndNotifiesAllCompleted()
    {
        var (service, repoMock, todoListServiceMock, notifierMock) = CreateSut();
        var todos = new List<Todo>
        {
            new Todo { Id = 1, Title = "a", Description = "a", IsCompleted = false },
            new Todo { Id = 2, Title = "b", Description = "b", IsCompleted = false },
            new Todo { Id = 3, Title = "c", Description = "c", IsCompleted = true }
        };
        var list = new TodoList { Id = 9, Name = "L", Todos = todos };

        todoListServiceMock
            .Setup(s => s.GetByIdWithIncludes(9, It.IsAny<string[]>()))
            .ReturnsAsync(list);

        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        notifierMock.Setup(n => n.NotifyAllCompleted(It.IsAny<long>())).Returns(Task.CompletedTask);

        await service.MarkAllAsCompleted(9);

        // expect the service to persist changes via SaveChangesAsync
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        notifierMock.Verify(n => n.NotifyAllCompleted(9), Times.Once);
    }
}