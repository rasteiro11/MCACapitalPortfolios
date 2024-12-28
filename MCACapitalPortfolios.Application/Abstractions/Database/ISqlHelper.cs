namespace MCACapitalPortfolios.Application.Abstractions.Database;

public interface ISqlHelper {
    public Task Execute(string query);
    public Task<T?> ExecuteScalar<T>(string query);
    public Task<IEnumerable<T>> Query<T>(string query);
}