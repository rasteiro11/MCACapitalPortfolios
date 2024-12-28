namespace MCACapitalPortfolios.Contracts.Portfolio;

public class EvaluatePortfolioAtRequest {
    public DateTime DtReference { get; set; }
    public int PortfolioId { get; set; }
    public bool? Store { get; set; } = false;
}