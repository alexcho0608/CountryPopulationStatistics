
using Application;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.External;

public class ExternaAPIStatService : IStatService
{
    private readonly ILogger<ExternaAPIStatService> _logger;

    public ExternaAPIStatService(ILogger<ExternaAPIStatService> logger)
    {
        _logger = logger;
    }

    public List<Country> GetCountryPopulations()
    {
        Tuple<string, int>[]  data = [
                Tuple.Create("India",1182105000),
                Tuple.Create("United Kingdom",62026962),
                Tuple.Create("Chile",17094270),
                Tuple.Create("Mali",15370000),
                Tuple.Create("Greece",11305118),
                Tuple.Create("Armenia",3249482),
                Tuple.Create("Slovenia",2046976),
                Tuple.Create("Saint Vincent and the Grenadines",109284),
                Tuple.Create("Bhutan",695822),
                Tuple.Create("Aruba (Netherlands)",101484),
                Tuple.Create("Maldives",319738),
                Tuple.Create("Mayotte (France)",202000),
                Tuple.Create("Vietnam",86932500),
                Tuple.Create("Germany",81802257),
                Tuple.Create("Botswana",2029307),
                Tuple.Create("Togo",6191155),
                Tuple.Create("Luxembourg",502066),
                Tuple.Create("U.S. Virgin Islands (US)",106267),
                Tuple.Create("Belarus",9480178),
                Tuple.Create("Myanmar",59780000),
                Tuple.Create("Mauritania",3217383),
                Tuple.Create("Malaysia",28334135),
                Tuple.Create("Dominican Republic",9884371),
                Tuple.Create("New Caledonia (France)",248000),
                Tuple.Create("Slovakia",5424925),
                Tuple.Create("Kyrgyzstan",5418300),
                Tuple.Create("Lithuania",3329039),
                Tuple.Create("United States of America",309349689)
        ];
        var result = data
                .Select(t => new Country { Name = t.Item1, Population = t.Item2 })
                .ToList();
        _logger?.LogInformation("Loaded {Count} countries from external API", result?.Count ?? 0);
        return result;
    }

    public Task<List<Country>> GetCountryPopulationsAsync()
    {
        return Task.FromResult(GetCountryPopulations());
    }
}
