using Business.Services;
using Business.Exceptions;
using Model;

namespace Business.Tests
{
    public class TourServiceTests
    {
        TourService _tourService;

        [SetUp]
        public void Setup()
        {
            _tourService = new TourService(null);
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
                Transportation_Type = "Bike",
                Distance = 1100,
                Estimated_Time = 60,
                Route_Information = "Test Tour"
            };

            try
            {
                _tourService.CheckTourRequiredAttributes(tour);
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
                Transportation_Type = "Bike",
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
            tour.Transportation_Type = "";
            CheckExceptionThrown("Transportation_Type");

            // Check atribute Distance
            tour.Transportation_Type = "Bike";
            tour.Distance = 0;
            CheckExceptionThrown("Distance");

            // Check atribute Estimated_Time
            tour.Distance = 1100;
            tour.Estimated_Time = -1;
            CheckExceptionThrown("Estimated_Time");


            /// Executes the method that checks the attributes and fails the test if no exception is thrown
            void CheckExceptionThrown(string attribute)
            {
                Exception exception;
                if (attribute == "Distance" || attribute == "Estimated_Time")
                {
                    exception = new NonPositiveNumberException(attribute);
                }
                else
                {
                    exception = new EmptyAttributeException(attribute);
                }

                bool successful = true;
                try
                {
                    _tourService.CheckTourRequiredAttributes(tour);
                    successful = false;
                }
                catch (Exception e) 
                {
                    Assert.AreEqual(e.GetType(), exception.GetType());
                }
                if (!successful) Assert.Fail($"Should have thrown exception for attribute: {attribute}");
            }
        }
    }
}