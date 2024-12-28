using MCACapitalPortfolios.Application.Abstractions.Database;
using MCACapitalPortfolios.Application.Abstractions.Repository;
using MCACapitalPortfolios.Domain.Entities;
using MCACapitalPortfolios.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MCACapitalPortfolios.Infrastructure.Repositories;
public class PortfolioHistoryRepository : MySqlHelper, ISqlHelper, IPortfolioHistoryRepository
{
    private readonly ILogger<PortfolioRepository> logger;
    public PortfolioHistoryRepository(IConfiguration config, ILogger<PortfolioRepository> logger) : base(config)
    {
        this.logger = logger;
    }

    public async Task<int> CreatePortfolioAsync(string description, params Position[] positions)
    {
        var query = @$"
            INSERT INTO Portfolio (Description) 
            VALUES ('{description}');
            SELECT LAST_INSERT_ID();";  

        logger.LogInformation(query);

        var id = await ExecuteScalar<int>(query);

        foreach (var item in positions) item.PortfolioId = id;

        if (positions.Any())
        {
            var values = string.Join(", ", positions.Select(p => @$"(
                {p.PortfolioId}, 
                {p.AssetId}, 
                '{p.PositionType}', 
                '{p.DtTimestamp:yyyy-MM-dd HH:mm:ss}', 
                {p.Price}, 
                {p.Quantity})"));

            var positionsQuery = @$"
                INSERT INTO Position (PortfolioId, AssetId, PositionType, DtTimestamp, Price, Quantity)
                VALUES {values};";

            logger.LogInformation(positionsQuery);

            await Execute(positionsQuery);
        }

        return id;
    }

    public async Task CreatePortfolioHistoryAsync(IEnumerable<PortfolioHistory> portfolioHistories)
    {
        if(portfolioHistories.Any()) {
            var values = string.Join(", ", portfolioHistories.Select(p => @$"(
                {p.PortfolioId}, 
                '{p.DtReference:yyyy-MM-dd}', 
                '{p.DtTimestamp:yyyy-MM-dd HH:mm:ss}', 
                {p.Value})"));

            var positionsQuery = @$"
                INSERT INTO PortfolioHistory (PortfolioId, DtReference, DtTimestamp, Value)
                VALUES {values} 
                ON DUPLICATE KEY UPDATE
                    Value = VALUES(Value);";

            logger.LogInformation(positionsQuery);

            await Execute(positionsQuery);
        }
    }

    public async Task<IEnumerable<PortfolioHistory>> GetPortfolioHistoryInRange(int portfolioId, DateTime? dtStart = null, DateTime? dtEnd = null)
    {
        var query = $@"
            SELECT * FROM PortfolioHistory ph
	            WHERE {((dtStart is not null && dtEnd is not null) ? $"ph.DtTimestamp BETWEEN '{dtStart:yyyy-MM-dd}' AND '{dtEnd:yyyy-MM-dd}' AND" : "")}
	            ph.PortfolioId = {portfolioId};
        ";

        logger.LogInformation(query);

        return await Query<PortfolioHistory>(query);
    }
}
