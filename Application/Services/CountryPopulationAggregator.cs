using Application;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Services.Application;

public class CountryPopulationAggregator : ICountryPopulationAggregator
{
    private readonly IStatService _dbService;
    private readonly IStatService _externalService;
    private readonly ILogger<CountryPopulationAggregator> _logger;

    public CountryPopulationAggregator(IEnumerable<IStatService> statServices, ILogger<CountryPopulationAggregator> logger)
    {
        _dbService = statServices.ElementAt(0) ?? throw new ArgumentNullException(nameof(_dbService));
        _externalService = statServices.ElementAt(1) ?? throw new ArgumentNullException(nameof(_externalService));
        _logger = logger;
    }

    public async Task<IEnumerable<Country>> GetCombinedAsync(CancellationToken ct = default)
    {
        try
        {
            _logger?.LogInformation("Starting to aggregate country populations from DB and external API");
            var dbTaskList = new List<Task<List<Country>>> 
            { 
                SafeRunTask(_dbService.GetCountryPopulationsAsync) ,
                SafeRunTask(_externalService.GetCountryPopulationsAsync) 
            };

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

            _logger?.LogInformation("Aggregated {Count} countries (DB overrides external)", result.Count);
            return result.Values.OrderBy(c => c.Name).ToList();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failure during statistics data aggregation");
        }

        return Enumerable.Empty<Country>();
    }

    private async Task<List<Country>> SafeRunTask(Func<Task<List<Country>>> func)
    {
        try
        {
            var result = await func();
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to retrieve country populations from " + func.Method.Name + ex);
        }
        return null;
    }
}
