using Application.Interfaces.Notifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tests.Integration;

public class GarageHubWebApplicationFactory
    : WebApplicationFactory<Program>
{
    public FakeEmailSender EmailSender { get; } = new();

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            "Host=localhost;Port=5432;Database=garagehub;Username=postgres;Password=postgres",

                        ["Jwt:Key"] =
                            "GarageHub-Development-Jwt-Key-2026-FIAP-Local-Environment"
                    });
            });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(EmailSender);
        });
    }
}
