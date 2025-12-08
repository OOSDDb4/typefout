using System.Data;

namespace Typefout.Core.Interfaces;

public interface IDatabaseService
{
    public void Connect();
    public void Open();
    public void Close();
    public DataTable ExecuteQuery(string sql);
}