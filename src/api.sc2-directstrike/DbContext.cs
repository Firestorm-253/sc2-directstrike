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

    public static async Task<List<Dictionary<string, object>>> ReadFromDb(string query)
    {
        var command = new MySqlCommand(query, Connection);
        var entries = new List<Dictionary<string, object>>();

        using (var dataReader = await command.ExecuteReaderAsync())
        {
            var columns = await dataReader.GetColumnSchemaAsync();

            while (await dataReader.ReadAsync())
            {
                var values = new object[dataReader.FieldCount];
                dataReader.GetValues(values);

                var entry = new Dictionary<string, object>();
                for (int i = 0; i < values.Length; i++)
                {
                    entry.Add(columns[i].ColumnName, values[i]);
                }

                entries.Add(entry);
            }
        };

        return entries;
    }

    public static async Task WriteToDb(string query)
    {
        var command = new MySqlCommand(query, Connection);
        await command.ExecuteNonQueryAsync();
    }
}
