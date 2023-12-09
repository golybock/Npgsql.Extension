namespace Npgsql.Extension.Reader;

public interface IReader<T>
{
    /// <summary>
    /// T - Database Data class, using auto mapping
    /// Now only UpperCamelCase (data class) to snake_case (database)
    /// </summary>
    /// <param name="reader">reader returned when used command.ExecuteReaderAsync</param>
    /// <returns></returns>
    public static abstract Task<T?> ReadAsync(NpgsqlDataReader reader);

    public static abstract Task<IEnumerable<T>> ReadListAsync(NpgsqlDataReader reader);
}