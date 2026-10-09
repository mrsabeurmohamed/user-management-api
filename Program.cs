using Microsoft.Extensions.DependencyInjection;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var bearerToken = builder.Configuration["Authentication:BearerToken"];
if (string.IsNullOrWhiteSpace(bearerToken))
{
    throw new InvalidOperationException(
        "Configure Authentication:BearerToken using application configuration or the " +
        "Authentication__BearerToken environment variable.");
}

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<BearerTokenMiddleware>(bearerToken);
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();