using System.Data.Common;

namespace Gaia.Models;

public readonly struct SqlRawParameters
{
    public SqlRawParameters(string sql, ReadOnlyMemory<DbParameter> parameters)
    {
        Sql = sql;
        Parameters = parameters.ToArray();
    }

    public string Sql { get; }
    public ReadOnlyMemory<object> Parameters { get; }
}