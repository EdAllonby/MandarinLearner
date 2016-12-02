using System.Windows;
using MandarinLearner.ViewModel;

namespace MandarinLearner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LearnerView
    {
        public LearnerView()
        {
            var viewModel = new LearnerViewModel();
            DataContext = viewModel;
            viewModel.CheckboxChanged += OnCheckboxChanged;

            InitializeComponent();
        }

        private void OnCheckboxChanged(object sender, bool e)
        {
            if (e)
            {
                EnglishPhrase.Visibility = Visibility.Collapsed;
                EnglishTest.Visibility = Visibility.Visible;
            }
            else
            {
                EnglishPhrase.Visibility = Visibility.Visible;
                EnglishTest.Visibility = Visibility.Collapsed;
            }
        }
    }
}