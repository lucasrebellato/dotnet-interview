using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.DataAccess;
using TodoApi.Domain.Domain;
using TodoApi.IDataAccess;

namespace TodoApi.Tests.Repositories;

public class GenericRepositoryTests
{
    private static TodoContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new TodoContext(options);
    }

    [Fact]
    public async Task Add_WhenCalled_AddsEntity()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new GenericRepository<TodoList>(context);

        var todo = new TodoList { Name = "Test Add" };

        await ((IGenericRepository<TodoList>)repo).Add(todo);

        var saved = await context.Set<TodoList>().FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Test Add", saved!.Name);
    }

    [Fact]
    public async Task GetAsync_WithPredicate_ReturnsEntity()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var seeded = new TodoList { Name = "Find me" };
        context.Set<TodoList>().Add(seeded);
        await context.SaveChangesAsync();

        var repo = new GenericRepository<TodoList>(context);
        var result = await ((IGenericRepository<TodoList>)repo).Get(x => x.Id == seeded.Id);

        Assert.NotNull(result);
        Assert.Equal("Find me", result!.Name);
    }

    [Fact]
    public async Task Get_SyncVariant_ReturnsEntity()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var seeded = new TodoList { Name = "SyncGet" };
        context.Set<TodoList>().Add(seeded);
        await context.SaveChangesAsync();

        var repo = new GenericRepository<TodoList>(context);
        var result = repo.Get(x => x.Id == seeded.Id);

        Assert.NotNull(result);
        Assert.Equal("SyncGet", result!.Name);
    }

    [Fact]
    public async Task Update_WhenCalled_UpdatesEntity()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var seeded = new TodoList { Name = "Before" };
        context.Set<TodoList>().Add(seeded);
        await context.SaveChangesAsync();

        seeded.Name = "After";

        var repo = new GenericRepository<TodoList>(context);
        await ((IGenericRepository<TodoList>)repo).Update(seeded);

        var updated = await context.Set<TodoList>().FirstOrDefaultAsync(x => x.Id == seeded.Id);
        Assert.NotNull(updated);
        Assert.Equal("After", updated!.Name);
    }

    [Fact]
    public async Task Delete_WhenEntityIsSoftDeletable_SetsIsDeleted()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var seeded = new TodoList { Name = "ToDelete" };
        context.Set<TodoList>().Add(seeded);
        await context.SaveChangesAsync();

        var repo = new GenericRepository<TodoList>(context);
        await ((IGenericRepository<TodoList>)repo).Delete(seeded);

        var loaded = await context.Set<TodoList>().FirstOrDefaultAsync(x => x.Id == seeded.Id);
        Assert.NotNull(loaded);
        Assert.True(loaded!.IsDeleted);
    }

    [Fact]
    public async Task GetAll_WithPredicate_ReturnsExpectedList()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var a = new TodoList { Name = "Keep" };
        var b = new TodoList { Name = "Remove", IsDeleted = true };
        context.Set<TodoList>().AddRange(a, b);
        await context.SaveChangesAsync();

        var repo = new GenericRepository<TodoList>(context);
        var result = await ((IGenericRepository<TodoList>)repo).GetAll(x => !x.IsDeleted);

        Assert.Single(result);
        Assert.Equal("Keep", result.First().Name);
    }
}