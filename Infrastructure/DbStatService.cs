using Application;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class DbStatService : IStatService
{
    private readonly ILogger<DbStatService> _logger;
    ICountryRepository _countryRepository;

    public DbStatService(ICountryRepository countryRepository, ILogger<DbStatService> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<List<Country>> GetCountryPopulationsAsync()
    {
        var res = await _countryRepository.GetCountryPopulationAsync();
        return res;
    }
}
