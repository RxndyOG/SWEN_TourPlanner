using UI.ViewModels;
using Model;

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
            _model.PropertyChanged += (s, e) => { if (e.PropertyName == "ImagePath") raised = true; };
            _model.ImagePath = "test.png";
            Assert.IsTrue(raised);
        }

        [Test]
        public void SaveTourLog_ValidData_AddsLog()
        {
            _model.TourLog.Date = "2025-01-01";
            _model.TourLog.Time = "10:00";
            _model.TourLog.Difficulty = "Easy";
            _model.TourLog.Duration = "1h";
            _model.TourLog.Distance = "5km";
            _model.TourLog.Rating = "5";
            _model.TourLog.Comment = "Nice tour";

            _model.AddTourLogCommand.Execute(null);

            Assert.That(_model.TourLog.TourLogsTable.Count, Is.EqualTo(1));
            Assert.That(_model.TourLog.TourLogsTable.First().Date, Is.EqualTo("2025-01-01"));
        }

        [Test]
        public void RemoveTourLog_ValidId_RemovesLog()
        {
            _model.TourLog.TourLogsTable.Add(new TourLogs.TourLog { IDTourLogs = 1, Date = "2025-01-01" });
            _model.IDTourLogsTest = 1;

            _model.RemoveTourCommand.Execute(null);

            Assert.That(_model.TourLog.TourLogsTable.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveTourLog_InvalidId_DoesNotRemove()
        {
            _model.TourLog.TourLogsTable.Add(new TourLogs.TourLog { IDTourLogs = 1, Date = "2025-01-01" });
            _model.IDTourLogsTest = 2;

            _model.RemoveTourCommand.Execute(null);

            Assert.That(_model.TourLog.TourLogsTable.Count, Is.EqualTo(1));
        }
    }
}
