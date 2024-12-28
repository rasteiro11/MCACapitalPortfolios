namespace MCACapitalPortfolios.Contracts.Portfolio;

public class EvaluatePortfolioAtRangeRequest {
    public DateTime DtStart { get; set; }
    public DateTime DtEnd { get; set; }
    public int PortfolioId { get; set; }
    public bool? Store { get; set; } = false;
}