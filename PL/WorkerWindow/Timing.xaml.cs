using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour Timing.xaml
    /// </summary>
    public partial class Timing : Window
    {
        BL.IBl instance;

        public SimulatorClock simulatorClock;
        DispatcherTimer TimerLeft;


        LineTiming s;
        StationLine temp;
        public Timing(StationLine station,IBl b,SimulatorClock sC)
        {
            InitializeComponent();
             s = new LineTiming();
            simulatorClock = sC;
            instance=b;
            temp = station;
            myTime.DataContext = simulatorClock;
            if (simulatorClock.Time.Seconds != -1)
            {
                LabelSimulationTime.Visibility = Visibility.Visible;
            }
            else
                LabelSimulationTime.Visibility = Visibility.Hidden;
            temp.myLines.Clear();
            instance.findlineForStation(temp);
            Uri myl = new Uri("https://drive.google.com/uc?export=download&id=1hXhXr4kTw4hQkomZZGF9XGe96UOy55-H", UriKind.RelativeOrAbsolute);
            var image = new BitmapImage(myl);
            myBus.Source = image;
            myl = new Uri("https://drive.google.com/uc?export=download&id=1cTQC49etPREUPvbq1azgo4vEfHQuUR_s", UriKind.RelativeOrAbsolute);
            image = new BitmapImage(myl);
            myTimer.Source = image;
            List<ExitLine> myList = instance.getmySchedules().ToList().Where(item => temp.myLines.Exists(line => item.IdBus == line.busLineNumber)).ToList();
            IEnumerable<LineTiming> myTiming = s.getmyTimings(instance, temp, myList,simulatorClock.Time);
            Info.DataContext = myTiming;
            myTiming.ToList().ForEach(item => MakeTimer(item, simulatorClock.Rate));
        }
        private void MakeTimer(LineTiming clas, int rate)
        {
            TimerLeft = new DispatcherTimer();
            TimerLeft.Start();
            TimerLeft.Interval = new TimeSpan(0, 0, 1);
            TimerLeft.Tick += (s, args) =>
            {
                //clas.MyTime = clas.MyTime.Subtract(TimeSpan.FromSeconds(1));
                if (clas.TimeBeforeArrival.Seconds == 0)
                {
                    if (clas.TimeBeforeArrival.Minutes > 0)
                    {
                        clas.TimeBeforeArrival = clas.TimeBeforeArrival.Subtract(TimeSpan.FromMinutes(1));
                        clas.TimeBeforeArrival = clas.TimeBeforeArrival.Add(TimeSpan.FromSeconds(59));
                    }
                    else if (clas.TimeBeforeArrival.Minutes == 0 && clas.TimeBeforeArrival.Hours > 0)
                    {
                        clas.TimeBeforeArrival = clas.TimeBeforeArrival.Subtract(TimeSpan.FromHours(1));
                        clas.TimeBeforeArrival = clas.TimeBeforeArrival.Add(TimeSpan.FromMinutes(59));
                        clas.TimeBeforeArrival = clas.TimeBeforeArrival.Add(TimeSpan.FromSeconds(59));
                    }
                    else
                        clas.TimeBeforeArrival = new TimeSpan(0,clas.Freq, 0);
                }
                else
                    clas.TimeBeforeArrival = clas.TimeBeforeArrival.Subtract(TimeSpan.FromSeconds(rate));

            };
        }



    }
}
