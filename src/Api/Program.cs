using GarageHub.Api.Middleware;
using GarageHub.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/openapi/v1.json", "GarageHub API V1");
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();