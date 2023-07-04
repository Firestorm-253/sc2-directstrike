using MySqlConnector;

namespace api.sc2_directstrike;

public static class DbContext
{
    public static MySqlConnection Connection { get; set; } = null!;

    public static void Connect(string server, int port, string database, string user, string password)
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            { "server", server },
            { "port", port },
            { "database", database },
            { "user", user },
            { "password", password }
        };
        Connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
        Connection.Open();
    }

    public static async Task<List<object[]>> ReadFromDb(string query)
    {
        var command = new MySqlCommand(query, Connection);
        var rows = new List<object[]>();

        using (var dataReader = await command.ExecuteReaderAsync())
        {
            while (await dataReader.ReadAsync())
            {
                var row = new object[dataReader.FieldCount];
                dataReader.GetValues(row);
                rows.Add(row);
            }
        };

        return rows;
    }

    public static async Task WriteToDb(string query)
    {
        var command = new MySqlCommand(query, Connection);
        await command.ExecuteNonQueryAsync();
    }
}
