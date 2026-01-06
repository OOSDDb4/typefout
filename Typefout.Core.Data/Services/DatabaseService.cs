using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MySqlConnector;
using Typefout.Core.Interfaces;

namespace Typefout.Core.Data.Services
{
    public class DatabaseService : IDatabaseService, IDisposable
    {
        private readonly string? _databaseHost;
        private readonly string? _databaseUser;
        private readonly string? _databasePassword;
        private readonly string? _databasePort;
        private readonly string? _databaseName;

        private MySqlConnection? _connection;

        public DatabaseService()
        {
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
                string cs =
                    $"Server={_databaseHost};Port={_databasePort};Database={_databaseName};User Id={_databaseUser};Password={_databasePassword};SslMode=Preferred;";
                _connection = new MySqlConnection(cs);

                _connection.Open();
                _connection.Close();

                return 202;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"DatabaseConnection failed: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return 500;
            }
        }

        public void Open()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void Close()
        {
            if (_connection == null) return;

            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public DataTable ReadQuery(string sql)
        {
            return ReadQuery(sql, null);
        }

        public DataTable ReadQuery(string sql, Dictionary<string, object>? parameters)
        {
            using MySqlCommand command = new MySqlCommand(sql, _connection);
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                }
            }

            using MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }


        public int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
                }

                using MySqlCommand command = new MySqlCommand(sql, _connection);

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> kvp in parameters)
                    {
                        command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }

                int affected = command.ExecuteNonQuery();
                return affected;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"DatabaseService ExecuteNonQuery Error: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return -1;
            }
        }

        public object? ExecuteScalar(string sql, Dictionary<string, object>? parameters = null)
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
                }

                using MySqlCommand command = new MySqlCommand(sql, _connection);

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> kvp in parameters)
                    {
                        command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }

                object? result = command.ExecuteScalar();
                return result;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"DatabaseService ExecuteScalar Error: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return null;
            }
        }

        public int Create(string table, Dictionary<string, object> data)
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
                }

                if (data == null || data.Count == 0)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided");
                }

                string columns = string.Join(", ", data.Keys.Select(c => $"`{c}`"));
                string parameters = string.Join(", ", data.Keys.Select(c => $"@{c}"));
                string query = $"INSERT INTO `{table}` ({columns}) VALUES ({parameters})";

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
                Trace.WriteLine($"DatabaseService Create Error: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return 500;
            }
        }

        public int CreateAndReturnId(string table, Dictionary<string, object> data)
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
                }

                if (data == null || data.Count == 0)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided");
                }

                string columns = string.Join(", ", data.Keys.Select(c => $"`{c}`"));
                string parameters = string.Join(", ", data.Keys.Select(c => $"@{c}"));

                string insertSql = $"INSERT INTO `{table}` ({columns}) VALUES ({parameters});";

                using MySqlCommand insertCommand = new MySqlCommand(insertSql, _connection);
                foreach (KeyValuePair<string, object> item in data)
                {
                    insertCommand.Parameters.AddWithValue($"@{item.Key}", item.Value ?? DBNull.Value);
                }

                insertCommand.ExecuteNonQuery();

                using MySqlCommand idCommand = new MySqlCommand("SELECT LAST_INSERT_ID();", _connection);
                object? result = idCommand.ExecuteScalar();

                int newId = Convert.ToInt32(result);
                return newId;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"DatabaseService CreateAndReturnId Error: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return -1;
            }
        }

        public DataTable Read(
            string table,
            List<string>? columns = null,
            string? where = null,
            Dictionary<string, object>? parameters = null,
            string? joins = null,
            string? orderBy = null,
            int? limit = null)
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
            }

            if (string.IsNullOrWhiteSpace(table))
            {
                throw new ArgumentException("Table is required", nameof(table));
            }

            string selectedColumns = (columns == null || columns.Count == 0)
                ? "*"
                : string.Join(", ", columns);

            StringBuilder query = new StringBuilder();
            query.Append($"SELECT {selectedColumns} FROM `{table}`");

            if (!string.IsNullOrWhiteSpace(joins))
            {
                query.Append(" ");
                query.Append(joins);
            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                query.Append(" WHERE ");
                query.Append(where);
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query.Append(" ORDER BY ");
                query.Append(orderBy);
            }

            if (limit.HasValue)
            {
                query.Append(" LIMIT @limit");
            }

            using MySqlCommand command = new MySqlCommand(query.ToString(), _connection);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                }
            }

            if (limit.HasValue)
            {
                command.Parameters.AddWithValue("@limit", limit.Value);
            }

            using MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }

        public int Update(string table, string whereName, string whereValue, Dictionary<string, object> data)
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
                }

                if (data == null || data.Count == 0)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided");
                }

                string setClause = string.Join(", ", data.Keys.Select(k => $"`{k}` = @set_{k}"));
                StringBuilder query = new StringBuilder();
                query.Append($"UPDATE `{table}` SET {setClause} WHERE `{whereName}` = @whereValue");

                using MySqlCommand command = new MySqlCommand(query.ToString(), _connection);

                foreach (KeyValuePair<string, object> item in data)
                {
                    command.Parameters.AddWithValue($"@set_{item.Key}", item.Value ?? DBNull.Value);
                }

                command.Parameters.AddWithValue("@whereValue", whereValue);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0 ? 202 : 404;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"DatabaseService Update Error: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return 500;
            }
        }

        public int Delete(string table, string columnName, int id)
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized. Call Connect() first.");
                }

                string query = $"DELETE FROM `{table}` WHERE `{columnName}` = @id";

                using MySqlCommand command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@id", id);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0 ? 202 : 404;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"DatabaseService Delete Error: {e.Message}");
                Trace.WriteLine($"Error: {e}");
                return 500;
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}
