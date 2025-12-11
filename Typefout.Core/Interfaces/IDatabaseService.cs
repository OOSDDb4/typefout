using System.Data;

namespace Typefout.Core.Interfaces;

public interface IDatabaseService
{
    public int Connect();
    public void Open();
    public void Close();
    public DataTable ExecuteQuery(string sql);
    public void Create();
    public void Read();
    public void Update();
    public void Delete();
}