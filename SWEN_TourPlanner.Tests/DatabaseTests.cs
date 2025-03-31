using DataAccess;
using Model;

namespace SWEN_TourPlanner.Tests
{
    public class DatabaseTests
    {
        Database db;

        [SetUp]
        public void Setup()
        {
            db = new Database();
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
            string difficulty = db.GetDifficultyType(1);

            Assert.IsNotNull(difficulty);
            Assert.AreEqual("Easy", difficulty);
        }

        [Test]
        public void Retrieve_Transportation_Type()
        {
            string transportation_type = db.GetTransportationType(3);

            Assert.IsNotNull(transportation_type);
            Assert.AreEqual("Running", transportation_type);
        }

        /* Tours */

        [Test]
        public void Retrieve_All_Tours()
        {
            List<Tour> tours = db.GetTours();

            Assert.NotNull(tours);
            Assert.IsNotEmpty(tours);

            foreach (Tour tour in tours)
            {
                Assert.IsTrue(tour.Name is string);
                Assert.IsTrue(tour.Description is string);
                Assert.IsTrue(tour.From_Location is string);
                Assert.IsTrue(tour.To_Location is string);
                Assert.IsTrue(tour.Transportation_Type is int);
                Assert.IsTrue(tour.Distance is int);
                Assert.IsTrue(tour.Estimated_Time is int);
                Assert.IsTrue(tour.Route_Information is string);
            }
        }

        [Test]
        public void Retrieve_Tour_With_Id()
        {
            // Get id of a tour
            int tourID = db.GetTours()[0].Id;

            // Get tour with the id
            Tour? tour = db.GetTourById(tourID);

            Assert.NotNull(tour);

            Assert.IsTrue(tour.Name is string);
            Assert.IsTrue(tour.Description is string);
            Assert.IsTrue(tour.From_Location is string);
            Assert.IsTrue(tour.To_Location is string);
            Assert.IsTrue(tour.Transportation_Type is int);
            Assert.IsTrue(tour.Distance is int);
            Assert.IsTrue(tour.Estimated_Time is int);
            Assert.IsTrue(tour.Route_Information is string);

            Assert.AreEqual(tourID, tour.Id);
        }

        [Test]
        public void Create_And_Delete_Tour()
        {
            // Get current amount of tours
            int tours_amount = db.GetTours().Count;

            var tour = new Tour
            {
                Name = "New Tour",
                Description = "DELETE ME",
                From_Location = "From",
                To_Location = "To",
                Transportation_Type = 1,
                Distance = 1100,
                Estimated_Time = 60,
                Route_Information = "Test Tour"
            };
            db.CreateTour(tour);

            // Get new amount of tours
            List<Tour> tours = db.GetTours();
            int new_tours_amount = tours.Count;
            Assert.IsTrue(new_tours_amount == tours_amount + 1);

            // Delete latest tour
            int created_tourID = tours[tours.Count - 1].Id;
            db.DeleteTour(created_tourID);

            new_tours_amount = db.GetTours().Count;
            Assert.IsTrue(new_tours_amount == tours_amount);
        }

        [Test]
        public void Update_Tour()
        {
            // Create tour
            var tour = new Tour
            {
                Name = "New Tour",
                Description = "DELETE ME",
                From_Location = "From",
                To_Location = "To",
                Transportation_Type = 1,
                Distance = 1100,
                Estimated_Time = 60,
                Route_Information = "Test Tour"
            };
            db.CreateTour(tour);

            // Update tour
            List<Tour> tours = db.GetTours();
            Tour created_tour = tours[tours.Count - 1];

            var new_tour = new Tour
            {
                Id = created_tour.Id,
                Name = tour.Name + "_Test",
                Description = tour.Description + "_Test",
                From_Location = tour.From_Location + "_Test",
                To_Location = tour.To_Location + "_Test",
                Transportation_Type = tour.Transportation_Type + 1,
                Distance = tour.Distance + 1,
                Estimated_Time = tour.Estimated_Time + 1,
                Route_Information = tour.Route_Information + "_Test"
            };
            db.UpdateTour(new_tour);

            // Get updated tour
            Tour? updated_tour = db.GetTourById(new_tour.Id);

            Assert.AreEqual(new_tour.Name, updated_tour.Name);
            Assert.AreEqual(new_tour.Description, updated_tour.Description);
            Assert.AreEqual(new_tour.From_Location, updated_tour.From_Location);
            Assert.AreEqual(new_tour.To_Location, updated_tour.To_Location);
            Assert.AreEqual(new_tour.Transportation_Type, updated_tour.Transportation_Type);
            Assert.AreEqual(new_tour.Distance, updated_tour.Distance);
            Assert.AreEqual(new_tour.Estimated_Time, updated_tour.Estimated_Time);
            Assert.AreEqual(new_tour.Route_Information, updated_tour.Route_Information);

            // Delete tour
            db.DeleteTour(new_tour.Id);
        }

