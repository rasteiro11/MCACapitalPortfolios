using Entities;
using MCACapitalPortfolios.Domain.Entities;

namespace MCACapitalPortfolios.Application.Abstractions.Repository;

public interface IPortfolioRepository {
    Task<int> CreatePortfolioAsync(string description, params Position[] positions);
    Task DeletePortfolioByIdAsync(int portfolioId);
    Task<IPaginatedResult<Portfolio>> GetPaginatedPortfolios(int page, int pageSize);
    Task<Portfolio?> GetPortfolioByIdAsync(int porfolioId);
}   