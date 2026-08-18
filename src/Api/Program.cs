using System.Text.Json.Serialization;
using GarageHub.Api.Middleware;
using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
//builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();
app.MapOpenApi();

app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/openapi/v1.json", "GarageHub API V1");
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();