using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Database
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetCountryPopulationAsync();
    }
}
