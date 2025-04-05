using DataAccess.Repositories;
using Model;

namespace DataAccess.Tests
{
    public class TourLogRepositoryTests
    {
        TourRepository _tourRepository;
        TourLogRepository _tourlogRepository;

        [SetUp]
        public void Setup()
        {
            _tourRepository = new TourRepository(new Database.Database());
            _tourlogRepository = new TourLogRepository(new Database.Database());
        }

        [Test]
        public void Retrieve_Tour_Logs_Of_Tour()
        {
            int tourID;
            List<Tour> tours = _tourRepository.GetAllTours();
            List<TourLog> tourlogs;

            int index = 0;
            do
            {
                // Get id of a tour
                tourID = tours[index].Id;

                // Get tour log of tour with the id
                tourlogs = _tourlogRepository.GetTourLogs(tourID);

                // Check next tour
                index++;
                if (index == tours.Count)
                {
                    Assert.Fail("There are no tours with tour logs to test");
                }
            } while (tourlogs.Count == 0);

            Assert.NotNull(tourlogs);
            Assert.IsNotEmpty(tourlogs);

            foreach (TourLog tourlog in tourlogs)
            {
                Assert.IsTrue(tourlog.Id is int);
                Assert.IsTrue(tourlog.Tour_Id is int);
                Assert.IsTrue(tourlog.Logdate is DateTime);
                Assert.IsTrue(tourlog.Comment is string);
                Assert.IsTrue(tourlog.Difficulty is int);
                Assert.IsTrue(tourlog.Total_Distance is int);
                Assert.IsTrue(tourlog.Total_Time is int);
                Assert.IsTrue(tourlog.Rating is int);

                Assert.AreEqual(tourID, tourlog.Tour_Id, "Tour log of another tour was retrieved");
            }

        }

        [Test]
        public void Create_And_Delete_Tour_Log()
        {
            // Get id of tour
            int tourID = _tourRepository.GetAllTours()[0].Id;

            // Get current amount of tour logs
            int tourlogs_amount = _tourlogRepository.GetTourLogs(tourID).Count;

            var tourlog = new TourLog
            {
                Tour_Id = tourID,
                Logdate = DateTime.Now,
                Comment = "DELETE ME",
                Difficulty = 1,
                Total_Distance = 1000,
                Total_Time = 1,
                Rating = 10
            };
            _tourlogRepository.AddTourLog(tourlog);

            // Get new amount of tour logs
            List<TourLog> tourlogs = _tourlogRepository.GetTourLogs(tourID);
            int new_tourlogs_amount = tourlogs.Count;
            Assert.IsTrue(new_tourlogs_amount == tourlogs_amount + 1);

            // Delete latest tour log
            int created_tourlogID = tourlogs[tourlogs.Count - 1].Id;
            _tourlogRepository.DeleteTourLog(created_tourlogID);

            new_tourlogs_amount = _tourlogRepository.GetTourLogs(tourID).Count;
            Assert.IsTrue(new_tourlogs_amount == tourlogs_amount);
        }

        [Test]
        public void Update_Tour_Log()
        {
            // Get id of tour
            int tourID = _tourRepository.GetAllTours()[0].Id;

            // Create tourlog
            var tourlog = new TourLog
            {
                Tour_Id = tourID,
                Logdate = DateTime.Now,
                Comment = "DELETE ME",
                Difficulty = 1,
                Total_Distance = 1000,
                Total_Time = 1,
                Rating = 10
            };
            _tourlogRepository.AddTourLog(tourlog);

            // Update tourlog
            List<TourLog> tourlogs = _tourlogRepository.GetTourLogs(tourID);
            TourLog created_tourlog = tourlogs[tourlogs.Count - 1];

            var new_tourlog = new TourLog
            {
                Id = created_tourlog.Id,
                Tour_Id = created_tourlog.Tour_Id,
                Logdate = DateTime.Now,
                Comment = created_tourlog.Comment + "_Test",
                Difficulty = created_tourlog.Difficulty + 1,
                Total_Distance = created_tourlog.Total_Distance + 1,
                Total_Time = created_tourlog.Total_Time + 1,
                Rating = created_tourlog.Rating - 1
            };
            _tourlogRepository.UpdateTourLog(new_tourlog);

            // Get updated tourlog
            tourlogs = _tourlogRepository.GetTourLogs(tourID);
            TourLog updated_tourlog = tourlogs[tourlogs.Count - 1];

            Assert.AreEqual(new_tourlog.Tour_Id, updated_tourlog.Tour_Id);
            Assert.AreEqual(new_tourlog.Logdate.ToString(), updated_tourlog.Logdate.ToString());
            Assert.AreEqual(new_tourlog.Comment, updated_tourlog.Comment);
            Assert.AreEqual(new_tourlog.Difficulty, updated_tourlog.Difficulty);
            Assert.AreEqual(new_tourlog.Total_Distance, updated_tourlog.Total_Distance);
            Assert.AreEqual(new_tourlog.Total_Time, updated_tourlog.Total_Time);
            Assert.AreEqual(new_tourlog.Rating, updated_tourlog.Rating);

            // Delete tour
            _tourlogRepository.DeleteTourLog(updated_tourlog.Id);
        }
    }
}
