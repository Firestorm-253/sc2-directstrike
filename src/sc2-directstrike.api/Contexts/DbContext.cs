using Microsoft.AspNetCore.Routing;
using MySqlConnector;
using System.Collections.Generic;
using System.Text;

namespace sc2_directstrike.api.Contexts;
using DTOs;
using Controllers;
using System.Text.Json;

public class DbContext
{
    public MySqlConnection Connection { get; set; } = null!;

    private readonly IServiceProvider serviceProvider;

    public DbContext(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        this.Connect();
    }

    public void Connect()
    {
        var dbData = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText("dbData.json"))!;
        var databases = JsonSerializer.Deserialize<Dictionary<string, string>>(dbData["databases"].ToString()!)!;

        var connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            { "server", dbData["server"].ToString()! },
            { "port", int.Parse(dbData["port"].ToString()!) },
            { "database", databases[Program.Environment.EnvironmentName.ToLower()].ToString()! },
            { "user", dbData["user"].ToString()! },
            { "password", dbData["password"].ToString()! }
        };
        this.Connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
        this.Connection.Open();
    }

    public async Task<List<Dictionary<string, object>>> ReadFromDb(string query, MySqlTransaction? transaction = null)
    {
        bool sharedTransaction = transaction != null;

        if (!sharedTransaction)
        {
            transaction = await this.Connection.BeginTransactionAsync();
        }
        using var command = new MySqlCommand(query, this.Connection, transaction);

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
            await transaction!.RollbackAsync();
            throw exp;
        }

        if (!sharedTransaction)
        {
            await transaction!.CommitAsync();
        }

        return entries;
    }

    public async Task<ulong?> ExecuteQuery(string query, bool select_lastInsertedId = true, MySqlTransaction? transaction = null)
    {
        bool sharedTransaction = transaction != null;
        if (!sharedTransaction)
        {
            transaction = await this.Connection.BeginTransactionAsync();
        }

        if (select_lastInsertedId)
        {
            query += "; SELECT LAST_INSERT_ID();";
        }

        using var command = new MySqlCommand(query, this.Connection, transaction);

        ulong? index = null;
        try
        {
            index = (ulong?)(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            throw ex;
        }

        if (!sharedTransaction)
        {
            await transaction!.CommitAsync();
        }

        if (select_lastInsertedId)
        {
            return index!.Value;
        }
        return null;
    }

    public async Task<ulong> WriteToDb(string pkt, string table, Dictionary<string, object> dict, MySqlTransaction? transaction = null)
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

        return (await ExecuteQuery($"INSERT INTO {table} ({names}) " +
                                   $"VALUES ({values}) ", transaction: transaction)).Value;
    }

    public async Task UpdateDb(string pkt, string table, Dictionary<string, object> entry)
    {
        var entries = new StringBuilder();
        var conditions = $"WHERE PKT = {PKTController.GetQuery(pkt)} AND Id = '{entry["Id"]}'";

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

        await ExecuteQuery($"UPDATE {table} " +
                           $"SET {entries} " +
                           $"{conditions}");
    }


    public async Task<ulong[]> InsertIncremental(string pkt, string table, IEnumerable<IAsDictionary> postReplays, MySqlConnector.MySqlTransaction? transaction = null)
    {
        var pktId = (await this.ReadFromDb(PKTController.GetQuery(pkt), transaction))[0]["Id"];

        var names = new StringBuilder("PKT");
        var values = new StringBuilder();

        for (int i = 0; i < postReplays.Count(); i++)
        {
            var replayDict = postReplays.ElementAt(i).AsDictionary();

            if (i != 0)
            {
                values.Append(',');
            }
            values.Append($"('{pktId}', ");

            for (int k = 1; k < replayDict.Count; k++)
            {
                var ent = replayDict.ElementAt(k);
                if (i == 0)
                {
                    names.Append($", {ent.Key}");
                }

                if (k != 1)
                {
                    values.Append(", ");
                }
                values.Append($"'{ent.Value}'");
            }

            values.Append(")");
        }

        ulong lastInsertedId = (await this.ExecuteQuery($"INSERT INTO {table} ({names}) " +
                                                             $"VALUES {values} ",
                                                             select_lastInsertedId: true,
                                                             transaction: transaction)).Value;

        ulong[] ids = new ulong[postReplays.Count()];
        for (int i = 0; i < ids.Length; i++)
        {
            ids[i] = lastInsertedId + (ulong)i;
        }
        return ids;
    }
}
