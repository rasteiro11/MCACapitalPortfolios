using Entities;
using MCACapitalPortfolios.Application.Abstractions.Repository;
using MCACapitalPortfolios.Contracts.Portfolio;
using MCACapitalPortfolios.Domain.Entities;

namespace MCACapitalPortfolios.Application.Abstractions.Services;

public interface IPortfoliosService {
    Task<Portfolio?> CreatePortfolioAsync(string description, params Position[] positions);
    Task DeletePortfolioByIdAsync(int portfolioId);
    Task<decimal> EvaluatePortfolioAt(DateTime dtReference, int portfolioId, bool? storeResult = false);
    Task<IPaginatedResult<Portfolio>> GetPaginatedPortfolios(int page, int pageSize);
    Task<Portfolio?> GetPortfolioByIdAsync(int portfolioId);
    Task<IEnumerable<EvaluatePortfolioAtInRangeResponseItem>> EvaluatePortfolioAtInRange(DateTime dtReferenceStart, DateTime dtReferenceEnd, int portfolioId, bool? storeResult = false);
    Task<IEnumerable<PortfolioHistory>> GetPortfolioHistoryInRange(int portfolioId, DateTime? dtStart = null, DateTime? dtEnd = null);
}