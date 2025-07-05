namespace DataAccess.Tests
{
    public class DatabaseTests
    {
        Database.Database db;

        [SetUp]
        public void Setup()
        {
            db = new Database.Database();
        }

        [Test]
        public void Check_Database_Connection()
        {
            try
            {
                db.ConnectDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail("Exception thrown while connecting to database");
            }
        }
    }
}
