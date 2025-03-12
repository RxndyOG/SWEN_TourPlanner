using SWEN_TourPlanner.Database;

namespace SWEN_TourPlanner.Tests
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

        [Test]
        public void Retrieve_Difficulty_Type()
        {
            string difficulty = db.Retrieve_Difficulty_By_Id(1);

            Assert.IsNotNull(difficulty);
            Assert.AreEqual("Easy", difficulty);
        }

        [Test]
        public void Retrieve_Transportation_Type()
        {
            string transportation_type = db.Retrieve_Transportation_Type_By_Id(3);

            Assert.IsNotNull(transportation_type);
            Assert.AreEqual("Running", transportation_type);
        }
    }
}