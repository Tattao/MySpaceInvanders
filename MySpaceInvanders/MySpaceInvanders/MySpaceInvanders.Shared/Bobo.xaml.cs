using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace MySpaceInvanders
{
    public sealed partial class Bobo : UserControl
    {
        public int AreaWidth { get; set; }
        public Point Location { get; set; }
        public bool Dead { get; set; }
        public int Worth { get; set; } // amount of score for kill
        public int Type; // 1 - green, 2 - blue, 3 - mega
        private readonly Random randomizer = new Random();
        public double Velocity;
        private int direction;
        private int directionCount = 0; // don't change direction on every loop

        public Bobo()
        {
            this.InitializeComponent();

            Velocity = randomizer.Next(1, 3);
            Type = randomizer.Next(1, 4);
            if (Type == 3)
            {
                Velocity = 4;
            }
            SetType();
        }

        public void Move()
        {
            int move;

            // Randomize the move direction
            if (directionCount == 0)
            {
                direction = randomizer.Next(1, 3);
            }
            if (direction == 1)
            {
                move = -1;
            }
            else
            {
                move = 1;
            }
            directionCount++;

            // Change direction every 30 count
            if (directionCount > 30)
            {
                directionCount = 0;
            }

            // Check that the bobo doesn't go through the game area walls
            if (Location.X + direction < 0)
            {
                move = 0;
            }
            if (Location.X + direction > AreaWidth)
            {
                move = AreaWidth;
            }

            // Set the new location
            Location = new Point(Location.X + move, Location.Y + Velocity);
        }

        private void SetType()
        {
            switch (Type)
            {
                case 1:
                    SetFill(Color.FromArgb(0xFF, 0x00, 0xA2, 0x07), Color.FromArgb(0xFF, 0x3A, 0xFF, 0x00));
                    Worth = 10;
                    break;
                case 2:
                    SetFill(Color.FromArgb(0xFF, 0x00, 0x00, 0xa0), Color.FromArgb(0xFF, 0x00, 0x0F, 0xff));
                    Worth = 20;
                    break;
                case 3:
                    SetFill(Color.FromArgb(0xFF, 0xaf, 0x00, 0x00), Color.FromArgb(0xFF, 0xff, 0x0F, 0x00));
                    Worth = 50;
                    break;
            }
        }

        private void SetFill(Color start, Color end)
        {
            var startGradient = new GradientStop();
            var endGradient = new GradientStop();
            startGradient.Color = start;
            startGradient.Offset = 1;
            endGradient.Color = end;
            var collection = new GradientStopCollection();
            collection.Add(startGradient);
            collection.Add(endGradient);

            InsideEllipse.Fill = new LinearGradientBrush(collection, 0);
        }
    }
}
