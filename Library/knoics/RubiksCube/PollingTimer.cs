using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Knoics.RubiksCube
{
    public class PollingTimer
    {
        private int elapsedTime = 0;
        private DateTime lastTime = DateTime.Now;
        private int interval = 0;
        public PollingTimer(int inter)
        {
            interval = inter;
        }
        public bool OnInterval() 
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - lastTime;
            lastTime = now;
            elapsedTime += elapsed.Milliseconds;
            bool onInterval = elapsedTime >= interval;
            if (onInterval) elapsedTime = 0;
            return onInterval;
        }

        public void Reset() 
        {
            elapsedTime = 0;
        }
    }
}
