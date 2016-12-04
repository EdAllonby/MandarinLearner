using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class LibraryViewModel : LearnerModeViewModel
    {
        private bool isLoading;
        private IEnumerable<Phrase> phrases;
        private HskPhrase selectedPhrase;

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLibraryEmpty));
            }
        }

        public bool IsLibraryEmpty => !IsLoading && (phrases == null || !phrases.Any());


        public IEnumerable<Phrase> Phrases
        {
            get { return phrases; }
            set
            {
                phrases = value;
                OnPropertyChanged();
            }
        }

        public HskPhrase SelectedPhrase
        {
            get { return selectedPhrase; }
            set
            {
                selectedPhrase = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand => new RelayCommand(InitializeAsync);

        public override string Name => "Library";

        private async void InitializeAsync()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Task<IEnumerable<Phrase>> loadedPhrases = PhraseRepository.GetAllPhrasesFromLevelAsync(6);
                IsLoading = true;
                Phrases = await loadedPhrases;
                IsLoading = false;
            }
        }
    }
}