using Npgsql.Extension.Tools;

namespace Npgsql.Extension.Reader;

public class Reader<T> : IReader<T> where T : new()
{
    public static async Task<T?> ReadAsync(NpgsqlDataReader reader)
    {
        if (await reader.ReadAsync())
        {
            var obj = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                var value = reader.GetValue(reader.GetOrdinal(property.Name.ToSnakeCase()));

                if (value != DBNull.Value)
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(obj, value);
                    }
                }
            }

            return obj;
        }

        return default(T);
    }

    public static async Task<IEnumerable<T>> ReadListAsync(NpgsqlDataReader reader)
    {
        IList<T> objects = new List<T>();

        while (await reader.ReadAsync())
        {
            var obj = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                var value = reader.GetValue(reader.GetOrdinal(property.Name.ToSnakeCase()));

                if (value != DBNull.Value)
                    property.SetValue(obj, value);
            }

            objects.Add(obj);
        }

        return objects;
    }
}