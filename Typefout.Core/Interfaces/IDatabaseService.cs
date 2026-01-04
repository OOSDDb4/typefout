using System.Data;

namespace Typefout.Core.Interfaces;

public interface IDatabaseService
{
    public int Connect();
    public void Open();
    public void Close();
    public DataTable ReadQuery(string sql);
    public int Create(string table, Dictionary<string, object> data);
    public DataTable Read(string table, List<string>? columns = null, string? where = null, string? joins = null, string? orderBy = null, int? limit = null);
    public void Update();
    public int Delete(string table, string idName, int id);
}