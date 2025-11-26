using Microsoft.AspNetCore.Mvc;
using TodoApi.Filters;
using TodoApi.Hubs;
using TodoApi.ServiceFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<ValidateModelFilter>();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Register app-level ASP.NET services here:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddControllers();

builder.Services.AddSignalR();

// Add project services from the factory
builder.Services.AddServices(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();

// Swagger Middlewares
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    options.RoutePrefix = ""; // Swagger en la raíz
});

app.UseCors("CustomPolicy");

app.UseAuthorization();

app.MapControllers();

app.MapHub<TodoHub>("/hubs/todos");

app.Run();
