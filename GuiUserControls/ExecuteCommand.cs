using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SWEN_TourPlanner
{
    public class ExecuteCommand : ICommand
    {
        private readonly MainViewModel _mainViewModel;

        public ExecuteCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += (sender, args) =>
            {
                Debug.Print("command: reveived prop changed");
                if (args.PropertyName == "Input")
                {
                    Debug.Print("command: reveived prop changed of Input");
                    //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    _canExecuteChangedEventHandler?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public bool CanExecute(object? parameter)
        {
            Debug.Print("command: can execute");
            return !string.IsNullOrWhiteSpace(_mainViewModel.Input);
        }

        public void Execute(object? parameter)
        {
            Debug.Print("command: execute");
            _mainViewModel.Output = $"Hello {_mainViewModel.Input}!";
            _mainViewModel.Input = string.Empty;
            Debug.Print("command: execute done");
        }

        //public event EventHandler? CanExecuteChanged;
        private EventHandler? _canExecuteChangedEventHandler;
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                Debug.Print($"added CanExecuteChanged-handler {value}");
                _canExecuteChangedEventHandler += value;
            }
            remove
            {
                Debug.Print($"removed CanExecuteChanged-handler {value}");
                _canExecuteChangedEventHandler -= value;
            }
        }
    }
}
