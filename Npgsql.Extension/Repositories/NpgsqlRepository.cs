using Npgsql.Extension.Options;
using Npgsql.Extension.Reader;

namespace Npgsql.Extension.Repositories;

public abstract class NpgsqlRepository(IDatabaseOptions databaseOptions)
{
    private string ConnectionString =>
        databaseOptions.ConnectionString;

    private NpgsqlConnection GetConnection() =>
        new NpgsqlConnection(ConnectionString);

    protected async Task<T?> GetAsync<T>(string query, NpgsqlParameter[] parameters) where T : new()
    {
        var connection = GetConnection();

        try
        {
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddRange(parameters);

            await using var reader = await cmd.ExecuteReaderAsync();

            return await Reader<T>.ReadAsync(reader);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<T?> GetAsync<T>(string query) where T : new()
    {
        var connection = GetConnection();

        try
        {
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, connection);

            await using var reader = await cmd.ExecuteReaderAsync();

            return await Reader<T>.ReadAsync(reader);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<IEnumerable<T>> GetListAsync<T>(string query, NpgsqlParameter[] parameters) where T : new()
    {
        var connection = GetConnection();

        try
        {
            connection.Open();

            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddRange(parameters);

            var reader = await cmd.ExecuteReaderAsync();

            return await Reader<T>.ReadListAsync(reader);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<IEnumerable<T>> GetListAsync<T>(string query) where T : new()
    {
        var connection = GetConnection();

        try
        {
            connection.Open();

            await using var cmd = new NpgsqlCommand(query, connection);

            var reader = await cmd.ExecuteReaderAsync();

            return await Reader<T>.ReadListAsync(reader);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<Boolean> ExecuteAsync(string query, NpgsqlParameter[] parameters)
    {
        var connection = GetConnection();

        try
        {
            connection.Open();

            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddRange(parameters);

            // returns executed or not
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<Boolean> DeleteAsync(string table, string column, object param)
    {
        var connection = GetConnection();

        try
        {
            connection.Open();

            var query = $"delete from {table} where {column} = $1";

            await using var cmd = new NpgsqlCommand(query, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter {Value = param}
                }
            };

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<Boolean> DeleteCascadeAsync(string table, string column, object param)
    {
        var connection = GetConnection();

        try
        {
            connection.Open();

            var query = $"delete from {table} where {column} = $1 cascade";

            await using var cmd = new NpgsqlCommand(query, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter {Value = param}
                }
            };

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}