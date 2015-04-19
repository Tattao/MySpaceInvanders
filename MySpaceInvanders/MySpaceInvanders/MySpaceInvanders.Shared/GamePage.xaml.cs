using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MySpaceInvanders
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private bool goingLeft = false, goingRight = false;
        private double shipPosition;
        private readonly double shipHorizontalPosition = App.ScreenHeight - 50;
        private const int StarCount = 200;
        private List<Dot> stars = new List<Dot>(StarCount);
        private Random randomizer = new Random();
        private List<Bobo> enemies = new List<Bobo>();
        private int maxEnemies = 20;
        private DispatcherTimer timer = new DispatcherTimer();
        private int Level { get; set; } // Player level 
        private int Score { get; set; } // Game score
        private List<Ellipse> bullets = new List<Ellipse>(); // Bullets on the screen
        private bool gameRunning = true; // Did we die already
        private TextBlock GameOver = new TextBlock();

        public GamePage()
        {
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                // Resize move controls to fit the area
                LeftCanvas.Width = LeftCanvas.Height = (LeftArea.ActualWidth / 2) - 10;
                RightCanvas.Width = RightCanvas.Height = (LeftArea.ActualWidth / 2) - 10;

                // Position the ship to the bottom center of the screen
                shipPosition = LayoutRoot.ActualWidth / 2;
                Rocket.Margin = new Thickness(shipPosition, shipHorizontalPosition, 0, 0);

                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            };

            timer.Tick += TimerOnTick;
            timer.Interval = new TimeSpan(0, 0, 0, 2);
            timer.Start();
            CompositionTarget.Rendering += GameLoop;

            // Starfield background
            CreateStar();
            Move.Completed += MoveStars;
            Move.Begin();

#if WINDOWS_PHONE_APP
            MiddleArea.Width = new GridLength(6, GridUnitType.Star);
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 24));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Top));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            Resources.Add(typeof(TextBlock), style);
            HighscoreBoard.Margin = new Thickness(5, 22, 0, 0);
            ScoreTitle.Margin = new Thickness(5, 44, 0, 0);
            ScoreBoard.Margin = new Thickness(5, 73, 0, 0);
#else
            MiddleArea.Width = new GridLength(12, GridUnitType.Star);
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, 32));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Top));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Left));
            Resources.Add(typeof(TextBlock), style);
            HighscoreBoard.Margin = new Thickness(5, 32, 0, 0);
            ScoreTitle.Margin = new Thickness(5, 64, 0, 0);
            ScoreBoard.Margin = new Thickness(5, 96, 0, 0);
