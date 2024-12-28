using System.Configuration.Internal;
using System.Text.Json;
using Flurl;
using Flurl.Http;
using MCACapitalPortfolios.Application.Services;
using MCACapitalPortfolios.Contracts.Series;
using Microsoft.Extensions.Configuration;

namespace MCACapitalPortfolios.Infrastructure.Services;
public class SeriesService : ISeriesService
{
    private readonly string _baseUrl;

    public SeriesService(IConfiguration configuration)
    {
        _baseUrl = configuration["SeriesServiceBaseEndpoint"];
    }

    public async Task<Dictionary<string, SeriesResponse>> GetManySeriesInRangeAsync(IEnumerable<SeriesRequest> series, DateTime startDate, DateTime endDate)
    {
        if (series == null || !series.Any())
            throw new ArgumentException("Series cannot be null or empty.", nameof(series));

        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date.", nameof(startDate));

        Console.WriteLine($"SERIES: {string.Join(",", series)}");
        var payload = new
        {
            Series = series,
            StartDate = startDate.ToString("yyyy-MM-dd"),
            EndDate = endDate.ToString("yyyy-MM-dd")
        };

        Console.WriteLine(JsonSerializer.Serialize(payload));

        try
        {
            var result = await(_baseUrl)
                .AppendPathSegment("/api/Series/GetManySeriesInRange")
                .PostJsonAsync(payload)
                .ReceiveJson<Dictionary<string, SeriesResponse>>();
            Console.WriteLine(result);

            return result ?? new Dictionary<string, SeriesResponse>();
        }
        catch (FlurlHttpException ex)
        {
            var errorMessage = await ex.GetResponseStringAsync();
            throw new ApplicationException($"Request failed: {errorMessage}", ex);
        }
    }
}
