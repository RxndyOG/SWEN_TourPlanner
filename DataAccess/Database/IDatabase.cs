namespace DataAccess.Database
{
    public interface IDatabase
    {
        void ConnectDatabase();
        string GetConnectionString();
    }
}
