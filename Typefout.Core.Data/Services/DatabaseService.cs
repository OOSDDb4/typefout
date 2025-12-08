using System.Diagnostics;
using Typefout.Core.Interfaces;
using System;
using System.Data;
using MySqlConnector;

namespace Typefout.Core.Data.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _databaseHost;
    private readonly string _databaseUser;
    private readonly string _databasePassword;
    private readonly string _databasePort;
    private readonly string _databaseName;
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
        Connect();
    }

    public void Connect()
    {
        try
        {
            string cs = $"Server={_databaseHost};Port={_databasePort};Database={_databaseName};User Id={_databaseUser};Password={_databasePassword};SslMode=Preferred;";
            _connection = new MySqlConnection(cs);
            Open();
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            Trace.WriteLine($"DatabaseConnection failed: {e.Message}");
            Trace.WriteLine($"Error: {e}");
            throw;
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

    public DataTable ExecuteQuery(string sql)
    {
        using MySqlCommand command = new MySqlCommand(sql, _connection);
        using MySqlDataAdapter adapter = new MySqlDataAdapter(command);
        DataTable table = new DataTable();
        adapter.Fill(table);
        return table;
    }
}

