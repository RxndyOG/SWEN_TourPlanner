﻿using Business.Services;
using Model;

namespace Business.Tests
{
    public class TourLogServiceTests
    {
        TourLogService _tourlogService;

        [SetUp]
        public void Setup()
        {
            _tourlogService = new TourLogService(null);
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
                _tourlogService.CheckTourLogRequiredAttributes(tourlog);
            }
            catch (Exception e)
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
                    _tourlogService.CheckTourLogRequiredAttributes(tourlog);
                    successful = false;
                }
                catch (Exception e) { }
                if (!successful) Assert.Fail($"Should have thrown exception for attribute: {attribute}");
            }
        }
    }
}
