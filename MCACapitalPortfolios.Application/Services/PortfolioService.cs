using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json;
using Entities;
using MCACapitalPortfolios.Application.Abstractions.Repository;
using MCACapitalPortfolios.Application.Abstractions.Services;
using MCACapitalPortfolios.Contracts.Portfolio;
using MCACapitalPortfolios.Contracts.Series;
using MCACapitalPortfolios.Domain.Entities;
using MCACapitalPortfolios.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace MCACapitalPortfolios.Application.Services;

public class PortfolioService : IPortfoliosService
{
    private readonly IPortfolioRepository portfolioRepository;
    private readonly IPortfolioHistoryRepository portfolioHistoryRepository;
    private readonly PriceType priceType = PriceType.CLOSE;
    private readonly Provider provider = Provider.ALPHA_ADVANTAGE;
    private readonly ISeriesService seriesService;
    private  readonly ICalendarService calendarService;
    private readonly ILogger<PortfolioService> logger;

    public PortfolioService(IPortfolioRepository portfolioRepository, ISeriesService seriesService, IPortfolioHistoryRepository portfolioHistoryRepository, ICalendarService calendarService, ILogger<PortfolioService> logger)
    {
        this.portfolioRepository = portfolioRepository;
        this.seriesService = seriesService;
        this.portfolioHistoryRepository = portfolioHistoryRepository;
        this.calendarService = calendarService;
        this.logger = logger;
    }

    public async Task<Portfolio?> CreatePortfolioAsync(string description, params Position[] positions)
    {
        int portfolioId = await portfolioRepository.CreatePortfolioAsync(description, positions);

        var createdPortfolio = await portfolioRepository.GetPortfolioByIdAsync(portfolioId);

        var firstPositionDate = positions.OrderBy(pos => pos.DtTimestamp).First().DtTimestamp;

        await EvaluatePortfolioAtInRange(firstPositionDate, DateTime.Now, createdPortfolio.Id, true);
        
        return createdPortfolio;
    }

    public async Task DeletePortfolioByIdAsync(int portfolioId)
    {
        await portfolioRepository.DeletePortfolioByIdAsync(portfolioId);
    }

    public async Task<Portfolio?> GetPortfolioByIdAsync(int portfolioId)
    {
        return await portfolioRepository.GetPortfolioByIdAsync(portfolioId);
    }

    public async Task<decimal> EvaluatePortfolioAt(DateTime dtReference, int portfolioId, bool? storeResult = false)
    {
        var portfolio = await portfolioRepository.GetPortfolioByIdAsync(portfolioId);
        if(portfolio is null) {
            throw new Exception($"Portfolio with id {portfolioId} does not exists");
        }


        var seriesUsed = portfolio.Positions.Select(pos => pos.Name);
        var seriesResponse = await  seriesService.GetManySeriesInRangeAsync(seriesUsed.Select(s => new Contracts.Series.SeriesRequest() {
            name=s,
            priceType=priceType.ToString(),
            provider=provider.ToString()
        }), dtReference, dtReference);

        Console.WriteLine(JsonSerializer.Serialize(seriesResponse, new JsonSerializerOptions() {
            WriteIndented=true
        }));

        decimal finalValue = 0.0M;

        foreach (var position in portfolio.Positions)
        {
            var val = seriesResponse[$"{position.Name}.{provider}.{priceType}"].SerieHistory.FirstOrDefault();
            if(val is null) {
                throw new ArgumentNullException($"Value for serie {position.Name} and provider {provider} and {priceType} at date {dtReference:yyyy-MM-dd} does not exists");
            }

            finalValue += position.EvaluateAt(dtReference, val.Value);
        } 

        if(storeResult.HasValue && storeResult.Value) {
            await portfolioHistoryRepository.CreatePortfolioHistoryAsync(new List<PortfolioHistory>() {
                new PortfolioHistory() {
                    DtReference=dtReference,
                    DtTimestamp=dtReference,
                    PortfolioId=portfolio.Id,
                    Value=finalValue
                }
            });
        }

        return finalValue;
    }

