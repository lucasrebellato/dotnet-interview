using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using TodoApi.BusinessLogic.Exceptions;
using TodoApi.BusinessLogic.Services;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IDataAccess;
using TodoApi.Domain.Domain;
using Xunit;

namespace TodoApi.Tests.BusinessLogic;

public class TodoListServiceTests
{
    private static (TodoListService service, Mock<IGenericRepository<TodoList>> repoMock) CreateSut()
    {
        var repoMock = new Mock<IGenericRepository<TodoList>>();
        var service = new TodoListService(repoMock.Object);
        return (service, repoMock);
    }

    [Fact]
    public async Task Create_WhenCalled_MapsAndAddsEntity_ReturnsDto()
    {
        var (service, repoMock) = CreateSut();

        // Capture the entity passed to Add and simulate DB assigning an Id
        repoMock
            .Setup(r => r.Add(It.IsAny<TodoList>()))
            .Returns(Task.CompletedTask)
            .Callback<TodoList>(t => t.Id = 10);

        var dto = new CreateTodoListDto { Name = "New list" };

        var result = await service.Create(dto);

        repoMock.Verify(r => r.Add(It.Is<TodoList>(x => x.Name == "New list")), Times.Once);
        Assert.Equal(10, result.Id);
        Assert.Equal("New list", result.Name);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedList()
    {
        var (service, repoMock) = CreateSut();

        var items = new List<TodoList>
        {
            new TodoList { Id = 1, Name = "A" },
            new TodoList { Id = 2, Name = "B" }
        };

        repoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(items);

        var result = await service.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == "A");
        Assert.Contains(result, r => r.Name == "B");
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsMappedDto_WithTodos()
    {
        var (service, repoMock) = CreateSut();

        var todoList = new TodoList
        {
            Id = 5,
            Name = "List",
            Todos = new List<Todo>
            {
                new Todo { Id = 11, Description = "T1", IsCompleted = true },
                new Todo { Id = 12, Description = "T2", IsCompleted = false }
            }
        };

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(todoList);

        var result = await service.GetById(5);

        Assert.Equal(5, result.Id);
        Assert.Equal("List", result.Name);
        Assert.NotNull(result.Todos);
        Assert.Equal(2, result.Todos!.Count);
        Assert.Contains(result.Todos, t => t.Title == "T1" && t.IsCompleted);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock) = CreateSut();

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync((TodoList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetById(123));
    }

    [Fact]
    public async Task Update_WhenFound_UpdatesName_AndReturnsDto()
    {
        var (service, repoMock) = CreateSut();

        var existing = new TodoList { Id = 7, Name = "Old", Todos = new List<Todo>() };

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(existing);

        repoMock
            .Setup(r => r.Update(It.IsAny<TodoList>()))
            .Returns(Task.CompletedTask);

        var updateDto = new UpdateTodoListDto { Name = "New" };

        var result = await service.Update(7, updateDto);

        repoMock.Verify(r => r.Update(It.Is<TodoList>(t => t.Id == 7 && t.Name == "New")), Times.Once);
        Assert.Equal("New", result.Name);
        Assert.Equal(7, result.Id);
    }

    [Fact]
    public async Task Update_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock) = CreateSut();

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync((TodoList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Update(99, new UpdateTodoListDto { Name = "X" }));
    }

    [Fact]
    public async Task Delete_WhenFound_CallsRepositoryDelete()
    {
        var (service, repoMock) = CreateSut();

        var existing = new TodoList { Id = 3, Name = "ToRemove" };

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(existing);

        repoMock
            .Setup(r => r.Delete(It.IsAny<TodoList>()))
            .Returns(Task.CompletedTask);

        await service.Delete(3);

        repoMock.Verify(r => r.Delete(It.Is<TodoList>(t => t.Id == 3)), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ThrowsNotFoundException()
    {
        var (service, repoMock) = CreateSut();

        repoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TodoList, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync((TodoList?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.Delete(55));
    }
}