namespace MCACapitalPortfolios.Domain.Entities;

public class PortfolioHistory {
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public DateTime DtReference { get; set; }
    public DateTime DtTimestamp { get; set; }
    public decimal Value { get; set; }
}