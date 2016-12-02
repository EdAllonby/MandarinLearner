using System.Collections.ObjectModel;
using System.Windows.Input;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class MainViewModel : ViewModel
    {
        private ObservableCollection<LearnerModeViewModel> modes;
        private LearnerModeViewModel selectedMode;

        public ObservableCollection<LearnerModeViewModel> Modes
        {
            get { return modes; }
            set
            {
                modes = value;
                OnPropertyChanged();
            }
        }

        public LearnerModeViewModel SelectedMode
        {
            get { return selectedMode; }
            set
            {
                selectedMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand => new RelayCommand(Initialize);

        private void Initialize()
        {
            Modes = new ObservableCollection<LearnerModeViewModel>();
            Modes.Add(new LearnerViewModel());
            Modes.Add(new LibraryViewModel());
        }
    }
}