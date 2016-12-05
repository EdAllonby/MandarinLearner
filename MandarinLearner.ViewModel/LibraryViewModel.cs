using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.RelayCommands;
using MaterialDesignThemes.Wpf;

namespace MandarinLearner.ViewModel
{
    public sealed class LibraryViewModel : LearnerModeViewModel
    {
        private bool isLoading;
        private ObservableCollection<Phrase> displayablePhrases;
        private ObservableCollection<Phrase> availablePhrases;

        private HskPhrase selectedPhrase;
        private string phraseSearchTerm;

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

        public bool IsLibraryEmpty => !IsLoading && (displayablePhrases == null || !displayablePhrases.Any());


        public ObservableCollection<Phrase> DisplayablePhrases
        {
            get { return displayablePhrases; }
            set
            {
                displayablePhrases = value;
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

        public string PhraseSearchTerm
        {
            get { return phraseSearchTerm; }
            set
            {
                phraseSearchTerm = value;
                OnPropertyChanged();
                FilterPhrases();
            }
        }

        private void FilterPhrases()
        {
            ItemFilter<Phrase>.Filter(availablePhrases, DisplayablePhrases, ShouldDisplayPhrase);
        }

        private bool ShouldDisplayPhrase(Phrase phrase)
        {
            string[] searchTerms = PhraseSearchTerm.Split(' ');
            string[] pinyinParts = phrase.Pinyin.Split(' ');
            string[] englishParts = phrase.English.Split(' ');

            return searchTerms.All(searchTerm => pinyinParts.Any(p => p.StartsWith(searchTerm)) || englishParts.Any(p => p.StartsWith(searchTerm)));
        }

        public ICommand LoadedCommand => new RelayCommand(InitializeAsync);

        public override string Name => "Library";

        public override PackIconKind Icon => PackIconKind.Library;

        private async void InitializeAsync()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                IsLoading = true;

                List<Phrase> loadedPhrases = (await PhraseRepository.GetAllPhrasesFromLevelAsync(6)).ToList();

                availablePhrases = new ObservableCollection<Phrase>(loadedPhrases);
                DisplayablePhrases = new ObservableCollection<Phrase>(loadedPhrases);

                IsLoading = false;
            }
        }
    }
}