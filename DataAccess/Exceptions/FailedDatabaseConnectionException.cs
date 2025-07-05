namespace DataAccess.Exceptions
{
    public class FailedDatabaseConnectionException : Exception
    {
        public FailedDatabaseConnectionException() : base("Failed to connect to the database") { }
    }
}
