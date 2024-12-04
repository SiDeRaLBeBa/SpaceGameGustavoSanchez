using System.Windows;

namespace SpaceGame
{
    public partial class GameOverDialog : Window
    {
        public bool PlayAgain { get; private set; }

        public GameOverDialog(int score, int timeSurvived)
        {
            InitializeComponent(); // Ensures XAML components are initialized

            ScoreText.Text = $"Score: {score}";
            TimeText.Text = $"Time Survived: {timeSurvived}s";
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            PlayAgain = false;  
            DialogResult = true;
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            PlayAgain = true;
            DialogResult = true;
        }
    }
}
