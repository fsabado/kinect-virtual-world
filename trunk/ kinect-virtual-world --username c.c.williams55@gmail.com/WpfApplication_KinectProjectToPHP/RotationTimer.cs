using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace WpfApplication_KinectProjectToPHP
{
    class RotationTimer
    {
        private Timer _rotationTimer;

        public event EventHandler RotateTime;

        public delegate void EventHandler(object sender, EventArgs e);

        public RotationTimer()
        {
            this._rotationTimer = new Timer();
            this._rotationTimer.Elapsed += OnOneSecond;
            this._rotationTimer.Interval = 3000;
            this._rotationTimer.Enabled = true;
        }

        public void ToggleTimer()
        {
            if(this._rotationTimer.Enabled == true)
                this._rotationTimer.Enabled = false;
            else
                this._rotationTimer.Enabled = true;
        }       

        private void OnOneSecond(object source, EventArgs args)
        {
            if (RotateTime != null)
            {
                RotateTime(source, args);
            }
        }

    }
}
