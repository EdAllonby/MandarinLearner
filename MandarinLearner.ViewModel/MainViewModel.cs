using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            await importer.ImportPhrasesAsync();
        }

        private void Initialize()
        {
            IEnumerable<LearnerModeViewModel> learnerModes = GetEnumerableOfType<LearnerModeViewModel>();
            Modes = new ObservableCollection<LearnerModeViewModel>(learnerModes);
        }


        private static IEnumerable<T> GetEnumerableOfType<T>() where T : class
        {
            Type abstractType = typeof(T);

            return abstractType
                .Assembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(abstractType))
                .Select(type => (T) Activator.CreateInstance(type));
        }
    }
}