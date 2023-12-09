namespace Npgsql.Extension.Options;

public class DatabaseOptions : IDatabaseOptions
{
	public string ConnectionString { get; set; } = null!;
}