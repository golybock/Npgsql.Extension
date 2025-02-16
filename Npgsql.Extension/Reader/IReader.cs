using System.Data.Common;

namespace Npgsql.Extension.Reader;

public interface IReader<T>
{
    public static abstract Task<T?> ReadAsync(DbDataReader reader);

    public static abstract Task<IEnumerable<T>> ReadListAsync(DbDataReader reader);
}