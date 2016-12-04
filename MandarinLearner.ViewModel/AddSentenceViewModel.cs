using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class AddSentenceViewModel : ViewModel
    {
        private readonly PinyinGenerator pinyinGenerator = new PinyinGenerator();
        private bool autoCompletePinyin = true;

        private ObservableCollection<SelectableItem<MeasureWord>> availableMeasureWords;
        private ObservableCollection<SelectableItem<Phrase>> availablePhrases;
        private ObservableCollection<SelectableItem<MeasureWord>> displayableMeasureWords;
        private ObservableCollection<SelectableItem<Phrase>> displayablePhrases;

        private string measureWordSearchTerm = string.Empty;
        private string newSentenceHanzi = string.Empty;
        private string newSentenceEnglish = string.Empty;
        private string newSentencePinyin = string.Empty;
        private string phraseSearchTerm = string.Empty;
        private bool showSelectedMeasureWords;
        private bool showSelectedPhrases;

        public ICommand LoadedCommand => new RelayCommand(LoadAsync);

        public ObservableCollection<SelectableItem<Phrase>> DisplayablePhrases
        {
            get { return displayablePhrases; }
            set
            {
                displayablePhrases = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SelectableItem<MeasureWord>> DisplayableMeasureWords
        {
            get { return displayableMeasureWords; }
            set
            {
                displayableMeasureWords = value;
                OnPropertyChanged();
            }
        }

        public bool ShowSelectedPhrases
        {
            get { return showSelectedPhrases; }
            set
            {
                showSelectedPhrases = value;
                OnPropertyChanged();
                FilterPhrases();
            }
        }

        public bool ShowSelectedMeasureWords
        {
            get { return showSelectedMeasureWords; }
            set
            {
                showSelectedMeasureWords = value;
                OnPropertyChanged();
                FilterMeasureWords();
            }
        }

        public bool AutoCompletePinyin
        {
            get { return autoCompletePinyin; }
            set
            {
                autoCompletePinyin = value;
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

        public string MeasureWordSearchTerm
        {
            get { return measureWordSearchTerm; }
            set
            {
                measureWordSearchTerm = value;
                OnPropertyChanged();
                FilterMeasureWords();
            }
        }

        public string NewSentencePinyin
        {
            get { return newSentencePinyin; }
            set
            {
                newSentencePinyin = value;
                OnPropertyChanged();
            }
        }

        public string NewSentenceHanzi
        {
            get { return newSentenceHanzi; }
            set
            {
                newSentenceHanzi = value;
                OnPropertyChanged();

                if (AutoCompletePinyin)
                {
                    NewSentencePinyin = pinyinGenerator.GetPinyinFromHanzi(newSentenceHanzi);
                }
            }
        }

        public string NewSentenceEnglish
        {
            get { return newSentenceEnglish; }
            set
            {
                newSentenceEnglish = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddSentenceCommand => new RelayCommand(AddSentence, IsSentenceComplete);

        public ICommand FindPhrasesCommand => new RelayCommand(FindPhrases, IsSentenceComplete);

        public ICommand FindMeasureWordsCommand => new RelayCommand(FindMeasureWords, IsSentenceComplete);

        private void FindPhrases()
        {
            foreach (SelectableItem<Phrase> availablePhrase in availablePhrases)
            {
                if (NewSentenceHanzi.Contains(availablePhrase.Item.Hanzi))
                {
                    availablePhrase.IsSelected = true;
                }
            }

            FilterPhrases();
        }

        private void FindMeasureWords()
        {
            foreach (SelectableItem<MeasureWord> availableMeasureWord in availableMeasureWords)
            {
                if (NewSentenceHanzi.Contains(availableMeasureWord.Item.Hanzi))
                {
                    availableMeasureWord.IsSelected = true;
                }
            }

            FilterMeasureWords();
        }

        private void FilterPhrases()
        {
            SelectableItemFilter<Phrase>.Filter(availablePhrases, DisplayablePhrases, ShowSelectedPhrases, ShouldDisplayPhrase);
        }

        private void FilterMeasureWords()
        {
            SelectableItemFilter<MeasureWord>.Filter(availableMeasureWords, DisplayableMeasureWords, ShowSelectedMeasureWords, ShouldDisplayMeasureWord);
        }

        private bool ShouldDisplayPhrase(Phrase phrase)
        {
            string[] searchTerms = PhraseSearchTerm.Split(' ');
            string[] phraseParts = phrase.Pinyin.Split(' ');

            return searchTerms.All(searchTerm => phraseParts.Any(phrasePart => phrasePart.StartsWith(searchTerm)));
        }

        private bool ShouldDisplayMeasureWord(MeasureWord measureWord)
        {
            string[] searchTerms = MeasureWordSearchTerm.Split(' ');
            string[] measureWordPart = measureWord.Pinyin.Split(' ');

            return searchTerms.All(searchTerm => measureWordPart.Any(phrasePart => phrasePart.StartsWith(searchTerm)));
        }

        private bool IsSentenceComplete()
        {
            return !string.IsNullOrWhiteSpace(NewSentencePinyin) && !string.IsNullOrWhiteSpace(NewSentenceEnglish) && !string.IsNullOrWhiteSpace(NewSentenceHanzi);
        }

        private void AddSentence()
        {
            IEnumerable<Phrase> selectedPhrases = FindSelectedItems(availablePhrases);
            IEnumerable<MeasureWord> selectedMeasureWords = FindSelectedItems(availableMeasureWords);

            SentenceMaker.AddSentence(NewSentenceEnglish, NewSentencePinyin, NewSentenceHanzi, selectedPhrases.ToList(), selectedMeasureWords.ToList());
        }

        private static IEnumerable<T> FindSelectedItems<T>(IEnumerable<SelectableItem<T>> selectableItems)
        {
            return selectableItems.Where(item => item.IsSelected).Select(item => item.Item);
        }

        private async void LoadAsync()
        {
            await LoadPhrasesAsync();
            await LoadMeasureWordsAsync();
        }

        private async Task LoadPhrasesAsync()
        {
            IEnumerable<Phrase> loadedPhrases = await PhraseRepository.GetAllPhrasesAsync();

            List<SelectableItem<Phrase>> phrases = loadedPhrases.Select(loadedPhrase => new SelectableItem<Phrase>(loadedPhrase)).ToList();

            availablePhrases = new ObservableCollection<SelectableItem<Phrase>>(phrases);
            DisplayablePhrases = new ObservableCollection<SelectableItem<Phrase>>(phrases);
        }

        private async Task LoadMeasureWordsAsync()
        {
            IEnumerable<MeasureWord> loadedMeasureWords = await MeasureWordRepository.GetAllMeasureWordsAsync();

            List<SelectableItem<MeasureWord>> measureWords = loadedMeasureWords.Select(loadedMeasureWord => new SelectableItem<MeasureWord>(loadedMeasureWord)).ToList();

            availableMeasureWords = new ObservableCollection<SelectableItem<MeasureWord>>(measureWords);
            DisplayableMeasureWords = new ObservableCollection<SelectableItem<MeasureWord>>(measureWords);
        }
    }
}