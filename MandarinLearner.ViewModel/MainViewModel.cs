using System.Collections.ObjectModel;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class MainViewModel : ViewModel
    {
        private readonly IPhraseImporter importer = new HskPhraseImporter();
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

        public ICommand ImportCommand => new RelayCommand(Import);

        public ICommand LoadedCommand => new RelayCommand(Initialize);

        private async void Import()
        {
            await importer.ImportPhrasesAsync("E:\\Ed\\Documents\\Repositories\\MandarinLearner\\MandarinLearner.ViewModel\\MandarinLearner.ViewModel\\bin\\Debug\\HSK6.csv");
        }

        private void Initialize()
        {
            Modes = new ObservableCollection<LearnerModeViewModel> { new LearnerViewModel(), new LibraryViewModel() };
        }
    }
}