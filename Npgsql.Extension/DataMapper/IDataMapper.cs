namespace Npgsql.Extension.DataMapper;

public interface IDataMapper
{
	T? Map<T>(NpgsqlDataReader reader) where T : new();

	IEnumerable<T> MapList<T>(NpgsqlDataReader reader) where T : new();

	Task<T?> MapAsync<T>(NpgsqlDataReader reader) where T : new();

	Task<IEnumerable<T>> MapListAsync<T>(NpgsqlDataReader reader) where T : new();

	protected T MapObject<T>(NpgsqlDataReader reader) where T : new();
}