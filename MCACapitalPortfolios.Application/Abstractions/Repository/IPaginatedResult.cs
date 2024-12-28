namespace MCACapitalPortfolios.Application.Abstractions.Repository;

public interface IPaginatedResult<T> {
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}