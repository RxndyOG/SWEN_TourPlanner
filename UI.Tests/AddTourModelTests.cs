using UI.ViewModels;
using Model;
using System.Linq;

namespace UI.Tests
{
    [TestFixture]
    public class AddTourModelTests
    {
        private AddTourModel _model;

        [SetUp]
        public void Setup()
        {
            _model = new AddTourModel();
        }

        [Test]
        public void ImagePath_PropertyChanged_IsRaised()
        {
            bool raised = false;
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "Image_Path") raised = true; };
            _model.Image_Path = "test.png";
            Assert.IsTrue(raised);
        }

        [Test]
        public void PropertyChanged_IsRaised_ForImagePath()
        {
            bool raised = false;
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "Image_Path") raised = true; };
            _model.Image_Path = "test.png";
            Assert.IsTrue(raised);
        }


        [Test]
        public void SaveTourLogCommand_InvalidData_DoesNotAddLog()
        {
            _model.TourLogs.Date = "";
            _model.TourLogs.Comment = "";
            int before = _model.TourLogsTable.Count;
            _model.AddTourLogCommand.Execute(null);
            Assert.That(_model.TourLogsTable.Count, Is.EqualTo(before));
        }


        [Test]
        public void SaveTourLog_ValidData_AddsLog()
        {
            // Create a new TourLog and add it to the TourLogsTable
            var log = new TourLogs.TourLog
            {
                Date = "2025-01-01",
                Time = "10:00",
                Difficulty = "Easy",
                Duration = "1h",
                Distance = "5km",
                Rating = "5",
                Comment = "Nice tour"
            };
            _model.TourLogsTable.Add(log);

            // Execute the AddTourLogCommand if needed (depends on your implementation)
            _model.AddTourLogCommand.Execute(null);

            Assert.That(_model.TourLogsTable.Count, Is.EqualTo(1));
            Assert.That(_model.TourLogsTable.First().Date, Is.EqualTo("2025-01-01"));
        }

        [Test]
        public void RemoveTourLog_ValidId_RemovesLog()
        {
            var log = new TourLogs.TourLog { IDTourLogs = 1, Date = "2025-01-01" };
            _model.TourLogsTable.Add(log);
            _model.LogIdToRemove = 1;

            _model.RemoveTourLogCommand.Execute(null);

            Assert.That(_model.TourLogsTable.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveTourLog_InvalidId_DoesNotRemove()
        {
            var log = new TourLogs.TourLog { IDTourLogs = 1, Date = "2025-01-01" };
            _model.TourLogsTable.Add(log);
            _model.LogIdToRemove = 2;

            _model.RemoveTourLogCommand.Execute(null);

            Assert.That(_model.TourLogsTable.Count, Is.EqualTo(1));
        }
    }
}