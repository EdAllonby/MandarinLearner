using System;
using System.Windows.Input;
using MandarinLearner.Model;
using MandarinLearner.ViewModel.RelayCommands;

namespace MandarinLearner.ViewModel
{
    public sealed class LearnerViewModel : LearnerModeViewModel
    {
        private string continueLabel;

        private Phrase currentPhrase;

        private bool isCheckEnglishOption;
        private Points points = new Points();

        private string updatedEnglishAnswer = string.Empty;

        public LearnerViewModel()
        {
            TryGenerateNextPhrase();
            UpdateContinueLabel();
        }

        public ICommand NextPhrase => new RelayCommand(TryGenerateNextPhrase, IsButtonEnabled);

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

        public override string Name => "Learner";

        public event EventHandler<bool> CheckboxChanged;

        private static bool IsButtonEnabled()
        {
            return true;
        }

        private async void TryGenerateNextPhrase()
        {
            if (IsCheckEnglishOption)
            {
                if (EnglishAnswerValidator())
                {
                    CurrentPhrase = await PhraseRepository.GetRandomHskPhraseByLevel(1);
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
                CurrentPhrase = await PhraseRepository.GetRandomHskPhraseByLevel(1);
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