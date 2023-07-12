﻿using Microsoft.AspNetCore.Routing;
using MySqlConnector;
using System.Collections.Generic;
using System.Text;

namespace api.sc2_directstrike.Contexts;
using DTOs;

public class DbContext
{
    public MySqlConnection Connection { get; set; } = null!;

    private readonly IServiceProvider serviceProvider;

    public DbContext(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
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

    public async Task WriteToDb(string query)
    {
        using var transaction = await this.Connection.BeginTransactionAsync();
        using var command = new MySqlCommand(query, this.Connection, transaction);
        
        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        transaction.Commit();
    }

    public async Task WriteToDb(string pkt, string route, Dictionary<string, object> dict)
    {
        var names = new StringBuilder("PKT, ");
        var values = new StringBuilder($"'{pkt}', ");

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

        await WriteToDb($"INSERT INTO {route} ({names}) " +
                             $"VALUES ({values}) ");
    }

    public async Task UpdateDb(string pkt, string route, Dictionary<string, object> entry)
    {
        var entries = new StringBuilder();
        var conditions = $"WHERE PKT='{pkt}' AND Id='{entry["Id"]}'";

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

        await WriteToDb($"UPDATE {route} " +
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
