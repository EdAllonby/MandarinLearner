using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.Filter;
using MandarinLearner.ViewModel.RelayCommands;
using MaterialDesignThemes.Wpf;

namespace MandarinLearner.ViewModel
{
    public sealed class LibraryViewModel : LearnerModeViewModel
    {
        private readonly IItemFilter<Phrase> phraseFilter;
        private bool isLoading;
        private string phraseSearchTerm;

        private SearcheableObservableCollection<Phrase> searcheablePhrases;
        private HskPhrase selectedPhrase;

        public LibraryViewModel()
        {
            phraseFilter = new ItemFilter<Phrase>(DisplayAny);
        }

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

        public bool IsLibraryEmpty => !IsLoading && (searcheablePhrases?.DisplayableItems == null || !searcheablePhrases.DisplayableItems.Any());


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
                SearcheablePhrases.Filter(phraseFilter);
            }
        }

        public SearcheableObservableCollection<Phrase> SearcheablePhrases
        {
            get { return searcheablePhrases; }
            set
            {
                searcheablePhrases = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand => new RelayCommand(InitializeAsync);

        public override string Name => "Library";

        public override PackIconKind Icon => PackIconKind.Library;

        private bool DisplayAny(Phrase phrase)
        {
            string[] searchTerms = PhraseSearchTerm.Split(' ');
            string[] pinyinParts = phrase.Pinyin.Split(' ');
            string[] englishParts = phrase.English.Split(' ');
            string[] hanziParts = phrase.Hanzi.Split(' ');

            return searchTerms.All(searchTerm => pinyinParts.Any(p => p.StartsWith(searchTerm)) || englishParts.Any(p => p.StartsWith(searchTerm)) || hanziParts.Any(p => p.StartsWith(searchTerm)));
        }

        private async void InitializeAsync()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                IsLoading = true;
                List<Phrase> loadedPhrases = (await PhraseRepository.GetAllPhrasesFromLevelAsync(6)).ToList();
                SearcheablePhrases = new SearcheableObservableCollection<Phrase>(loadedPhrases);
                IsLoading = false;
            }
        }
    }
}