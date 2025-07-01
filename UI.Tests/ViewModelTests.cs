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

    }
}