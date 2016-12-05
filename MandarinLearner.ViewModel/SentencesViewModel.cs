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
    public sealed class SentencesViewModel : LearnerModeViewModel
    {
        private bool isLoading;
        private ObservableCollection<Sentence> sentences;

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

        public bool IsLibraryEmpty => !IsLoading && (sentences == null || !sentences.Any());


        public ObservableCollection<Sentence> Sentences
        {
            get { return sentences; }
            set
            {
                sentences = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadedCommand => new RelayCommand(InitializeAsync);

        public override string Name => "Sentences";

        public override PackIconKind Icon => PackIconKind.Message;

        private async void InitializeAsync()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Task<IEnumerable<Sentence>> loadedSentences = SentenceRepository.GetAllSentencesAsync();
                IsLoading = true;
                Sentences = new ObservableCollection<Sentence>((await loadedSentences).ToList());
                IsLoading = false;
            }
        }
    }
}