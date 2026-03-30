using Dapper;
using Domain.Entities;

namespace Infrastructure.Database
{
    public class CountryRepository : ICountryRepository
    {
        IDbConnectionManager _connectionManager;
        public CountryRepository(IDbConnectionManager dbConnectionManager)
        {
            _connectionManager = dbConnectionManager;
        }

        public async Task<List<Country>> GetCountryPopulationAsync()
        {
            using var _connection =  await _connectionManager.GetConnection();

            if (_connection == null)
                return new List<Country>();

            var rows = await _connection.QueryAsync<Country>(GetCountryPopulationSqlQuery);
            return rows.ToList();
        }


        private string GetCountryPopulationSqlQuery = @"
            SELECT co.CountryName AS Name, CAST(SUM(ci.Population) AS INT) AS Population 
                        FROM City ci 
                        JOIN State st ON ci.StateId = st.StateId 
                        JOIN Country co ON co.CountryId = st.CountryId 
                        GROUP BY co.CountryName;";
    }
}
