using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.IntegrationTests;

public class TestingFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    private MongoDbRunner? _runner;

    private bool _isDataInitialaized = false;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Mongo2Go is not supported on ARM64 so use real MongoDB instance
        Console.WriteLine($"[ARCH]: {RuntimeInformation.ProcessArchitecture}");

        // var connectionString = string.Empty;
        // if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
        // {
        //     connectionString = "mongodb+srv://api:7a3O9po3bHO4b6em@cluster0.c6qpf78.mongodb.net/?retryWrites=true&w=majority";
        // }
        // else 
        // {
        //     _runner = MongoDbRunner.Start();
        //     connectionString = _runner.ConnectionString;
        // }

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // context.HostingEnvironment.EnvironmentName = "Test";

            config
                .AddJsonFile($"appsettings.Test.json", optional: true, reloadOnChange: true);
            if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
                _runner = MongoDbRunner.Start();

                var dbConfig = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        { "ConnectionStrings:MongoDb", _runner.ConnectionString }
                    })
                    .Build();

                config.AddConfiguration(dbConfig);
            }
        });
    }

    public void InitialaizeDatabase()
    {
        if (_isDataInitialaized) return;

        using var scope = Services.CreateScope();
        var mongodbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        var initialaizer = new DbInitializer(mongodbContext);
        initialaizer.InitializeDb();

        _isDataInitialaized = true;
    }

    protected override void Dispose(bool disposing)
    {
        _runner?.Dispose();
        base.Dispose(disposing);
    }
}