#endif
        }

        private void ToLeftPressed(object sender, PointerRoutedEventArgs e)
        {
            goingLeft = true;
        }

        private void ToRightPressed(object sender, PointerRoutedEventArgs e)
        {
            goingRight = true;
        }

        private void ToLeftReleased(object sender, PointerRoutedEventArgs e)
        {
            goingLeft = false;
        }

        private void ToLeftExited(object sender, PointerRoutedEventArgs e)
        {
            goingLeft = false;
        }
        private void ToRightReleased(object sender, PointerRoutedEventArgs e)
        {
            goingRight = false;
        }

        private void ToRightExited(object sender, PointerRoutedEventArgs e)
        {
            goingRight = false;
        }
        private void OnFire(object sender, TappedRoutedEventArgs e)
        {
            if (gameRunning)
            {
                var bullet = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Fill = new SolidColorBrush(Colors.Red)
                };
                bullet.Margin = new Thickness(shipPosition + (Rocket.Width / 2) - (bullet.Width / 2),
                    shipHorizontalPosition + 2, 0, 0);
                LayoutRoot.Children.Add(bullet);
                bullets.Add(bullet);
            }
        }

        private void MoveShip(int amount)
        {
            shipPosition += amount;

            // Let's make sure that the ship stays in the screen
            if (shipPosition > LayoutRoot.ActualWidth - 30)
            {
                shipPosition = LayoutRoot.ActualWidth - 30;
            }
            else if (shipPosition < 0)
            {
                shipPosition = 0;
            }

            Rocket.Margin = new Thickness(shipPosition, shipHorizontalPosition, 0, 0);
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    MoveShip(-5);
                    break;
                case VirtualKey.Right:
                    MoveShip(5);
                    break;
                case VirtualKey.Space:
                    OnFire(null, null);
                    break;
                default:
                    break;
            }
        }

        void MoveStars(object sender, object e)
        {
            if (stars.Count < StarCount)
            {
                CreateStar();
            }

            foreach (Dot star in stars)
            {
                Canvas.SetLeft(star.Shape, Canvas.GetLeft(star.Shape) + star.Velocity.X);
                Canvas.SetTop(star.Shape, Canvas.GetTop(star.Shape) + star.Velocity.Y);

                if (Canvas.GetTop(star.Shape) > LayoutRoot.ActualHeight)
                {
                    int left = randomizer.Next(0, (int)LayoutRoot.ActualWidth);
                    Canvas.SetLeft(star.Shape, left);
                    Canvas.SetTop(star.Shape, 0);
                }
            }
            Move.Begin();
        }
        private void CreateStar()
        {
            var star = new Dot()
            {
                Shape = new Ellipse() { Height = 2, Width = 2 },
                Velocity = new Point(0, randomizer.Next(1, 5))
            };

            int left = randomizer.Next(0, (int)LayoutRoot.ActualWidth);
            Canvas.SetLeft(star.Shape, left);
            Canvas.SetTop(star.Shape, 0);
            Canvas.SetZIndex(star.Shape, 1);

            // Set color
            byte c = (byte)randomizer.Next(10, 255);
            star.Shape.Fill = new SolidColorBrush(Color.FromArgb(c, c, c, c));

            stars.Add(star);
            LayoutRoot.Children.Add(star.Shape);
        }

        /// <summary>
        /// Create a new enemy if not max amount on the screen already
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="o"></param>
        private void TimerOnTick(object sender, object o)
        {
            if (enemies.Count < maxEnemies)
            {
                var enemy = new Bobo
                {
                    AreaWidth = (int)LayoutRoot.ActualWidth,
                    Location = new Point(randomizer.Next(0, (int)LayoutRoot.ActualWidth - 80), 0)
                };
                if (enemy.Type == 3)
                {
                    // Make the red enemy smaller and more difficult to hit
                    var scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX = scaleTransform.ScaleX * 0.50;
                    scaleTransform.ScaleY = scaleTransform.ScaleY * 0.50;
                    enemy.RenderTransform = scaleTransform;
                    enemy.Width = 30;
                    enemy.Height = 30;
                }
                enemy.Velocity = enemy.Velocity * ((Level / (double)10) + 1);
                enemies.Add(enemy);
                Canvas.SetZIndex(enemy, 7);
                LayoutRoot.Children.Add(enemy);
            }
        }

        private void GameLoop(object sender, object e)
        {
            if (goingRight)
                MoveShip(5);
            if (goingLeft)
                MoveShip(-5);
            // TODO - collision test

            // TODO - move bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                MoveBullet(bullets[i]);
            }
            // Move enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Dead == false)
                {
                    enemies[i].Move();
                    enemies[i].Margin = new Thickness(enemies[i].Location.X, enemies[i].Location.Y, 0, 0);
                }

                if (enemies[i].Margin.Top > App.ScreenHeight || enemies[i].Dead)
                {
                    LayoutRoot.Children.Remove(enemies[i]);
                    enemies.Remove(enemies[i]);
                }
            }
        }

        private void MoveBullet(Ellipse ellipse)
        {
            if ((ellipse.Margin.Top - 10) > 0)
            {
                ellipse.Margin = new Thickness(ellipse.Margin.Left, ellipse.Margin.Top - 10, 0, 0);
                HitTest(ellipse);
            }
            else
            {
                bullets.Remove(ellipse);
                LayoutRoot.Children.Remove(ellipse);
            }
        }

        private void HitTest(Ellipse ellipse)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemyInFire = new Rect(enemies[i].Location.X, enemies[i].Location.Y, enemies[i].ActualWidth, enemies[i].ActualHeight);
                if (enemyInFire.Contains(new Point(ellipse.Margin.Left, ellipse.Margin.Top)))
                {
                    Score += enemies[i].Worth;
                    ScoreBoard.Text = Score.ToString();
                    if (Score > App.Highscore)
                    {
                        App.Highscore = Score;
                        HighscoreBoard.Text = Score.ToString();
                    }
                    LayoutRoot.Children.Remove(ellipse);
                    bullets.Remove(ellipse);
                    enemies[i].Dead = true;
                    return;
                }
            }
        }

        private void CrashTest()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemyCreature = new Rect(enemies[i].Location.X, enemies[i].Location.Y, enemies[i].ActualWidth, enemies[i].ActualHeight);
                enemyCreature.Intersect(new Rect(Rocket.Margin.Left, Rocket.Margin.Top, Rocket.ActualWidth,
                    Rocket.Margin.Top));
                if (!enemyCreature.IsEmpty)
                {
                    CompositionTarget.Rendering -= GameLoop;
                    Move.Completed -= MoveStars;

                    GameOver.Text = "Game Over!";
                    GameOver.FontSize = 48;
                    GameOver.VerticalAlignment = VerticalAlignment.Center;
                    GameOver.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(GameOver, 1);

                    MainGrid.Children.Add(GameOver);
                    gameRunning = false;

                    if (App.Highscore < Score)
                    {
                        App.Highscore = Score;
                    }
                }
            }
        }
    }
}
