using SWEN_TourPlanner.GUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SWEN_TourPlanner
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _output = "Hello World!";
        private string? _input;
        public ObservableCollection<TableRow> TableData { get; set; }
        public ObservableCollection<BlockModel> Blocks { get; set; } = new();

        private string _newBlockText;
        public string NewBlockText
        {
            get => _newBlockText;
            set
            {
                _newBlockText = value;
                OnPropertyChanged();
            }
        }

        private Page _currentPage;

        public Page CurrentPageMiddle
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPageMiddle));
            }
        }

        private Page _currentPageRight;

        public Page CurrentPageRight
        {
            get => _currentPageRight;
            set
            {
                _currentPageRight = value;
                OnPropertyChanged(nameof(CurrentPageRight));
            }
        }


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
        public ICommand NavigateCommand { get; }
        public ICommand NavigateCommandRight { get; }
        public ICommand AddBlockCommand { get; }
        public ICommand RemoveBlockCommand { get; }

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

            NavigateCommand = new RelayCommand(Navigate);
            NavigateCommandRight = new RelayCommand(NavigateRight);

            this.ExecuteCommand = new ExecuteCommand(this);

            AddBlockCommand = new RelayCommand(_ => AddBlock());
            RemoveBlockCommand = new RelayCommand(RemoveBlock);

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

        private void AddBlock()
        {
            if (string.IsNullOrWhiteSpace(NewBlockText))
            {
                MessageBox.Show("Bitte einen Text eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newBlock = new BlockModel
            {
                Text = NewBlockText,
                RemoveCommand = RemoveBlockCommand
            };

            Blocks.Add(newBlock);
            NewBlockText = "";  // Eingabefeld zurücksetzen
        }

        private void RemoveBlock(object block)
        {
            if (block is BlockModel blockModel && Blocks.Contains(blockModel))
            {
                Blocks.Remove(blockModel);
            }
        }

        private void Navigate(object parameter)
        {
            if (parameter is string pageName)
            {
                switch (pageName)
                {
                    case "Page1":
                        CurrentPageMiddle = new MainPageMiddleHome();
                        break;
                    case "Page2":
                        CurrentPageMiddle = new secondPageMiddleSearch();
                        break;
                }
            }
        }

        private void NavigateRight(object parameter)
        {
            if (parameter is string pageName)
            {
                switch (pageName)
                {
                    case "Page1":
                        CurrentPageRight = new MainPageMiddleHome();
                        break;
                    case "Page2":
                        CurrentPageRight = new secondPageMiddleSearch();
                        break;
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
