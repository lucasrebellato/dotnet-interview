using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.BusinessLogic.Interfaces;
using TodoApi.BusinessLogic.Services;
using TodoApi.DataAccess;
using TodoApi.IBusinessLogic.INotifier;
using TodoApi.IBusinessLogic.Interfaces;
using TodoApi.IBusinessLogic.IServices;
using TodoApi.IDataAccess;
using TodoApi.Infrastructure.BackgroundJobs;
using TodoApi.Infrastructure.BackgroundJobs.Worker;
using TodoApi.Infrastructure.Interfaces;
using TodoApi.Infrastructure.Notifiers;

namespace TodoApi.ServiceFactory;

public static class TodoApiFactory
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Business Logic Services
        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<ITodoListInternalService, TodoListService>();

        // Background Jobs
        services.AddSingleton<IBackgroundJobService, TodoBackgroundService>();
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<BackgroundTaskHostedService>();

        // Notifier
        services.AddScoped<ITodoNotifier, TodoNotifier>();

        // Data Access
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // DbContext
        services.AddDbContext<TodoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TodoContext"))
        );
    }
}