    public async Task<IPaginatedResult<Portfolio>> GetPaginatedPortfolios(int page, int pageSize)
    {
        return await portfolioRepository.GetPaginatedPortfolios(page, pageSize);
    }
    public async Task<IEnumerable<EvaluatePortfolioAtInRangeResponseItem>> EvaluatePortfolioAtInRange(DateTime dtReferenceStart, DateTime dtReferenceEnd, int portfolioId, bool? storeResult = false)
    {
        var response = new List<EvaluatePortfolioAtInRangeResponseItem>();
        var portfolio = await portfolioRepository.GetPortfolioByIdAsync(portfolioId);
        if(portfolio is null) {
            throw new Exception($"Portfolio with id {portfolioId} does not exists");
        }


        var seriesUsed = portfolio.Positions.Select(pos => pos.Name);
        var seriesResponse = await  seriesService.GetManySeriesInRangeAsync(seriesUsed.Select(s => new Contracts.Series.SeriesRequest() {
            name=s,
            priceType=priceType.ToString(),
            provider=provider.ToString()
        }), dtReferenceStart, dtReferenceEnd);

        Console.WriteLine(JsonSerializer.Serialize(seriesResponse, new JsonSerializerOptions() {
            WriteIndented=true
        }));

        var dateRange = await GetDUsAtRange(dtReferenceStart, dtReferenceEnd);

        Console.WriteLine(JsonSerializer.Serialize(dateRange, new JsonSerializerOptions() {
            WriteIndented=true
        }));

        foreach (var d in dateRange)
        {
            try {
                var finalValue = EvaluatePortfolioPositionsForDate(d, portfolio.Positions, seriesResponse);
                if(storeResult.HasValue && storeResult.Value) {
                    await portfolioHistoryRepository.CreatePortfolioHistoryAsync(new List<PortfolioHistory>() {
                        new PortfolioHistory() {
                            DtReference=d,
                            DtTimestamp=d,
                            PortfolioId=portfolio.Id,
                            Value=finalValue
                        }
                    });
                }
                response.Add(new EvaluatePortfolioAtInRangeResponseItem() {
                    DtReference=d,
                    Value=finalValue
                });
            } catch(Exception e) {
                logger.LogError(e.Message);
                continue;
            }
        }


        return response;
    }

    private decimal EvaluatePortfolioPositionsForDate(DateTime dtReference, IEnumerable<Position> positions, IDictionary<string, SeriesResponse> series) {
        decimal finalValue = 0.0M;

        foreach (var position in positions)
        {
            var val = series[$"{position.Name}.{provider}.{priceType}"].SerieHistory
                            .Where(o => o.DtReference == dtReference).FirstOrDefault();
            if(val is null) {
                throw new ArgumentNullException($"Value for serie {position.Name} and provider {provider} and {priceType} at date {dtReference:yyyy-MM-dd} does not exists");
            }

            finalValue += position.EvaluateAt(dtReference, val.Value);
        } 

        return finalValue;
    }

    private async Task<IEnumerable<DateTime>> GetDUsAtRange(DateTime dtStart, DateTime dtEnd) {
        IList<DateTime> datesInRange = new List<DateTime>();
        for (DateTime i = dtStart; i <= dtEnd; i = i.AddDays(1))
        {
            datesInRange.Add(i);
        }

        return (await calendarService.ProxDUBatch(new Contracts.Calendar.ProxDUBatchRequest() {
            Country="BR",
            Requests= datesInRange.Select(date => new Contracts.Calendar.ProxDURequest() {
                Offset=0,
                DtReference=date
            })
        })).Outputs.Select(date => date.ModifiedDtReference);
    }

    public async Task<IEnumerable<PortfolioHistory>> GetPortfolioHistoryInRange(int portfolioId, DateTime? dtStart = null, DateTime? dtEnd = null) {
        return await portfolioHistoryRepository.GetPortfolioHistoryInRange(portfolioId, dtStart, dtEnd);
    }
}