using MaterialDesignThemes.Wpf;

namespace MandarinLearner.ViewModel
{
    public abstract class LearnerModeViewModel : ViewModel
    {
        public abstract string Name { get; }
        public abstract PackIconKind Icon { get; }
    }
}