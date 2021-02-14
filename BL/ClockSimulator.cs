using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
namespace BL
{
    public class SimulatorClock:INotifyPropertyChanged
    {
        #region Singleton
        static readonly SimulatorClock instance = new SimulatorClock();

        static SimulatorClock() { }

        SimulatorClock() { Time = new TimeSpan(0, 0, -1); }

        public static SimulatorClock Instance { get => instance; }
        #endregion

        private TimeSpan time;
       
        public string TimeinFormat
        {
            get
            {
                return time.ToString("hh\\:mm\\:ss");
            }
            set
            {
                time = TimeSpan.Parse(value);
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TimeinFormat"));
            }
        }
        public TimeSpan Time
        {
            get
            {
                
                return time;
            }
            set
            {
                time=value;
                TimeinFormat = value.ToString();
                clockObserver?.Invoke(time);
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Time"));
            }
        }
        public int Rate { get; set; }
        internal volatile bool Cancel;
        //protected void NotifyPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        public Stopwatch stopWatch = new Stopwatch();

        event Action<TimeSpan> clockObserver;
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<TimeSpan> ClockObserver
        {
            add
            {
                ClockObserver -= value;
                clockObserver += value;
            }
            remove
            {
                if (clockObserver != null)
                {
                    foreach (var d in clockObserver.GetInvocationList())
                    {
                        clockObserver -= (Action<TimeSpan>)d;
                    }
                }
            }
        }
    }
}
