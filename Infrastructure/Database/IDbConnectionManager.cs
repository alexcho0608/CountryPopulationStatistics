using System.Data.Common;

namespace Infrastructure.Database;

public interface IDbConnectionManager
{
    Task<DbConnection> GetConnection();
}
