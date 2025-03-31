using UI.ViewModels;
using Model;

namespace SWEN_TourPlanner.Tests
{
    [Apartment(ApartmentState.STA)] // required for WPF components (prevents UI-related exceptions)
    public class ViewModelTests
    {
        MainViewModel _viewModel;
        AddTourModel _addTourModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new MainViewModel();
            _addTourModel = new AddTourModel();
        }

        /* Tour */

        [Test]
        public void SaveTour_ValidData()
        {
            // Arrange
            _viewModel.NewTour.Name = "Tour 1";
            _viewModel.NewTour.From = "Start Location";
            _viewModel.NewTour.To = "End Location";
            _viewModel.NewTour.Transport = "Car";
            _viewModel.NewTour.Distance = "100km";
            _viewModel.NewTour.Description = "A fun trip";
            _viewModel.NewTour.RouteInfo = "Via highway";
            _viewModel.NewTour.EstimatedTime = "2 hours";

            // Act
            _viewModel.SaveTourCommand.Execute(null);

            // Assert
            Assert.AreEqual(1, _viewModel.Tours.Count);
            Assert.AreEqual("Tour 1", _viewModel.Tours.First().Name);
        }

        [Test]
        public void DeleteTour_ValidTourId()
        {
            // Arrange
            AddTourModel tour = new AddTourModel { ID = 0, Name = "Tour 1" };
            _viewModel.Tours.Add(tour);
            _viewModel.Blocks.Add(new BlockModel { TourID = tour.ID } );

            // Act
            _viewModel.DeleteTour(0); // funktioniert aktuell nicht richtig

            // Assert
            Assert.IsEmpty(_viewModel.Tours);
        }

        [Test]
        public void DeleteTour_InvalidTourId()
        {
            // Arrange
            var tour = new AddTourModel { ID = 0, Name = "Tour 1" };
            _viewModel.Tours.Add(tour);

            // Act
            _viewModel.DeleteTour(5); // ID 5 does not exist

            // Assert
            Assert.AreEqual(1, _viewModel.Tours.Count);
            Assert.AreEqual(tour.ID, _viewModel.Tours[0].ID);
            Assert.AreEqual(tour.Name, _viewModel.Tours[0].Name);
        }

        // TODO: ModifyTour

        /* Tour log */

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