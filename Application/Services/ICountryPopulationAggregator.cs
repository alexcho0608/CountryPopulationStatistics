using Domain.Entities;

namespace Services.Application
{
    public interface ICountryPopulationAggregator
    {
        Task<IEnumerable<Country>> GetCombinedAsync(CancellationToken ct = default);
    }
}
