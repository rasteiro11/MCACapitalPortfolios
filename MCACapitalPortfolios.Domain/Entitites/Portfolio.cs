using MCACapitalPortfolios.Domain.Entities;

namespace Entities;

public class Portfolio {
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public IEnumerable<Position>? Positions { get; set; }
}