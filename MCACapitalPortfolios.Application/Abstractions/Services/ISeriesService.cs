using MCACapitalPortfolios.Contracts.Series;

namespace MCACapitalPortfolios.Application.Services;

public interface ISeriesService
{
    Task<Dictionary<string, SeriesResponse>> GetManySeriesInRangeAsync(IEnumerable<SeriesRequest> series, DateTime startDate, DateTime endDate);
}