namespace MCACapitalPortfolios.Contracts.Series;

public class SeriesHistory
{
    public int Id { get; set; }
    public int SerieId { get; set; }
    public DateTime DtReference { get; set; }
    public DateTime DtTimestamp { get; set; }
    public decimal Value { get; set; }
}

public class SeriesResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Provider { get; set; }
    public string PriceType { get; set; }
    public List<SeriesHistory> SerieHistory { get; set; }
}