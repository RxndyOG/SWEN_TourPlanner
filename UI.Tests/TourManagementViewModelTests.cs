using UI.ViewModels;
using NUnit.Framework;
using System.Linq;

namespace UI.Tests
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class TourManagementViewModelTests
    {
        private MainViewModel _mainViewModel;
        private NavigationViewModel _navigationViewModel;
        private TourManagementViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _mainViewModel = new MainViewModel();
            _navigationViewModel = new NavigationViewModel(_mainViewModel);
            _viewModel = new TourManagementViewModel(_navigationViewModel, _mainViewModel);
        }

        [Test]
        public void SaveTourCommand_AddsTourToCollection()
        {
            _viewModel.NewTour.Name = "Test Tour";
            _viewModel.NewTour.From_Location = "Vienna";
            _viewModel.NewTour.To_Location = "Graz";
            _viewModel.NewTour.Transportation_Type = "Car";
            _viewModel.NewTour.Distance = 200;
            _viewModel.NewTour.Estimated_Time = 120;
            _viewModel.NewTour.Description = "A test tour";
            _viewModel.NewTour.Route_Information = "Route details";

            int initialCount = _viewModel.Tours.Count;

            _viewModel.SaveTourCommand.Execute(null);

            Assert.That(_viewModel.Tours.Count, Is.EqualTo(initialCount + 1));
            Assert.That(_viewModel.Tours.Last().Name, Is.EqualTo("Test Tour"));
        }

        [Test]
        public void DeleteTour_RemovesTourFromCollection()
        {
            var tour = new AddTourModel
            {
                Id = 1,
                Name = "Tour to Delete",
                From_Location = "Vienna",
                To_Location = "Salzburg",
                Transportation_Type = "Train",
                Distance = 300,
                Estimated_Time = 180,
                Description = "A tour to be deleted",
                Route_Information = "Route details"
            };
            _viewModel.Tours.Add(tour);

            int initialCount = _viewModel.Tours.Count;

            _viewModel.DeleteTour(tour.Id);

            Assert.That(_viewModel.Tours.Count, Is.EqualTo(initialCount - 1));
            Assert.That(_viewModel.Tours.Any(t => t.Id == tour.Id), Is.False);
        }


    }
}