using api.sc2_directstrike.DTOs;
using Microsoft.AspNetCore.Routing;
using MySqlConnector;
using System.Text;

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

    public static async Task WriteToDb(string pkt, string route, Dictionary<string, object> dict)
    {
        var names = new StringBuilder("PKT, ");
        var values = new StringBuilder($"'{pkt}', ");

        foreach (var keyValuePair in dict)
        {
            names.Append($"{keyValuePair.Key}, ");
            values.Append($"'{keyValuePair.Value}', ");
        }
        names.Remove(names.Length - 2, 2);
        values.Remove(values.Length - 2, 2);

        await DbContext.WriteToDb($"INSERT INTO {route} ({names}) " +
                                  $"VALUES ({values}) ");
    }

    public static string AddCondition(this string query, string name, object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        string connectorString = !query.Contains("WHERE") ? "WHERE" : "AND";
        return $"{connectorString} {name}='{value}' ";
    }

}
