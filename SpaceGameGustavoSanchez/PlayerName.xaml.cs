using System.Windows;

namespace SpaceGame
{
    public partial class PlayerName : Window
    {
        public string PlayerNameInput { get; private set; }

        public PlayerName()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (!string.IsNullOrWhiteSpace(PlayerNameTextBox.Text))
            {
                PlayerNameInput = PlayerNameTextBox.Text;
                DialogResult = true; // Close the dialog and return true
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Close the dialog and return false
            Close();
        }
    }
}
