using Entities;
using MCACapitalPortfolios.Application.Abstractions.Repository;

namespace MCACapitalPortfolios.Infrastructure.Models;

public class GetPaginatedPortfoliosResult : IPaginatedResult<Portfolio>
{
    public List<Portfolio> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}