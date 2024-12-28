using MCACapitalPortfolios.Contracts.Calendar;
using MCACapitalPortfolios.Contracts.Series;

namespace MCACapitalPortfolios.Application.Services;

public interface ICalendarService
{
    Task<ProxDUBatchResponse> ProxDUBatch(ProxDUBatchRequest proxDUBatchRequest);
}