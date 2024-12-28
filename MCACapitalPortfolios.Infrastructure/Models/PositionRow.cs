using MCACapitalPortfolios.Domain.Enums;

namespace MCACapitalPortfolios.Infrastructure.Models;

public class PositionRow {
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public int AssetId { get; set; }
    public PositionType PositionType { get; set; }
    public DateTime DtTimestamp { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; } 
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}