        [Test]
        public void Check_Valid_Tour_Attributes()
        {
            var tour = new Tour
            {
                Name = "New Tour",
                Description = "DELETE ME",
                From_Location = "From",
                To_Location = "To",
                Transportation_Type = 1,
                Distance = 1100,
                Estimated_Time = 60,
                Route_Information = "Test Tour"
            };

            try
            {
                db.CheckTourRequiredAttributes(tour);
            }
            catch (Exception e)
            {
                Assert.Fail("Exception thrown even though all attributes of the tour object are valid");
            }
        }

        [Test]
        public void Check_Invalid_Tour_Attributes()
        {
            var tour = new Tour
            {
                Name = "", // intentional error
                Description = "DELETE ME",
                From_Location = "From",
                To_Location = "To",
                Transportation_Type = 1,
                Distance = 1100,
                Estimated_Time = 60,
                Route_Information = "Test Tour"
            };

            // Check atribute Name
            CheckExceptionThrown("Name");

            // Check atribute Description
            tour.Name = "New Tour";
            tour.Description = "";
            CheckExceptionThrown("Description");

            // Check atribute From_Location
            tour.Description = "DELETE ME";
            tour.From_Location = "";
            CheckExceptionThrown("From_Location");

            // Check atribute To_Location
            tour.From_Location = "From";
            tour.To_Location = "";
            CheckExceptionThrown("To_Location");

            // Check atribute Transportation_Type
            tour.To_Location = "To";
            tour.Transportation_Type = 0;
            CheckExceptionThrown("Transportation_Type");

            // Check atribute Distance
            tour.Transportation_Type = 1;
            tour.Distance = 0;
            CheckExceptionThrown("Distance");

            // Check atribute Estimated_Time
            tour.Distance = 1100;
            tour.Estimated_Time = -1;
            CheckExceptionThrown("Estimated_Time");

            // Check atribute Route_Information
            tour.Estimated_Time = 60;
            tour.Route_Information = "";
            CheckExceptionThrown("Route_Information");

            /// Executes the method that checks the attributes and fails the test if no exception is thrown
            void CheckExceptionThrown(string attribute)
            {
                bool successful = true;
                try
                {
                    db.CheckTourRequiredAttributes(tour);
                    successful = false;
                }
                catch (Exception e) { }
                if (!successful) Assert.Fail($"Should have thrown exception for attribute: {attribute}");
            }
        }

        /* Tour logs */

