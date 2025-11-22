using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.BusinessLogic.Interfaces;
using TodoApi.BusinessLogic.Services;
using TodoApi.DataAccess;
using TodoApi.IBusinessLogic.Interfaces;
using TodoApi.IBusinessLogic.IServices;
using TodoApi.IDataAccess;

namespace TodoApi.ServiceFactory;

public static class TodoApiFactory
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<ITodoListInternalService, TodoListService>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddDbContext<TodoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TodoContext"))
        );
    }
}
