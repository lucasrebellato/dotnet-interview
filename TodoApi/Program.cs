using TodoApi.ServiceFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Register app-level ASP.NET services here:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Add project services from the factory
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();