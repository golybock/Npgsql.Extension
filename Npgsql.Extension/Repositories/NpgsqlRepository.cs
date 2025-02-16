using Npgsql.Extension.DataMapper;
using Npgsql.Extension.Options;
using Npgsql.Extension.Reader;

namespace Npgsql.Extension.Repositories;

public abstract class NpgsqlRepository
{
	private readonly IDataMapper _dataMapper;
	private readonly IDatabaseOptions _databaseOptions;

	private NpgsqlConnection GetConnection()
	{
		return new NpgsqlConnection(_databaseOptions.ConnectionString);
	}

	protected NpgsqlRepository(IDatabaseOptions databaseOptions, IDataMapper dataMapper)
	{
		_databaseOptions = databaseOptions;
		_dataMapper = dataMapper;
	}

	/// <summary>
	/// Получает один объект типа T, используя запрос с параметрами.
	/// </summary>
	protected async Task<T?> GetAsync<T>(string query, NpgsqlParameter[]? parameters = null) where T : new()
	{
		NpgsqlConnection connection = GetConnection();

		try
		{
			await connection.OpenAsync();

			await using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

			if (parameters != null) cmd.Parameters.AddRange(parameters);

			await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
			return await _dataMapper.MapAsync<T>(reader);
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

	/// <summary>
	/// Получает список объектов типа T, используя запрос с параметрами.
	/// </summary>
	protected async Task<IEnumerable<T>> GetListAsync<T>(string query, NpgsqlParameter[]? parameters = null) where T : new()
	{
		NpgsqlConnection connection = GetConnection();

		try
		{
			await connection.OpenAsync();

			await using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

			if (parameters != null) cmd.Parameters.AddRange(parameters);

			NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

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

	/// <summary>
	/// Выполняет команду (например, INSERT, UPDATE) с параметрами и возвращает результат выполнения.
	/// </summary>
	protected async Task<Boolean> ExecuteAsync(string query, NpgsqlParameter[] parameters)
	{
		NpgsqlConnection connection = GetConnection();

		try
		{
			await connection.OpenAsync();

			await using NpgsqlCommand cmd = new NpgsqlCommand(query, connection);

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

	/// <summary>
	/// Удаляет запись из указанной таблицы по условию.
	/// </summary>
	protected async Task<Boolean> DeleteAsync(string table, string column, object param)
	{
		NpgsqlConnection connection = GetConnection();

		try
		{
			await connection.OpenAsync();

			string query = $"delete from {table} where {column} = $1";

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

	/// <summary>
	/// Удаляет запись с каскадным удалением связанных записей.
	/// </summary>
	protected async Task<Boolean> DeleteCascadeAsync(string table, string column, object param)
	{
		NpgsqlConnection connection = GetConnection();

		try
		{
			await connection.OpenAsync();

			string query = $"delete from {table} where {column} = $1 cascade";

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

	/// <summary>
	/// Выполняет переданный делегат в контексте транзакции.
	/// В случае ошибки транзакция откатывается.
	/// Пример использования:
	/// await ExecuteTransactionAsync(async (tx) =>
	/// {
	///     // создать и выполнить команды, не забывая устанавливать
	///     // свойство Transaction у команд: cmd.Transaction = tx;
	/// });
	/// </summary>
	protected async Task ExecuteTransactionAsync(Func<NpgsqlTransaction, Task> transactionalWork)
	{
		NpgsqlConnection connection = GetConnection();

		await connection.OpenAsync();

		await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

		try
		{
			await transactionalWork(transaction);
			await transaction.CommitAsync();
		}
		catch (Exception)
		{
			await transaction.RollbackAsync();
			throw;
		}
		finally
		{
			await connection.CloseAsync();
		}
	}
}