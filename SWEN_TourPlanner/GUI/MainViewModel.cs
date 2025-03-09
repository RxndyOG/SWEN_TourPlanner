using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;

namespace SWEN_TourPlanner
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _output = "Hello World!";
        private string? _input;
        public ObservableCollection<TableRow> TableData { get; set; }

        public string? Input
        {
            get
            {
                Debug.Print("read Input");
                return _input;
            }
            set
            {
                Debug.Print("write Input");
                if (Input != value)
                {
                    Debug.Print("set Input-value");
                    _input = value;

                    // it does not work to fire an event from outside in C#
                    // can be achieved by creating a method like "RaiseCanExecuteChanged".
                    // this.ExecuteCommand.CanExecuteChanged?.Invoke(this, EventArgs.Empty);

                    // this triggers the UI and the ExecuteCommand
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        public string Output
        {
            get
            {
                Debug.Print("read Output");
                return _output;
            }
            set
            {
                Debug.Print("write Output");
                if (_output != value)
                {
                    Debug.Print("set Output");
                    _output = value;
                    Debug.Print("fire propertyChanged: Output");
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ExecuteCommand { get; }

        //public event PropertyChangedEventHandler PropertyChanged;
        private PropertyChangedEventHandler? _propertyChangedEventHandler;
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                Debug.Print($"added PropertyChanged-handler {value}");
                _propertyChangedEventHandler += value;
            }
            remove
            {
                Debug.Print($"removed PropertyChanged-handler {value}");
                _propertyChangedEventHandler -= value;
            }
        }

        public class TableRow
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
        }

        public MainViewModel()
        {
            Debug.Print("ctor MainViewModel");
            this.ExecuteCommand = new ExecuteCommand(this);
            TableData = new ObservableCollection<TableRow>
            {
                new TableRow { Column1 = "Wert 1", Column2 = "Wert A", Column3 = "Wert X" },
                new TableRow { Column1 = "Wert 2", Column2 = "Wert B", Column3 = "Wert Y" }
                
            };

            #region Simpler Solution

            // Alternative: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            /*
            this.ExecuteCommand = new RelayCommand((_) => {
                Output = $"Hello {Input}!";
                Input = string.Empty;
            }, (_) => !string.IsNullOrWhiteSpace(Input));
            */

            #endregion
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
