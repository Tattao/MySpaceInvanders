using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace MySpaceInvanders
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StartPage : Page
    {
        private const int StarCount = 200;
        private List<Dot> stars=new List<Dot>(StarCount);
        private Random randomizer = new Random();

        public StartPage()
        {
            this.InitializeComponent();

            Loaded += (sender, args) =>
                {
                    CreateStar();
                    Move.Completed += MoveStars;
                    Move.Begin();
                };
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GamePage));
        }

        private void CreateStar()
        {
            var star = new Dot()
            {
                Shape = new Ellipse() { Height = 2, Width = 2 },
                Velocity = new Point(randomizer.Next(-5, 5), randomizer.Next(-5, 5))
            };

            // Center the star
            Canvas.SetLeft(star.Shape, LayoutRoot.ActualWidth / 2 - star.Shape.Width / 2);
            Canvas.SetTop(star.Shape, (LayoutRoot.ActualHeight / 2 - star.Shape.Height / 2) + 20);

            // Prevent stars getting stuck
            if ((int)star.Velocity.X == 0 && (int)star.Velocity.Y == 0)
            {
                star.Velocity = new Point(randomizer.Next(1, 5), randomizer.Next(1, 5));
            }

            // Set color

            stars.Add(star);
            LayoutRoot.Children.Add(star.Shape);

            //Add color-line
            var colors = new byte[4];
            randomizer.NextBytes(colors);
            star.Shape.Fill = new SolidColorBrush(
                Color.FromArgb(colors[0], colors[1], colors[2], colors[3]));

        }

        void MoveStars(object sender, object e)
        {
            if (stars.Count < StarCount)
            {
                CreateStar();
            }

            foreach (var star in stars)
            {
                double left = Canvas.GetLeft(star.Shape) + star.Velocity.X;
                double top = Canvas.GetTop(star.Shape) + star.Velocity.Y;

                Canvas.SetLeft(star.Shape, left);
                Canvas.SetTop(star.Shape, top);

                // Star is off the screen
                if ((int)left < 0 ||
                    (int)left > LayoutRoot.ActualWidth ||
                    (int)top < 0 ||
                    (int)top > LayoutRoot.ActualHeight)
                {
                    Canvas.SetLeft(star.Shape, LayoutRoot.ActualWidth / 2 - star.Shape.Width / 2);
                    Canvas.SetTop(star.Shape, (LayoutRoot.ActualHeight / 2 - star.Shape.Height / 2) + 20);
                }
            }
            Move.Begin();
        }

    }
}
