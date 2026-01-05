using System.Data;
using System.Diagnostics;
using System.Text;
using MySqlConnector;
using Typefout.Core.Interfaces;

namespace Typefout.Core.Data.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string? _databaseHost;
    private readonly string? _databaseUser;
    private readonly string? _databasePassword;
    private readonly string? _databasePort;
    private readonly string? _databaseName;
    private MySqlConnection _connection;

    public DatabaseService()
    {
        // Getting the database credentials from the .env file
        Trace.WriteLine("Database service initialized");
        EnvService.Load();
        _databaseHost = EnvService.Get("DATABASE_HOST");
        _databaseUser = EnvService.Get("DATABASE_USER");
        _databasePassword = EnvService.Get("DATABASE_PASSWORD");
        _databasePort = EnvService.Get("DATABASE_PORT");
        _databaseName = EnvService.Get("DATABASE_NAME");
    }

    public int Connect()
    {
        try
        {
            string cs = $"Server={_databaseHost};Port={_databasePort};Database={_databaseName};User Id={_databaseUser};Password={_databasePassword};SslMode=Preferred;";
            _connection = new MySqlConnection(cs);
            _connection.Open();
            _connection.Close();
            return 202;
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            Trace.WriteLine($"DatabaseConnection failed: {e.Message}");
            Trace.WriteLine($"Error: {e}");
            return 500;
        }
    }

    public void Open()
    {
        if (_connection.State != ConnectionState.Open) _connection.Open();
    }

    public void Close()
    {
        if (_connection.State != ConnectionState.Closed) _connection.Close();
    }

    public DataTable ReadQuery(string sql)
    {
        using MySqlCommand command = new MySqlCommand(sql, _connection);
        using MySqlDataAdapter adapter = new MySqlDataAdapter(command);
        DataTable table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public int Create(string table, Dictionary<string, object> data)
    {
        try
        {
            if (data == null || data.Count == 0)
            {
                throw new ArgumentNullException("No data provided");
            }
            string columns = string.Join(", ", data.Keys.Select(c => $"`{c}`"));
            string parameters = string.Join(", ", data.Keys.Select(c => $"@{c}"));
            string query = $"INSERT INTO {table} ({columns}) VALUES ({parameters})";
            using MySqlCommand command = new MySqlCommand(query, _connection);
            foreach (KeyValuePair<string, object> item in data)
            {
                command.Parameters.AddWithValue($"@{item.Key}", item.Value ?? DBNull.Value);
            }
            command.ExecuteNonQuery();
            return 202;
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
            // throw new Exception("Something went wrong with the creation of the database");
            return 500;
        }
    }

    public DataTable Read(
        string table,
        List<string>? columns = null,
        string? where = null,
        string? joins = null,
        string? orderBy = null,
        int? limit = null)
    {
        try
        {
            // Check if the database table is entered
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentException("Table is required");
            }

            // Check if there are columns added, standard is everything (*)
            string selectedColumns = (columns == null || columns.Count == 0) ? "*" : string.Join(", ", columns);

            StringBuilder query = new StringBuilder();
            query.Append($"SELECT {selectedColumns} FROM {table}");

            if (!string.IsNullOrWhiteSpace(joins))
            {
                query.Append($" {joins}");
            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                query.Append($" WHERE {where}");
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query.Append($" ORDER BY {orderBy}");
            }

            if (limit.HasValue)
            {
                query.Append($" LIMIT @limit");
            }

            using MySqlCommand command = new MySqlCommand(query.ToString(), _connection);

            if (limit.HasValue)
            {
                command.Parameters.AddWithValue($"@limit", limit.Value);
            }

            using MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
            throw;
        }
    }


    public void Update()
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}

