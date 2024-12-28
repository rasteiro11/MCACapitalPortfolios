using System.Net.Http.Headers;
using Entities;
using MCACapitalPortfolios.Application.Abstractions.Database;
using MCACapitalPortfolios.Application.Abstractions.Repository;
using MCACapitalPortfolios.Domain.Entities;
using MCACapitalPortfolios.Infrastructure.Database;
using MCACapitalPortfolios.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCACapitalPortfolios.Infrastructure.Repositories;
public class PortfolioRepository : MySqlHelper, ISqlHelper, IPortfolioRepository
{
    private readonly ILogger<PortfolioRepository> logger;
    public PortfolioRepository(IConfiguration config, ILogger<PortfolioRepository> logger) : base(config)
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

    public async Task DeletePortfolioByIdAsync(int portfolioId)
    {
        var query = $"DELETE FROM Portfolio WHERE Id = {portfolioId}";

        logger.LogInformation(query);

        await Execute(query);
    }

    public async Task<IPaginatedResult<Portfolio>> GetPaginatedPortfolios(int page, int pageSize)
    {
        var query = @$"
            WITH PaginatedPortfolios AS (
                SELECT p.Id
                    FROM Portfolio p
                    ORDER BY p.Id
                    LIMIT {pageSize} OFFSET {(page-1)*pageSize}
            )
            SELECT *
                FROM Portfolio p
                INNER JOIN Position pos ON pos.PortfolioId = p.Id
                INNER JOIN Series s ON pos.AssetId = s.Id
                WHERE p.Id IN (SELECT Id FROM PaginatedPortfolios)
            ORDER BY p.Id, pos.Id, s.Id;   
        ";

        var positions = await Query<PositionRow>(query);
        var portfolios = positions.GroupBy(row => row.PortfolioId).Select(portfolioPositions => {
            var first = portfolioPositions.FirstOrDefault();
            if(first is null) return null;

            return new Portfolio() {
                Id=first.PortfolioId,
                Description=first.Description,
                Positions=portfolioPositions.Select(pos => new Position{
                        AssetId=pos.AssetId,
                        DtTimestamp=pos.DtTimestamp,
                        PositionType=pos.PositionType,
                        Price=pos.Price,
                        Quantity=pos.Quantity,
                        PortfolioId=pos.PortfolioId,
                        Id=pos.Id,
                        Name=pos.Name
                })
            };
        });

        return new GetPaginatedPortfoliosResult(){
            Items=portfolios.ToList(),
            PageIndex=page,
            PageSize=pageSize,
            TotalCount=(await Query<int>("SELECT COUNT(*) FROM Portfolio;")).First()
        };
    }

    public async Task<Portfolio?> GetPortfolioByIdAsync(int portfolioId)
    {
        var query = $@"
            SELECT * FROM Portfolio p
            	INNER JOIN Position pos ON pos.PortfolioId = p.Id
            	INNER JOIN Series s on pos.AssetId = s.Id
            	WHERE p.Id = {portfolioId}";


        var positions = await Query<PositionRow>(query);
        var first = positions.FirstOrDefault();
        if(first is null) return null;

        return new Portfolio() {
            Id=first.PortfolioId,
            Description=first.Description,
            Positions=positions.Select(pos => new Position{
                    AssetId=pos.AssetId,
                    DtTimestamp=pos.DtTimestamp,
                    PositionType=pos.PositionType,
                    Price=pos.Price,
                    Quantity=pos.Quantity,
                    PortfolioId=pos.PortfolioId,
                    Id=pos.Id,
                    Name=pos.Name
            })
        };
    }
}
