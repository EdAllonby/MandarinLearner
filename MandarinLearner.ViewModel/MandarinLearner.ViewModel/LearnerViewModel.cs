using System;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class LearnerViewModel : ViewModel
    {
        readonly PhraseRepository phraseRepository = new PhraseRepository();

        private Phrase currentPhrase;

        private bool isCheckEnglishOption;

        private string updatedEnglishAnswer = string.Empty;
        private Points points = new Points();
        private string continueLabel;

        public event EventHandler<bool> CheckboxChanged; 

        public LearnerViewModel()
        {
            phraseRepository.LoadAll();
            TryGenerateNextPhrase();
            UpdateContinueLabel();
        }
        public ICommand NextPhrase
        {
            get { return new RelayCommand(TryGenerateNextPhrase, IsButtonEnabled); }
        }

        public string ContinueLabel
        {
            get { return continueLabel; }
            set
            {
                continueLabel = value;
                OnPropertyChanged();
            }
        }

        public bool IsCheckEnglishOption
        {
            get { return isCheckEnglishOption; }
            set
            {
                isCheckEnglishOption = value;

                UpdateContinueLabel();
                
                if (CheckboxChanged != null)
                {
                    CheckboxChanged(this, IsCheckEnglishOption);
                }

                OnPropertyChanged();
            }
        }

        public string UpdatedEnglishAnswer
        {
            get { return updatedEnglishAnswer; }
            set
            {
                updatedEnglishAnswer = value;
                EnglishAnswerValidator();
            }
        }

        public Phrase CurrentPhrase
        {
            get { return currentPhrase; }
            set
            {
                currentPhrase = value;
                OnPropertyChanged();
            }
        }

        public Points Points
        {
            get { return points; }
            set
            {
                points = value;
                OnPropertyChanged();
            }
        }

        private static bool IsButtonEnabled()
        {
            return true;
        }

        private void TryGenerateNextPhrase()
        {
            if (IsCheckEnglishOption)
            {
                if (EnglishAnswerValidator())
                {
                    CurrentPhrase = phraseRepository.GetRandomPhrase();       
                    points.AnsweredCorrect();
                    SendPointsUpdate();
                }
                else
                {
                    points.AnseredIncorrect();
                    SendPointsUpdate();
                }
            }
            else
            {
                CurrentPhrase = phraseRepository.GetRandomPhrase();                
            }
        }

        private bool EnglishAnswerValidator()
        {
            return UpdatedEnglishAnswer.ToLower().Equals(CurrentPhrase.EnglishPhrase.ToLower());
        }

        private void SendPointsUpdate()
        {
            OnPropertyChanged("Points");
        }

        private void UpdateContinueLabel()
        {
            ContinueLabel = IsCheckEnglishOption ? "Check" : "Next";
        }
    }
}