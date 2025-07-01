using UI.ViewModels;
using Model;

namespace UI.Tests
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class ViewModelTests
    {
        [Test]
        public void MainViewModel_PropertyChanged_IsRaised()
        {
            var vm = new MainViewModel();
            bool raised = false;
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == "SearchText") raised = true; };
            vm.SearchText = "Test";
            Assert.IsTrue(raised);
        }

        [Test]
        public void MainViewModel_FilteredBlocks_Refreshes()
        {
            var vm = new MainViewModel();
            vm.SearchText = "abc";
            Assert.IsNotNull(vm.FilteredBlocks);
        }

        [Test]
        public void TourManagementViewModel_NewTour_PropertyChanged()
        {
            var main = new MainViewModel();
            var vm = new TourManagementViewModel(new NavigationViewModel(main), main);
            bool raised = false;
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == "NewTour") raised = true; };
            vm.NewTour = new AddTourModel();
            Assert.IsTrue(raised);
        }

        [Test]
        public void TourManagementViewModel_SaveTour_AddsTour()
        {
            var main = new MainViewModel();
            var vm = new TourManagementViewModel(new NavigationViewModel(main), main);
            vm.NewTour.Name = "Test";
            vm.NewTour.From = "A";
            vm.NewTour.To = "B";
            vm.NewTour.Transport = "Car";
            vm.NewTour.Distance = "10";
            vm.NewTour.Description = "Desc";
            vm.NewTour.RouteInfo = "Route";
            vm.NewTour.EstimatedTime = "1h";
            int before = vm.Tours.Count;
            vm.SaveTourCommand.Execute(null);
            Assert.That(vm.Tours.Count, Is.EqualTo(before + 1));
        }

        [Test]
        public void AddTourModel_ImagePath_PropertyChanged()
        {
            var model = new AddTourModel();
            bool raised = false;
            model.PropertyChanged += (s, e) => { if (e.PropertyName == "ImagePath") raised = true; };
            model.ImagePath = "test.png";
            Assert.IsTrue(raised);
        }
    }
}