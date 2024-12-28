using Dapper;
using MCACapitalPortfolios.Application.Abstractions.Database;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MCACapitalPortfolios.Infrastructure.Database;

public class MySqlHelper : ISqlHelper
{
    private readonly string connString;
    public MySqlHelper(IConfiguration config)
    {
        connString = $@"
            Data Source={config["MySql:DataSource"]};
            port={config["MySql:Port"]};
            Initial Catalog={config["MySql:InitialCatalog"]};
            User ID={config["MySql:UserID"]};
            Password={config["MySql:Password"]};
            MaximumPoolSize=5;Pooling=true";
    }

    private MySqlConnection Connection() {
        return new MySqlConnection(connString);
    }
    
    public async Task Execute(string query)
    {
        using(var connection = Connection()) {
            connection.Open();
            try {
                await connection.ExecuteAsync(query);
            } catch(Exception e) {
                throw;
            }
        }
    }

    public async Task<T?> ExecuteScalar<T>(string query)
    {
        using(var connection = Connection()) {
            connection.Open();
            try {
                return await connection.ExecuteScalarAsync<T>(query);
            } catch(Exception e) {
                throw;
            }
        }
    }

    public async Task<IEnumerable<T>> Query<T>(string query)
    {
        using(var connection = Connection()) {
            connection.Open();
            try {
                return await connection.QueryAsync<T>(query);
            } catch(Exception e) {
                throw;
            }
        }
    }
}
