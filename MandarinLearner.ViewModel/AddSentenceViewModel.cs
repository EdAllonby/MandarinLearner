using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.Filter;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class AddSentenceViewModel : ViewModel
    {
        private readonly PinyinGenerator pinyinGenerator = new PinyinGenerator();

        private bool autoCompletePinyin = true;
        private string measureWordSearchTerm = string.Empty;
        private string newSentenceEnglish = string.Empty;
        private string newSentenceHanzi = string.Empty;
        private string newSentencePinyin = string.Empty;
        private string phraseSearchTerm = string.Empty;

        private SearcheableObservableCollection<SelectableItem<MeasureWord>> searcheableMeasureWords;
        private SearcheableObservableCollection<SelectableItem<Phrase>> searcheablePhrases;
        private bool showSelectedMeasureWords;
        private bool showSelectedPhrases;

        public ICommand LoadedCommand => new RelayCommand(LoadAsync);

        public SearcheableObservableCollection<SelectableItem<Phrase>> SearcheablePhrases
        {
            get { return searcheablePhrases; }
            set
            {
                searcheablePhrases = value;
                OnPropertyChanged();
            }
        }

        public SearcheableObservableCollection<SelectableItem<MeasureWord>> SearcheableMeasureWords
        {
            get { return searcheableMeasureWords; }
            set
            {
                searcheableMeasureWords = value;
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

        public ICommand ClearSelectedPhrasesCommand => new RelayCommand(ClearSelectedPhrases, CanClearSelectedPhrases);

        private bool CanClearSelectedPhrases()
        {
            return (SearcheablePhrases?.AnyAvailable(x => x.IsSelected)).GetValueOrDefault();
        }

        private void ClearSelectedPhrases()
        {
            SearcheablePhrases.ApplyToAll(phrase => phrase.IsSelected = false);
            ShowSelectedPhrases = false;
        }

        private void FindPhrases()
        {
            SearcheablePhrases.ApplyToAll(FindHanziFromPhrase);
            FilterPhrases();
        }

        private void FindHanziFromPhrase(SelectableItem<Phrase> availablePhrase)
        {
            if (NewSentenceHanzi.Contains(availablePhrase.Item.Hanzi))
            {
                availablePhrase.IsSelected = true;
            }
        }

        private void FindMeasureWords()
        {
            SearcheableMeasureWords.ApplyToAll(FindHanziFromMeasureWord);
            FilterMeasureWords();
        }

        private void FindHanziFromMeasureWord(SelectableItem<MeasureWord> availableMeasureWord)
        {
            if (NewSentenceHanzi.Contains(availableMeasureWord.Item.Hanzi))
            {
                availableMeasureWord.IsSelected = true;
            }
        }

        private void FilterPhrases()
        {
            SearcheablePhrases.Filter(new SelectableItemFilter<Phrase>(ShowSelectedPhrases, ShouldDisplayPhrase));
        }

        private void FilterMeasureWords()
        {
            SearcheableMeasureWords.Filter(new SelectableItemFilter<MeasureWord>(ShowSelectedMeasureWords, ShouldDisplayMeasureWord));
        }

        private bool ShouldDisplayPhrase(SelectableItem<Phrase> phrase)
        {
            string[] searchTerms = PhraseSearchTerm.Split(' ');
            string[] phraseParts = phrase.Item.Pinyin.Split(' ');

            return searchTerms.All(searchTerm => phraseParts.Any(phrasePart => phrasePart.StartsWith(searchTerm)));
        }

        private bool ShouldDisplayMeasureWord(SelectableItem<MeasureWord> measureWord)
        {
            string[] searchTerms = MeasureWordSearchTerm.Split(' ');
            string[] measureWordPart = measureWord.Item.Pinyin.Split(' ');

            return searchTerms.All(searchTerm => measureWordPart.Any(phrasePart => phrasePart.StartsWith(searchTerm)));
        }

        private bool IsSentenceComplete()
        {
            return !string.IsNullOrWhiteSpace(NewSentencePinyin) && !string.IsNullOrWhiteSpace(NewSentenceEnglish) && !string.IsNullOrWhiteSpace(NewSentenceHanzi);
        }

        private void AddSentence()
        {
            IEnumerable<Phrase> selectedPhrases = SearcheablePhrases.FindAvailable(item => item.IsSelected).Select(item => item.Item);
            IEnumerable<MeasureWord> selectedMeasureWords = SearcheableMeasureWords.FindAvailable(item => item.IsSelected).Select(item => item.Item);

            SentenceMaker.AddSentence(NewSentenceEnglish, NewSentencePinyin, NewSentenceHanzi, selectedPhrases.ToList(), selectedMeasureWords.ToList());
        }

        private async void LoadAsync()
        {
            IEnumerable<Phrase> loadedPhrases = await PhraseRepository.GetAllPhrasesAsync();
            IEnumerable<SelectableItem<Phrase>> phrases = loadedPhrases.Select(loadedPhrase => new SelectableItem<Phrase>(loadedPhrase));
            SearcheablePhrases = new SearcheableObservableCollection<SelectableItem<Phrase>>(phrases);

            IEnumerable<MeasureWord> loadedMeasureWords = await MeasureWordRepository.GetAllMeasureWordsAsync();
            IEnumerable<SelectableItem<MeasureWord>> measureWords = loadedMeasureWords.Select(loadedMeasureWord => new SelectableItem<MeasureWord>(loadedMeasureWord));
            SearcheableMeasureWords = new SearcheableObservableCollection<SelectableItem<MeasureWord>>(measureWords);
        }
    }
}