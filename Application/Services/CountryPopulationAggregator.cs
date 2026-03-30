using Application;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Services.Application;

public class CountryPopulationAggregator : ICountryPopulationAggregator
{
    private readonly IEnumerable<IStatService> _statServices;
    private readonly ILogger<CountryPopulationAggregator> _logger;

    public CountryPopulationAggregator(IEnumerable<IStatService> statServices, ILogger<CountryPopulationAggregator> logger)
    {
        _statServices = statServices;
        _logger = logger;
    }

    public async Task<IEnumerable<Country>> GetCombinedAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Starting to aggregate country populations from DB and external API");
            var dbTaskList = new List<Task<List<Country>>>();
            foreach (var statService in _statServices) 
            {
                dbTaskList.Add(SafeRunTask(statService.GetCountryPopulationsAsync, statService.GetType().Name));
            }

            await Task.WhenAll(dbTaskList);
            var result = new Dictionary<string, Country>(StringComparer.OrdinalIgnoreCase);
            foreach (var task in dbTaskList)
            {
                var statisticsData = task.Result ?? Enumerable.Empty<Country>();
                foreach (var item in statisticsData)
                {
                    if (item.Name != null && !result.ContainsKey(item.Name))
                    {
                        result[item.Name] = item;
                    }
                }
            }

            _logger.LogInformation("Aggregated population for {Count} countries ", result.Count);
            return result.Values.OrderBy(c => c.Name).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure during statistics data aggregation");
        }

        return Enumerable.Empty<Country>();
    }

    private async Task<List<Country>> SafeRunTask(Func<Task<List<Country>>> func, string serviceName)
    {
        try
        {
            var result = await func();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve country populations from " + serviceName);
        }
        return new List<Country>();
    }
}
