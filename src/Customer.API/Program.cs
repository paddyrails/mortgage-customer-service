using Microsoft.EntityFrameworkCore;
using Customer.API.Data;
using Customer.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseInMemoryDatabase("CustomerDb"));

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Customer Service API", 
        Version = "v1",
        Description = "Microservice for managing customer information"
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    context.Database.EnsureCreated();
}

var port = Environment.GetEnvironmentVariable("PORT") ?? "5001";
app.Urls.Add($"http://+:{port}");

Console.WriteLine($"Customer Service starting on port {port}...");

app.Run();
