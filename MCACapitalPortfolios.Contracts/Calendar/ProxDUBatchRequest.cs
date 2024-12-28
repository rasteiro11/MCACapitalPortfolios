namespace MCACapitalPortfolios.Contracts.Calendar;

public class ProxDUBatchRequest {
    public string Country { get; set; }
    public IEnumerable<ProxDURequest> Requests { get; set; }
}

public class ProxDURequest {
    public DateTime DtReference { get; set; }
    public int Offset { get; set; }
}