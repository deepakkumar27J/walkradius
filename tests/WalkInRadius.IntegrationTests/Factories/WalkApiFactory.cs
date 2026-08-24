using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using WalkInRadius.Domain.Entities;
using WalkInRadius.Domain.Interfaces;
using WalkInRadius.Domain.ValueObjects;


namespace WalkInRadius.IntegrationTests.Factories;

public class WalkApiFactory : WebApplicationFactory<Program>
{
    public Mock<IRouteGenerator> RouteGeneratorMock { get; } = new ();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //remove real IRouteGenerator
            services.RemoveAll<IRouteGenerator>();

            //replace with mock
            services.AddScoped<IRouteGenerator>(_ => RouteGeneratorMock.Object);
        });

        builder.UseEnvironment("Development");
    }
}
