using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SpaceGame
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer countdownTimer = new DispatcherTimer();
        private DispatcherTimer starsTimer = new DispatcherTimer();  // New timer for stars movement

        private MediaPlayer laserSound = new MediaPlayer(); // Declare at class level
        private int countdownTimeLeft = 3; // Countdown from 3 seconds
        private bool gameStarted = false; // Track if the game has started

        private DispatcherTimer elapsedTimeTimer = new DispatcherTimer();
        private int elapsedTimeInSeconds = 0; // Time elapsed since the game started

        Dictionary<Image, string> enemyOriginalImages = new Dictionary<Image, string>();


        private ObservableCollection<PlayerStatistics> playerStatistics;

        private string playerName; // Field to store the player's name



        public MainWindow()
        {
            InitializeComponent();
            GameCanvas.Visibility = Visibility.Hidden; // Hide game canvas initially
            StartMenu.Visibility = Visibility.Visible; // Show main menu

            GameCanvas.Focus(); // Ensures the canvas is ready to receive key events

            gameTimer.Interval = TimeSpan.FromMilliseconds(20); // 50 FPS
            gameTimer.Tick += GameTimer_Tick;

            countdownTimer.Interval = TimeSpan.FromSeconds(1); // 1 second interval
            countdownTimer.Tick += CountdownTimer_Tick;

            starsTimer.Interval = TimeSpan.FromMilliseconds(50); // 50 milliseconds for smooth movement
            starsTimer.Tick += StarsTimer_Tick;  // Call Stars method on each tick

            elapsedTimeTimer.Interval = TimeSpan.FromSeconds(1); // Tick every second
            elapsedTimeTimer.Tick += ElapsedTimeTimer_Tick; // Hook up the event handler

            playerStatistics = new ObservableCollection<PlayerStatistics>();
            StatisticsDataGrid.ItemsSource = playerStatistics; // Bind to DataGrid


            enemyOriginalImages[Rock1] = "pack://application:,,,/Resources/image_7.png";
            enemyOriginalImages[Rock2] = "pack://application:,,,/Resources/image_8.png";
            enemyOriginalImages[e_4] = "pack://application:,,,/Resources/image_11.png";
            enemyOriginalImages[e_3] = "pack://application:,,,/Resources/image_3.png";
            enemyOriginalImages[e_1] = "pack://application:,,,/Resources/image_1.png";
            enemyOriginalImages[e_2] = "pack://application:,,,/Resources/image_2.png";
            enemyOriginalImages[e_1_bullet] = "pack://application:,,,/Resources/image_5.png";
            enemyOriginalImages[e_2_bullet] = "pack://application:,,,/Resources/image_5.png";
            enemyOriginalImages[e_3_bullet] = "pack://application:,,,/Resources/image_5.png";
            enemyOriginalImages[e_4_bullet] = "pack://application:,,,/Resources/image_5.png";

            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "LaserSound.wav");
            if (System.IO.File.Exists(soundFilePath))
            {
                laserSound.Open(new Uri(soundFilePath, UriKind.Absolute));
            }
            else
            {
                Debug.WriteLine("Laser sound file not found: " + soundFilePath);
            }

            starsTimer.Start();

        }

        // Main menu - start button click
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the PlayerName dialog
            PlayerName playerNameDialog = new PlayerName();

            // Show the dialog and wait for the user's input
            bool? result = playerNameDialog.ShowDialog();

            // Check if the dialog was closed with the OK button
            if (result == true)
            {
                // Retrieve the player's name from the dialog (assume PlayerName has a property for this)
                playerName = playerNameDialog.PlayerNameInput;

                // Proceed with game initialization
                StartMenu.Visibility = Visibility.Hidden;
                GameCanvas.Visibility = Visibility.Visible;

                TimerLabel.Visibility = Visibility.Visible;

                RandomizeInitialPositions();

                countdownTimer.Start();
                countdownTimeLeft = 3; // Start from 3 seconds
                TimerLabel.Content = countdownTimeLeft.ToString();
            }
            else
            {
                // If the user canceled, you can handle it here (e.g., return to the menu or show a message)
                MessageBox.Show("Player name is required to start the game.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            StartMenu.Visibility = Visibility.Collapsed;
            StatisticsLayer.Visibility = Visibility.Visible;
            // Populate the statistics table
        }


        // Countdown timer logic
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTimeLeft--; // Decrease the time

            // Update the timer label
            TimerLabel.Content = countdownTimeLeft.ToString();

            if (countdownTimeLeft == 0)
            {
                countdownTimer.Stop(); // Stop the countdown timer
                TimerLabel.Visibility = Visibility.Hidden; // Hide the countdown label

                // Start the game logic here after the countdown ends
                StartGame();
            }
        }
        void StarsMainMenu()
        {
            foreach (UIElement x in starsMenu.Children)  // Use StartMenu.Children instead of GameCanvas
            {
                if (x is Image && (x as Image).Tag?.ToString() == "starsMainMenu")  // Correct the tag name
                {
                    double left = Canvas.GetLeft(x);
                    Canvas.SetLeft(x, left - 5);  // Move star left by 5 pixels

                    if (left < 0)
                    {
                        Canvas.SetLeft(x, 800);  // Reset star to the right side
                    }
                }
            }
        }

        private void StarsTimer_Tick(object sender, EventArgs e)
        {
            StarsMainMenu(); 
            StarsStatisticsvoid();
        }

        //Statics Logic

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the Main Layer and Hide the Statistics Layer
            StartMenu.Visibility = Visibility.Visible;
            StatisticsLayer.Visibility = Visibility.Collapsed;
        }

        void StarsStatisticsvoid()
        {
            foreach (UIElement x in StarsStatisticsContainer.Children)  // Use StartMenu.Children instead of GameCanvas
            {
                if (x is Image && (x as Image).Tag?.ToString() == "StarsStatistics")  // Correct the tag name
                {
                    double left = Canvas.GetLeft(x);
                    Canvas.SetLeft(x, left - 5);  // Move star left by 5 pixels

                    if (left < 0)
                    {
                        Canvas.SetLeft(x, 800);  // Reset star to the right side
                    }
                }
            }
        }


        public class PlayerStatistics
        {
            public int Rank { get; set; }
            public string PlayerName { get; set; }
            public int Score { get; set; }
            public int Time { get; set; }
        }



        private void AddPlayerStatistics(string playerName, int score, int time)
        {
            // Add the player's result to the ObservableCollection
            playerStatistics.Add(new PlayerStatistics
            {
                PlayerName = playerName,
                Score = score,
                Time = time
            });

            // Sort the collection by score in descending order and update the rank
            var sortedStatistics = playerStatistics
                .OrderByDescending(p => p.Score)
                .Select((p, index) => new PlayerStatistics
                {
                    Rank = index + 1,
                    PlayerName = p.PlayerName,
                    Score = p.Score,
                    Time = p.Time
                })
                .ToList();

            // Refresh the ObservableCollection
            playerStatistics.Clear();
            foreach (var stat in sortedStatistics)
            {
                playerStatistics.Add(stat);
            }
        }




        // Main Game logic
        int score, kill;

        // This method starts the game after the countdown ends
        private void StartGame()
        {


            gameStarted = true; // Mark the game as started
                                // Initialize elapsed time

            elapsedTimeInSeconds = 0; // Reset elapsed time
            elapsedTimeTimer.Start(); // Start the elapsed time timer
            gameTimer.Start();


        }
        private void RandomizeInitialPositions()
        {
            Random rnd = new Random();

            // Randomize enemies
            Canvas.SetLeft(e_1, rnd.Next(899, 900)); // Randomize within the right side of the screen
            Canvas.SetTop(e_1, rnd.Next(0, 150));

            Canvas.SetLeft(e_2, rnd.Next(899, 900));
            Canvas.SetTop(e_2, rnd.Next(50, 250));

            Canvas.SetLeft(e_3, rnd.Next(899, 900));
            Canvas.SetTop(e_3, rnd.Next(250, 350));

            Canvas.SetLeft(e_4, rnd.Next(899, 900));
            Canvas.SetTop(e_4, rnd.Next(250, 400));

            // Randomize rocks
            Canvas.SetLeft(Rock1, rnd.Next(899, 900));
            Canvas.SetTop(Rock1, rnd.Next(0, 400));

            Canvas.SetLeft(Rock2, rnd.Next(899, 900));
            Canvas.SetTop(Rock2, rnd.Next(0, 500));

            // Randomize bullets (optional, if they have specific starting positions)
            Canvas.SetLeft(e_1_bullet, Canvas.GetLeft(e_1));
            Canvas.SetTop(e_1_bullet, Canvas.GetTop(e_1) + 25);

            Canvas.SetLeft(e_2_bullet, Canvas.GetLeft(e_2));
            Canvas.SetTop(e_2_bullet, Canvas.GetTop(e_2) + 25);

            Canvas.SetLeft(e_3_bullet, Canvas.GetLeft(e_3));
            Canvas.SetTop(e_3_bullet, Canvas.GetTop(e_3) + 25);

            Canvas.SetLeft(e_4_bullet, Canvas.GetLeft(e_4));
            Canvas.SetTop(e_4_bullet, Canvas.GetTop(e_4) + 25);
        }


        void Game_Result()
        {
            if (!gameStarted) return;

            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Image enemy && enemy.Tag?.ToString() == "Enemy")
                {
                    // Get bounding rectangles for Bullet1, PlayerImage, and the enemy
                    Rect bulletRect = new Rect(Canvas.GetLeft(Bullet1), Canvas.GetTop(Bullet1), Bullet1.ActualWidth, Bullet1.ActualHeight);
                    Rect enemyRect = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.ActualWidth, enemy.ActualHeight);
                    Rect playerRect = new Rect(Canvas.GetLeft(PlayerImage), Canvas.GetTop(PlayerImage), PlayerImage.ActualWidth, PlayerImage.ActualHeight);

                    // Bullet hits enemy
                    if (bulletRect.IntersectsWith(enemyRect))
                    {
                        score += 10;
                        kill++;
                        Label_Kill.Content = "Killed: " + kill;
                        Label_Score.Content = "Score: " + score;

                        // Change enemy to explosion image
                        if (enemy is Image enemyImage)
                        {
                            enemyImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/explosion.png"));
                            string originalImageUri = enemyOriginalImages[enemyImage]; // Get the original image URI

                            // Reset enemy and bullet
                            DispatcherTimer explosionTimer = new DispatcherTimer();
                            explosionTimer.Interval = TimeSpan.FromMilliseconds(100); // Duration of the explosion effect
                            explosionTimer.Tick += (s, args) =>
                            {
                                explosionTimer.Stop();
                                Canvas.SetLeft(enemy, 1500); // Move the enemy off-screen
                                enemyImage.Source = new BitmapImage(new Uri(originalImageUri)); // Reset enemy image
                            };
                            explosionTimer.Start();
                        }
                    }

                    // Player hits enemy
                    if (playerRect.IntersectsWith(enemyRect))
                    {

                        EndGame();
                       
                    }
                }
            }
        }


        private void EndGame()
        {
            PlayerImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/explosion.png"));
            elapsedTimeTimer.Stop();
            gameTimer.Stop();
            gameStarted = false;



            var gameOverDialog = new GameOverDialog(score, elapsedTimeInSeconds);
            if (gameOverDialog.ShowDialog() == true)
            {
                // Add player's result to the statistics list
                AddPlayerStatistics(playerName, score, elapsedTimeInSeconds);
                if (gameOverDialog.PlayAgain)
                {

                    PlayerImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/SpaceShip_resized_1.png"));

                    RestartGame();
                }
                else
                {
                    PlayerImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/SpaceShip_resized_1.png"));

                    GoToMainMenu();
                }
            }
        }
        private void RestartGame()
        {
            score = 0;
            kill = 0;
            elapsedTimeInSeconds = 0;
            Label_Score.Content = "Score: 0";
            Label_Kill.Content = "Killed: 0";
            TimerDisplay.Content = "Time: 0 s";

            StartGameButton_Click(null,null);
            
        }
        private void GoToMainMenu()
        {
            GameCanvas.Visibility = Visibility.Hidden;
            StartMenu.Visibility = Visibility.Visible;
            score = 0;
            kill = 0;
            elapsedTimeInSeconds = 0;
            Label_Score.Content = "Score: 0";
            Label_Kill.Content = "Killed: 0";
            TimerDisplay.Content = "Time: 0 s";
        }

        private void ElapsedTimeTimer_Tick(object sender, EventArgs e)
        {
            elapsedTimeInSeconds++; // Increment the time
            TimerDisplay.Content = $"Time: {elapsedTimeInSeconds} s"; // Update the TimerDisplay label with the elapsed time
        }



        void Bullet()
        {
            // Get current left position of the bullet
            double bulletLeft = Canvas.GetLeft(Bullet1);
            Canvas.SetLeft(Bullet1, bulletLeft + 50);  // Move bullet to the right

            if (bulletLeft > 600)
            {
                Bullet1.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/bullet.png"));
                // Reset bullet position
                Canvas.SetLeft(Bullet1, Canvas.GetLeft(PlayerImage));
                Canvas.SetTop(Bullet1, Canvas.GetTop(PlayerImage) + 25);

                // Play laser sound
                laserSound.Position = TimeSpan.Zero; // Reset the sound to the beginning
                laserSound.Play();

            }

            //enemy bullet 1
            double bulletLefte1 = Canvas.GetLeft(e_1_bullet);
            Canvas.SetLeft(e_1_bullet, bulletLefte1 - 15);

            if (bulletLefte1 < 0)
            {
                Canvas.SetLeft(e_1_bullet, Canvas.GetLeft(e_1));
                Canvas.SetTop(e_1_bullet, Canvas.GetTop(e_1) + 25);
                e_1_bullet.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/image_5.png"));

            }

            //enemy bullet 2
            double bulletLefte2 = Canvas.GetLeft(e_2_bullet);
            Canvas.SetLeft(e_2_bullet, bulletLefte2 - 15);

            if (bulletLefte2 < 0)
            {
                Canvas.SetLeft(e_2_bullet, Canvas.GetLeft(e_2));
                Canvas.SetTop(e_2_bullet, Canvas.GetTop(e_2) + 25);
                e_2_bullet.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/image_5.png"));

            }

            //enemy bullet 3
            double bulletLefte3 = Canvas.GetLeft(e_3_bullet);
            Canvas.SetLeft(e_3_bullet, bulletLefte3 - 15);

            if (bulletLefte3 < 0)
            {
                Canvas.SetLeft(e_3_bullet, Canvas.GetLeft(e_3));
                Canvas.SetTop(e_3_bullet, Canvas.GetTop(e_3) + 25);
                e_3_bullet.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/image_5.png"));

            }

            //enemy bullet 4
            double bulletLefte4 = Canvas.GetLeft(e_4_bullet);
            Canvas.SetLeft(e_4_bullet, bulletLefte4 - 15);

            if (bulletLefte4 < 0)
            {
                Canvas.SetLeft(e_4_bullet, Canvas.GetLeft(e_4));
                Canvas.SetTop(e_4_bullet, Canvas.GetTop(e_4) + 25);
                e_4_bullet.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/image_5.png"));

            }

        }
        void Rocks()
        {
            Random rnd = new Random();
            int x, y;

            // Move Rock1 left
            double rock1Left = Canvas.GetLeft(Rock1);
            Canvas.SetLeft(Rock1, rock1Left - 2);  // Move rock1 left by 2 pixels
            if (rock1Left < 0)
            {
                x = rnd.Next(0, 400);
                Canvas.SetLeft(Rock1, 800);  // Reset rock1 to the right side
                Canvas.SetTop(Rock1, x);  // Random vertical position
            }

            // Move Rock2 left
            double rock2Left = Canvas.GetLeft(Rock2);
            Canvas.SetLeft(Rock2, rock2Left - 2);  // Move rock2 left by 2 pixels
            if (rock2Left < 0)
            {
                y = rnd.Next(0, 500);
                Canvas.SetLeft(Rock2, 800);  // Reset rock2 to the right side
                Canvas.SetTop(Rock2, y);  // Random vertical position
            }
        }


        void Enemy()
        {
            Random rnd = new Random();

            // Move e_1
            double e1Left = Canvas.GetLeft(e_1);
            Canvas.SetLeft(e_1, e1Left - 10); // Move enemy left by 10 pixels
            if (e1Left < 0)
            {
                int e1Top = rnd.Next(0, 150); // Random vertical position
                Canvas.SetLeft(e_1, 800);     // Reset to the right side
                Canvas.SetTop(e_1, e1Top);   // Set new random Top position
            }

            // Move e_2
            double e2Left = Canvas.GetLeft(e_2);
            Canvas.SetLeft(e_2, e2Left - 8); // Move enemy left by 8 pixels
            if (e2Left < 0)
            {
                int e2Top = rnd.Next(50, 250);
                Canvas.SetLeft(e_2, 800);
                Canvas.SetTop(e_2, e2Top);
            }

            // Move e_3
            double e3Left = Canvas.GetLeft(e_3);
            Canvas.SetLeft(e_3, e3Left - 12); // Move enemy left by 12 pixels
            if (e3Left < 0)
            {
                int e3Top = rnd.Next(250, 350);
                Canvas.SetLeft(e_3, 800);
                Canvas.SetTop(e_3, e3Top);
            }

            double e4Left = Canvas.GetLeft(e_4);
            Canvas.SetLeft(e_4, e4Left - 10); // Move enemy left by 10 pixels
            if (e4Left < 0)
            {
                int e4Top = rnd.Next(250, 400);
                Canvas.SetLeft(e_4, 800);
                Canvas.SetTop(e_4, e4Top);
            }
        }
        void Stars()
        {
            foreach (UIElement x in GameCanvas.Children)
            {
                if (x is Image && (x as Image).Tag?.ToString() == "star")
                {
                    double left = Canvas.GetLeft(x);
                    Canvas.SetLeft(x, left - 5);  // Move star left by 5 pixels

                    if (left < 0)
                    {
                        Canvas.SetLeft(x, 800);  // Reset star to the right side
                    }
                }
            }
        }

        // This is called on every tick of the game timer
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted) return;

            Bullet();
            Enemy();
            Game_Result();
            Stars();
            Rocks();

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted) return;

            if (e.Key == Key.Up)
            {
                if (Canvas.GetTop(PlayerImage) > 20)
                {
                    double newTop = Canvas.GetTop(PlayerImage) - 5;
                    Canvas.SetTop(PlayerImage, newTop);
                    Debug.WriteLine($"Moved Up: New Top = {newTop}");
                }
            }
            if (e.Key == Key.Down)
            {
                if (Canvas.GetTop(PlayerImage) < 350)
                {
                    double newTop = Canvas.GetTop(PlayerImage) + 5;
                    Canvas.SetTop(PlayerImage, newTop);
                    Debug.WriteLine($"Moved Down: New Top = {newTop}");
                }
            }
        }
    }
}