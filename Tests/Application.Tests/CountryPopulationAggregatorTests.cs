using Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Services.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class CountryPopulationAggregatorTests
    {
        private readonly Mock<IStatService> _dbMock;
        private readonly Mock<IStatService> _extMock;
        private readonly NullLogger<CountryPopulationAggregator> _logger;

        public CountryPopulationAggregatorTests()
        {
            _dbMock = new Mock<IStatService>();
            _extMock = new Mock<IStatService>();
            _logger = new NullLogger<CountryPopulationAggregator>();
        }

        [Fact]
        public async Task GetCombinedAsync_PrefersDbOverExternalAPI_OnDuplicate()
        {
            var dbData = new List<Country>
            {
                new Country { Name = "USA", Population = 1000 },
                new Country { Name = "UK", Population = 200 }
            };

            var externalData = new List<Country>
            {
                new Country { Name = "USA", Population = 50 },
                new Country { Name = "Bulgaria", Population = 300 }
            };

            _dbMock.Setup(x => x.GetCountryPopulationsAsync()).ReturnsAsync(dbData);

            _extMock.Setup(x => x.GetCountryPopulationsAsync()).ReturnsAsync(externalData);

            var aggregator = new CountryPopulationAggregator(new List<IStatService> { _dbMock.Object, _extMock.Object }, _logger);

            var result = (await aggregator.GetCombinedAsync()).ToList();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.Name == "USA" && r.Population == dbData[0].Population);
            Assert.Contains(result, r => r.Name == "UK" && r.Population == dbData[1].Population);
            Assert.Contains(result, r => r.Name == "Bulgaria" && r.Population == externalData[1].Population);
        }

        [Fact]
        public async Task GetCombinedAsync_HandlesNullsGracefully()
        {
            _dbMock.Setup(x => x.GetCountryPopulationsAsync()).ReturnsAsync((List<Country>)null);

            _extMock.Setup(x => x.GetCountryPopulationsAsync()).ReturnsAsync((List<Country>)null);

            var aggregator = new CountryPopulationAggregator(new List<IStatService> { _dbMock.Object, _extMock.Object }, _logger);

            var result = (await aggregator.GetCombinedAsync()).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCombinedAsync_DbServiceThrowsException_UsesExternalAPIData()
        {
            _dbMock.Setup(x => x.GetCountryPopulationsAsync()).ThrowsAsync(new System.Exception("DB failure"));

            var externalData = new List<Country>
            {
                new Country { Name = "USA", Population = 10 },
                new Country { Name = "UK", Population = 20 }
            };
            _extMock.Setup(x => x.GetCountryPopulationsAsync()).ReturnsAsync(externalData);

            var aggregator = new CountryPopulationAggregator(new List<IStatService> { _dbMock.Object, _extMock.Object }, _logger);

            var result = (await aggregator.GetCombinedAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Name == "USA" && r.Population == externalData[0].Population);
            Assert.Contains(result, r => r.Name == "UK" && r.Population == externalData[1].Population);
        }
    }
}
