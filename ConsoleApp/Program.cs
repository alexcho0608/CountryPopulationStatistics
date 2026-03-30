using Application;
using Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Application;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddFile("logs/myapp.log");
                });
                services.AddSingleton<IDbConnectionManager>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString("Default");
                    return new DbConnectionManager(connectionString);
                }); 
                services.AddSingleton<ICountryRepository, CountryRepository>();
                services.AddSingleton<IStatService, Infrastructure.DbStatService>();
                services.AddSingleton<IStatService, Infrastructure.External.ExternaAPIStatService>();
                services.AddSingleton<ICountryPopulationAggregator, CountryPopulationAggregator>();
            })
            .Build();
        var countryAggregator = host.Services.GetRequiredService<ICountryPopulationAggregator>();
        var countriesPopulations = await countryAggregator.GetCombinedAsync();
        foreach (var country in countriesPopulations)
        {
            Console.WriteLine(String.Format("Country {0}: Population {1}", country.Name, country.Population));
        }
        Console.ReadLine();
    }

}

