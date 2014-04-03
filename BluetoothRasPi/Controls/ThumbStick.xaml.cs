using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BluetoothRasPi.Controls
{
    public partial class ThumbStick : PhoneApplicationPage
    {
        public event EventHandler<string> NewPosition;
        private int _snapValue = 10;
        public ThumbStick()
        {
            InitializeComponent();
        }

        private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            WorkOutPosition(e.GetPosition(LayoutRoot));
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LayoutRoot.CaptureMouse();
            WorkOutPosition(e.GetPosition(LayoutRoot));
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LayoutRoot.ReleaseMouseCapture();
            //return pos to middle
            WorkOutPosition(new Point(200, 200));
        }

        private void WorkOutPosition(Point newPos)
        {
            newPos.X = Clamp((newPos.X > -_snapValue && newPos.X < _snapValue) ? 0 : newPos.X, 0, 400);
            newPos.Y = Clamp((newPos.Y > -_snapValue && newPos.Y < _snapValue) ? 0 : newPos.Y, 0, 400);

            crosshairTransform.TranslateX = newPos.X - 200;
            crosshairTransform.TranslateY = newPos.Y - 200;

            //work out how far on either side of the center they have moved
            //we want the turn to be in the range -100 to 100
            var turnPercent = -((newPos.X - 200) / 2);
            //and the speed to be -1 to 1
            var speedPercent = -((newPos.Y - 200) / 200);
            var speedPercentAbs = Math.Abs(speedPercent);

            var leftTurnVal = (100 - Clamp(turnPercent, 0, 100)) * speedPercentAbs;
            var rightTurnVal = (100 + Clamp(turnPercent, -100, 0)) * speedPercentAbs;

            if (NewPosition != null)
            {
                NewPosition(this, string.Format("{0},{1},{2}",
                 speedPercent < 0 ? "0" : "1", //0 means back, 1 means next
                 (int)leftTurnVal,
                 (int)rightTurnVal));
            }
        }

        private double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}