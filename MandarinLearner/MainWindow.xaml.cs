using System.Windows;
using System.Windows.Input;
using MandarinLearner.ViewModel;

namespace MandarinLearner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
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

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}