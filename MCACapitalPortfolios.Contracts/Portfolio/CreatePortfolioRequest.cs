using MCACapitalPortfolios.Domain.Enums;

namespace MCACapitalPortfolios.Contracts.Portfolio;

public class CreatePortfolioRequest {
    public string Description { get; set; } = string.Empty;
    public IEnumerable<CreatePortfolioRequestPosition>? Positions { get; set; }
}

public class CreatePortfolioRequestPosition {
    public int AssetId { get; set; }
    public PositionType PositionType { get; set; }
    public DateTime DtTimestamp { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}