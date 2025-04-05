using UI.ViewModels;
using Model;

namespace UI.Tests
{
    public class AddTourModelTests
    {
        AddTourModel _addTourModel;

        [SetUp]
        public void Setup()
        {
            _addTourModel = new AddTourModel();
        }

        [Test]
        public void SaveTourLog_ValidData()
        {
            // Arrange
            _addTourModel.TourLog.Date = "2025-03-23";
            _addTourModel.TourLog.Time = "12:00";
            _addTourModel.TourLog.Difficulty = "Medium";
            _addTourModel.TourLog.Duration = "2 hours";
            _addTourModel.TourLog.Distance = "10 km";
            _addTourModel.TourLog.Rating = "4";
            _addTourModel.TourLog.Comment = "Great tour!";

            // Act
            _addTourModel.SaveTourCommand.Execute(null);

            // Assert
            Assert.That(_addTourModel.TourLog.TourLogsTable.Count, Is.EqualTo(1));
            Assert.That(_addTourModel.TourLog.TourLogsTable.First().Date, Is.EqualTo("2025-03-23"));
        }

        [Test]
        public void RemoveTourLog_ValidId()
        {
            // Arrange
            _addTourModel.TourLog.TourLogsTable.Add(new TourLogs.TourLog { IDTourLogs = 1, Date = "2025-03-23" });
            _addTourModel.IDTourLogsTest = 1;

            // Act
            _addTourModel.RemoveTourCommand.Execute(null);

            // Assert
            Assert.That(_addTourModel.TourLog.TourLogsTable.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveTourLog_InvalidId()
        {
            // Arrange
            _addTourModel.TourLog.TourLogsTable.Add(new TourLogs.TourLog { IDTourLogs = 1, Date = "2025-03-23" });
            _addTourModel.IDTourLogsTest = 2; // ID does not exist

            // Act
            _addTourModel.RemoveTourCommand.Execute(null);

            // Assert
            Assert.That(_addTourModel.TourLog.TourLogsTable.Count, Is.EqualTo(1));
        }
    }
}
