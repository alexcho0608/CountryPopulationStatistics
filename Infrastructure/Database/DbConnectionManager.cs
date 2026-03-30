using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Infrastructure.Database;

public class DbConnectionManager : IDbConnectionManager
{
    private readonly string _connectionString;
    public DbConnectionManager(string connectionString)
    {
        _connectionString = connectionString; 
    }

    public async Task<DbConnection> GetConnection()
    {
        try
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        catch(SqliteException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
