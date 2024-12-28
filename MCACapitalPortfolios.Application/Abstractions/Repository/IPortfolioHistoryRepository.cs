using Entities;
using MCACapitalPortfolios.Domain.Entities;

namespace MCACapitalPortfolios.Application.Abstractions.Repository;

public interface IPortfolioHistoryRepository {
    Task CreatePortfolioHistoryAsync(IEnumerable<PortfolioHistory> portfolioHistories);
    Task<IEnumerable<PortfolioHistory>> GetPortfolioHistoryInRange(int portfolioId, DateTime? dtStart = null, DateTime? dtEnd = null);
}   