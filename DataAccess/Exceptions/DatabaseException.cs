namespace DataAccess.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base($"Failed to perform task in the database: {message}") { }
    }
}
