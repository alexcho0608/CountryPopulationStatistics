using Domain.Entities;

namespace Application;

public interface IStatService
{
    Task<List<Country>> GetCountryPopulationsAsync();
    // convenience synchronous wrapper
}