        [Test]
        public void Retrieve_Tour_Logs_Of_Tour()
        {
            int tourID;
            List<Tour> tours = db.GetTours();
            List<TourLog> tourlogs;

            int index = 0;
            do
            {
                // Get id of a tour
                tourID = tours[index].Id;

                // Get tour log of tour with the id
                tourlogs = db.GetTourLogsOfTour(tourID);

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
            int tourID = db.GetTours()[0].Id;
            
            // Get current amount of tour logs
            int tourlogs_amount = db.GetTourLogsOfTour(tourID).Count;

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
            db.CreateTourLog(tourlog);

            // Get new amount of tour logs
            List<TourLog> tourlogs = db.GetTourLogsOfTour(tourID);
            int new_tourlogs_amount = tourlogs.Count;
            Assert.IsTrue(new_tourlogs_amount == tourlogs_amount + 1);

            // Delete latest tour log
            int created_tourlogID = tourlogs[tourlogs.Count - 1].Id;
            db.DeleteTourLog(created_tourlogID);

            new_tourlogs_amount = db.GetTourLogsOfTour(tourID).Count;
            Assert.IsTrue(new_tourlogs_amount == tourlogs_amount);
        }

        [Test]
        public void Update_Tour_Log()
        {
            // Get id of tour
            int tourID = db.GetTours()[0].Id;

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
            db.CreateTourLog(tourlog);

            // Update tourlog
            List<TourLog> tourlogs = db.GetTourLogsOfTour(tourID);
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
            db.UpdateTourLog(new_tourlog);

            // Get updated tourlog
            tourlogs = db.GetTourLogsOfTour(tourID);
            TourLog updated_tourlog = tourlogs[tourlogs.Count - 1];

            Assert.AreEqual(new_tourlog.Tour_Id, updated_tourlog.Tour_Id);
            Assert.AreEqual(new_tourlog.Logdate.ToString(), updated_tourlog.Logdate.ToString());
            Assert.AreEqual(new_tourlog.Comment, updated_tourlog.Comment);
            Assert.AreEqual(new_tourlog.Difficulty, updated_tourlog.Difficulty);
            Assert.AreEqual(new_tourlog.Total_Distance, updated_tourlog.Total_Distance);
            Assert.AreEqual(new_tourlog.Total_Time, updated_tourlog.Total_Time);
            Assert.AreEqual(new_tourlog.Rating, updated_tourlog.Rating);

            // Delete tour
            db.DeleteTourLog(updated_tourlog.Id);
        }

        [Test]
        public void Check_Valid_Tour_Logs_Attributes()
        {
            var tourlog = new TourLog
            {
                Tour_Id = 1,
                Logdate = DateTime.Now,
                Comment = "DELETE ME",
                Difficulty = 1,
                Total_Distance = 1000,
                Total_Time = 1,
                Rating = 10
            };

            try
            {
                db.CheckTourLogRequiredAttributes(tourlog);
            }
            catch(Exception e)
            {
                Assert.Fail("Exception thrown even though all attributes of the tour log object are valid");
            }
        }

        [Test]
        public void Check_Invalid_Tour_Logs_Attributes()
        {
            var tourlog = new TourLog
            {
                Tour_Id = 0, // intentional error
                Logdate = DateTime.Now,
                Comment = "DELETE ME",
                Difficulty = 1,
                Total_Distance = 1000,
                Total_Time = 5,
                Rating = 10
            };

            // Check atribute Tour_Id
            CheckExceptionThrown("Tour_Id");

            // By default DateTime can't be invalid

            // Check atribute Comment
            tourlog.Tour_Id = 1;
            tourlog.Comment = "";
            CheckExceptionThrown("Comment");

            // Check atribute Difficulty
            tourlog.Comment = "DELETE ME";
            tourlog.Difficulty = 0;
            CheckExceptionThrown("Difficulty");

            // Check atribute Total_Distance
            tourlog.Difficulty = 1;
            tourlog.Total_Distance = -1;
            CheckExceptionThrown("Total_Distance");

            // Check atribute Total_Time
            tourlog.Total_Distance = 1000;
            tourlog.Total_Time = 0;
            CheckExceptionThrown("Total_Time");

            // Check atribute Rating
            tourlog.Total_Time = 5;
            tourlog.Rating = 0;
            CheckExceptionThrown("Rating");

            /// Executes the method that checks the attributes and fails the test if no exception is thrown
            void CheckExceptionThrown(string attribute)
            {
                bool successful = true;
                try
                {
                    db.CheckTourLogRequiredAttributes(tourlog);
                    successful = false;
                }
                catch (Exception e) { }
                if (!successful) Assert.Fail($"Should have thrown exception for attribute: {attribute}");
            }
        }

    }
}