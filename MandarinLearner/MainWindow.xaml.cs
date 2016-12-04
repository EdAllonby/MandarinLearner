using System.Windows;

namespace MandarinLearner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowAddSentenceView(object sender, RoutedEventArgs e)
        {
            var addSentenceView = new AddSentenceView();
            addSentenceView.ShowDialog();
        }
    }
}