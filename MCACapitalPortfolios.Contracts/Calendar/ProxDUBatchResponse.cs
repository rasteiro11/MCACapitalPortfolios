namespace MCACapitalPortfolios.Contracts.Calendar;

public class ProxDUBatchResponse {
    public string Country { get; set; }
    public IEnumerable<ProxDUResponse> Outputs  { get; set; }
}

public class ProxDUResponse {
    public DateTime DtReference { get; set; }
    public int Offset { get; set; }
    public DateTime ModifiedDtReference { get; set; }
}