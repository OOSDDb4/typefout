using System.Collections.Generic;
using System.Data;

namespace Typefout.Core.Interfaces
{
    public interface IDatabaseService
    {
        int Connect();
        void Open();
        void Close();

        // Existing
        DataTable ReadQuery(string sql);

        // New: parameterized SQL (safer)
        DataTable ReadQuery(string sql, Dictionary<string, object>? parameters);

        // New: helper execution methods (useful for relation tables)
        int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters);
        object? ExecuteScalar(string sql, Dictionary<string, object>? parameters);

        // Existing
        int Create(string table, Dictionary<string, object> data);

        // New: insert and return auto id (replaces JSON nextId)
        int CreateAndReturnId(string table, Dictionary<string, object> data);

        // Existing Read, plus optional parameters for where
        DataTable Read(
            string table,
            List<string>? columns = null,
            string? where = null,
            Dictionary<string, object>? parameters = null,
            string? joins = null,
            string? orderBy = null,
            int? limit = null);

        int Update(string table, string whereName, string whereValue, Dictionary<string, object> data);
        int Delete(string table, string columnName, int id);
    }
}