using Microsoft.AspNetCore.Routing;
using MySqlConnector;
using System.Collections.Generic;
using System.Text;

namespace sc2_directstrike.api.Contexts;
using DTOs;
using sc2_directstrike.api.Controllers;

public class DbContext
{
    public MySqlConnection Connection { get; set; } = null!;

    private readonly IServiceProvider serviceProvider;

    public DbContext(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        this.ConnectDb();
    }

    public void ConnectDb()
    {
        var privatData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("privatdata.json"))!;

        string dbName = string.Empty;
        if (Program.Environment.IsProduction())
        {
            dbName = "sc2_directstrike";
        }
        else if (Program.Environment.IsDevelopment())
        {
            dbName = "sc2_directstrike_dev";
        }

        this.Connect("server90.hostfactory.ch", 3306, dbName,
            user: privatData["user"],
            password: privatData["password"]);
    }

    public void Connect(string server, int port, string database, string user, string password)
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

    public async Task<List<Dictionary<string, object>>> ReadFromDb(string query)
    {
        using var command = new MySqlCommand(query, this.Connection);

        var entries = new List<Dictionary<string, object>>();

        try
        {
            using var dataReader = await command.ExecuteReaderAsync();
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
        }
        catch (Exception exp)
        {
            throw exp;
        }

        return entries;
    }

    public async Task<ulong> WriteToDb(string query)
    {
        query += "; SELECT LAST_INSERT_ID();";

        using var transaction = await this.Connection.BeginTransactionAsync();
        using var command = new MySqlCommand(query, this.Connection, transaction);

        ulong? index = null;
        try
        {
            index = (ulong?)(await command.ExecuteScalarAsync());
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        transaction.Commit();

        return index!.Value;
    }

    public async Task<ulong> WriteToDb(string pkt, string table, Dictionary<string, object> dict)
    {
        var names = new StringBuilder("PKT, ");
        var values = new StringBuilder($"{PKTController.GetQuery(pkt)}, ");

        foreach (var keyValuePair in dict)
        {
            if (keyValuePair.Key == "Id")
            {
                continue;
            }

            names.Append($"{keyValuePair.Key}, ");
            values.Append($"'{keyValuePair.Value}', ");
        }
        names.Remove(names.Length - 2, 2);
        values.Remove(values.Length - 2, 2);

        return await WriteToDb($"INSERT INTO {table} ({names}) " +
                               $"VALUES ({values}) ");
    }

    public async Task UpdateDb(string pkt, string table, Dictionary<string, object> entry)
    {
        var entries = new StringBuilder();
        var conditions = $"WHERE PKT={PKTController.GetQuery(pkt)} AND Id='{entry["Id"]}'";

        for (int i = 1; i < entry.Count; i++)
        {
            var name = entry.Keys.ElementAt(i);
            var value = entry.Values.ElementAt(i);
            entries.Append($"{name}='{value}'");

            if (i < entry.Count - 1)
            {
                entries.Append(", ");
            }
        }

        await WriteToDb($"UPDATE {table} " +
                             $"SET {entries} " +
                             $"{conditions}");
    }

    public static string AddCondition(string query, string name, object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        string connectorString = !query.Contains("WHERE") ? "WHERE" : "AND";
        return $"{connectorString} {name}='{value}' ";
    }
}
