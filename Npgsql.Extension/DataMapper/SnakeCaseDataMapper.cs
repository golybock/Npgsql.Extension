﻿using System.Reflection;
using Npgsql.Extension.Tools;

namespace Npgsql.Extension.DataMapper;

public class SnakeCaseDataMapper : IDataMapper
{
	public T? Map<T>(NpgsqlDataReader reader) where T : new()
	{
		if (reader.Read())
		{
			return Map<T>(reader);
		}

		return default;
	}

	public IEnumerable<T> MapList<T>(NpgsqlDataReader reader) where T : new()
	{
		List<T> list = new();

		while (reader.Read())
		{
			T? item = Map<T>(reader);

			if (item != null)
			{
				list.Add(item);
			}
		}

		return list;
	}

	public async Task<T?> MapAsync<T>(NpgsqlDataReader reader) where T : new()
	{
		if (await reader.ReadAsync())
		{
			return Map<T>(reader);
		}

		return default;
	}

	public async Task<IEnumerable<T>> MapListAsync<T>(NpgsqlDataReader reader) where T : new()
	{
		List<T> list = new();

		while (await reader.ReadAsync())
		{
			T? item = Map<T>(reader);

			if (item != null)
			{
				list.Add(item);
			}
		}

		return list;
	}

	public T MapObject<T>(NpgsqlDataReader reader) where T : new()
	{
		T? obj = new T();
		foreach (PropertyInfo property in typeof(T).GetProperties())
		{
			string columnName = property.Name.ToSnakeCase();

			object value = reader.GetValue(reader.GetOrdinal(columnName));

			if (value != DBNull.Value)
				property.SetValue(obj, value);
		}
		return obj;
	}
}