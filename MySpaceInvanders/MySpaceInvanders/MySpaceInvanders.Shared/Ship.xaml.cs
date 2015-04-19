using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Ship : UserControl
    {
        public Ship()
        {
            this.InitializeComponent();

#if WINDOWS_PHONE_APP
            Width = 20;
            Height = 40;
            BodyShape.Points = new PointCollection()
            {
                new Point(0, 40), new Point(10,0), new Point(20,40)
            };
#else
            Width = 40;
            Height = 80;
            BodyShape.Points = new PointCollection()
            {
                new Point(0, 80), new Point(20,0), new Point(40,80)
            };
#endif

        }
    }
}
