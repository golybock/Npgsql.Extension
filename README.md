# Npgsql.Extension

Extension for Npgsql ADO.Net

Simply structure and fast write repositories to Npgsql

Includes reader with auto mapping data classes to data returned from sql query

Now only available snake_case (database) and UpperCamelCase(data class)

Main classes (interface + base realization):
- IReader + Reader
- IDatabaseOptions + DatabaseOptions
- NpgsqlRepository (abstract)

### Example usage with CRUD:

- Table
```postgresql
create table users
(
    id       uuid        not null
        primary key,
    username varchar(100) not null
        unique
);
```

- Database model
```csharp
public class UserDatabase
{
    public Guid Id { get; set; }

    public String Username { get; set; } = null!;
}
```

- Repository interface
```csharp
public interface IUserRepository
{
    public Task<UserDatabase?> GetUserAsync(Guid id);

    public Task<Boolean> CreateUserAsync(UserDatabase userDatabase);

    public Task<Boolean> UpdateUserAsync(Guid id, UserDatabase userDatabase);

    public Task<Boolean> DeleteUserAsync(Guid id);
}
```

- Repository
```csharp
public class UserRepository : NpgsqlRepository, IUserRepository
{
    public UserRepository(IDatabaseOptions databaseOptions) : base(databaseOptions) { }

    public async Task<UserDatabase?> GetUserAsync(Guid id)
    {
        string query = "select * from users where id = $1";

        var parameters = new[]
        {
            new NpgsqlParameter{Value = id}
        };

        return await GetAsync<UserDatabase>(query, parameters);
    }

    public async Task<Boolean> CreateUserAsync(UserDatabase userDatabase)
    {
        string query = "insert into users(id, username)" +
                       "values($1, $2)";

        var parameters = new[]
        {
            new NpgsqlParameter{Value = userDatabase.Id},
            new NpgsqlParameter{Value = userDatabase.Username}
        };

        return await ExecuteAsync(query, parameters);
    }

    public async Task<Boolean> UpdateUserAsync(Guid id, UserDatabase userDatabase)
    {
        string query = "update users set " +
                       "username = $2" +
                       "where id = $1";

        var parameters = new[]
        {
            new NpgsqlParameter{Value = userDatabase.Id},
            new NpgsqlParameter{Value = userDatabase.Username},
        };

        return await ExecuteAsync(query, parameters);
    }

    public Task<Boolean> DeleteUserAsync(Guid id)
    {
        return DeleteAsync("users", "id", id);
    }
}
```