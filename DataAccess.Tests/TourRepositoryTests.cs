using DataAccess.Repositories;
using Model;

namespace DataAccess.Tests
{
    public class TourRepositoryTests
    {
        TourRepository _tourRepository;

        [SetUp]
        public void Setup()
        {
            _tourRepository = new TourRepository(new Database.Database());
        }

        [Test]
        public void Retrieve_All_Tours()
        {
            List<Tour> tours = _tourRepository.GetAllTours();

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
            int tourID = _tourRepository.GetAllTours()[0].Id;

            // Get tour with the id
            Tour? tour = _tourRepository.GetTour(tourID);

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
            int tours_amount = _tourRepository.GetAllTours().Count;

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
            _tourRepository.AddTour(tour);

            // Get new amount of tours
            List<Tour> tours = _tourRepository.GetAllTours();
            int new_tours_amount = tours.Count;
            Assert.IsTrue(new_tours_amount == tours_amount + 1);

            // Delete latest tour
            int created_tourID = tours[tours.Count - 1].Id;
            _tourRepository.DeleteTour(created_tourID);

            new_tours_amount = _tourRepository.GetAllTours().Count;
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
            _tourRepository.AddTour(tour);

            // Update tour
            List<Tour> tours = _tourRepository.GetAllTours();
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
            _tourRepository.UpdateTour(new_tour);

            // Get updated tour
            Tour? updated_tour = _tourRepository.GetTour(new_tour.Id);

            Assert.AreEqual(new_tour.Name, updated_tour.Name);
            Assert.AreEqual(new_tour.Description, updated_tour.Description);
            Assert.AreEqual(new_tour.From_Location, updated_tour.From_Location);
            Assert.AreEqual(new_tour.To_Location, updated_tour.To_Location);
            Assert.AreEqual(new_tour.Transportation_Type, updated_tour.Transportation_Type);
            Assert.AreEqual(new_tour.Distance, updated_tour.Distance);
            Assert.AreEqual(new_tour.Estimated_Time, updated_tour.Estimated_Time);
            Assert.AreEqual(new_tour.Route_Information, updated_tour.Route_Information);

            // Delete tour
            _tourRepository.DeleteTour(new_tour.Id);
        }
    }
}