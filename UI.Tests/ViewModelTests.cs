using UI.ViewModels;
using Model;

namespace UI.Tests
{
    [Apartment(ApartmentState.STA)] // required for WPF components (prevents UI-related exceptions)
    public class ViewModelTests
    {
        MainViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new MainViewModel();
        }

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
            _viewModel.DeleteTour(0); // TODO: does not work correctly

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
    }
}