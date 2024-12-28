using System.Text.Json;
using Flurl.Http;
using Flurl;
using MCACapitalPortfolios.Application.Services;
using MCACapitalPortfolios.Contracts.Series;
using Microsoft.Extensions.Configuration;
using MCACapitalPortfolios.Contracts.Calendar;

namespace MCACapitalPortfolios.Infrastructure.Services;
public class CalendarService : ICalendarService
{
    private readonly string _baseUrl;

    public CalendarService(IConfiguration configuration)
    {
        _baseUrl = configuration["CalendarServiceBaseEndpoint"];
    }

    public async Task<ProxDUBatchResponse> ProxDUBatch(ProxDUBatchRequest proxDUBatchRequest)
    {
        Console.WriteLine(JsonSerializer.Serialize(proxDUBatchRequest));

        try
        {
            var result = await(_baseUrl)
                .AppendPathSegment("/api/Calendar/ProxDUBatch")
                .PostJsonAsync(proxDUBatchRequest)
                .ReceiveJson<ProxDUBatchResponse>();
            Console.WriteLine(result);

            return result;
        }
        catch (FlurlHttpException ex)
        {
            var errorMessage = await ex.GetResponseStringAsync();
            throw new ApplicationException($"Request failed: {errorMessage}", ex);
        }
    }
}
