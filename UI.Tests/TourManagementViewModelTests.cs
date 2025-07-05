using UI.ViewModels;

namespace UI.Tests;

    [TestFixture]
    public class TourManagementViewModelTests
    {
        private MainViewModel _main;
        private NavigationViewModel _nav;
        private TourManagementViewModel _vm;

        [SetUp]
        public void Setup()
        {
            _main = new MainViewModel();
            _nav = new NavigationViewModel(_main);
            _vm = new TourManagementViewModel(_nav, _main);
        }

        [Test]
        public void SaveTourCommand_AddsTour()
        {
            _vm.NewTour.Name = "Test";
            _vm.NewTour.From_Location = "A";
            _vm.NewTour.To_Location = "B";
            _vm.NewTour.Transportation_Type = "Car";
            _vm.NewTour.Distance = 10;
            _vm.NewTour.Description = "Desc";
            _vm.NewTour.Route_Information = "Route";
            _vm.NewTour.Estimated_Time = 60;
            int before = _vm.Tours.Count;

            _vm.SaveTourCommand.Execute(null);

            Assert.That(_vm.Tours.Count, Is.EqualTo(before + 1));
            Assert.That(_vm.Tours.Last().Name, Is.EqualTo("Test"));
        }


        [Test]
        public void AddTour_AddsBlock()
        {
            var tour = new AddTourModel { Id = 42, Name = "BlockTest", Description = "BlockDesc" };
            int before = _vm.Blocks.Count;
            var method = _vm.GetType().GetMethod("AddTour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_vm, new object[] { tour });
            Assert.That(_vm.Blocks.Count, Is.EqualTo(before + 1));
            Assert.That(_vm.Blocks.Last().Text, Is.EqualTo("BlockTest"));
        }

        [Test]
        public void RemoveBlock_RemovesBlock()
        {
            var tour = new AddTourModel { Id = 99, Name = "RemoveTest", Description = "Desc" };
            var method = _vm.GetType().GetMethod("AddTour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_vm, new object[] { tour });
            var block = _vm.Blocks.Last();
            _vm.RemoveBlockCommand.Execute(block);
            Assert.That(_vm.Blocks.Any(b => b.TourID == 99), Is.False);
        }
    }

