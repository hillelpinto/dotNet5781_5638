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
            List<ExitLine> myList = instance.getmySchedules().ToList().Where(item => temp.myLines.Exists(line => item.IdBus == line.ID)).ToList();
           
            IEnumerable<LineTiming> myTiming = s.getmyTimings(temp, myList,simulatorClock.Time);
            Info.DataContext = myTiming;
            myTiming.ToList().ForEach(item => ServiceEnded(item));

            myTiming.ToList().ForEach(item => MakeTimer(item, simulatorClock.Rate));
        }

        private void MakeTimer(LineTiming clas, int rate)
        {
            TimerLeft = new DispatcherTimer();
            TimerLeft.Start();
            TimerLeft.Interval = new TimeSpan(0, 0, 1);
            TimerLeft.Tick += (s, args) =>
            {
                Check(clas, rate);
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
                        clas.TimeBeforeArrival = new TimeSpan(0, clas.Freq, 0);
                }
                else
                {

                    clas.TimeBeforeArrival = clas.TimeBeforeArrival.Subtract(TimeSpan.FromSeconds(rate));
            

                }


            };
        }
        void ServiceEnded(LineTiming line)//This function handle the case that the line is not in  service
        {
            int id = instance.getAllAllLine().ToList().Find(item => item.busLineNumber == line.BusLineNumber).ID;
            TimeSpan debut = instance.getmySchedules().ToList().Find(item => item.IdBus == id).Start;
            TimeSpan fin = instance.getmySchedules().ToList().Find(item => item.IdBus == id).End;
            int h1 = simulatorClock.Time.Hours;
            int h2 = debut.Hours;
            if (simulatorClock.Time.Hours >= fin.Hours || simulatorClock.Time.Hours < debut.Hours)
            {
                if (h2 - h1 > 0)//After midnight,ex : h2=06:00:00 , h1=01:00:00
                {
                        line.TimeBeforeArrival = new TimeSpan(h2 - h1 - 1, 60 - simulatorClock.Time.Minutes, 60 - simulatorClock.Time.Seconds);
                    
                }
                else if (h2 - h1 < 0)//Before midnight,ex  :h2=06:00:00 ,h1=23:00:00
                {
                   
                            line.TimeBeforeArrival = new TimeSpan(23 + h2 - h1, 60 - simulatorClock.Time.Minutes, 60 - simulatorClock.Time.Seconds);
                    

                    
                }
                else
                {
                  
                        line.TimeBeforeArrival = new TimeSpan(debut.Hours, 60 - simulatorClock.Time.Minutes, 60 - simulatorClock.Time.Seconds);
                }
            }



        }

        private void Check(LineTiming objet,int rate)//This function check if the bus is arrive to the station then we add to the station frequencies time of the Line
        {
            var temp = (Line)instance.getAllAllLine().ToList().Find(line => line.busLineNumber == objet.BusLineNumber);
            int id = temp.ID;
            TimeSpan realCheck = objet.TimeBeforeArrival;
            realCheck = realCheck.Subtract(TimeSpan.FromSeconds(rate));
            TimeSpan beginAt = instance.getmySchedules().ToList().Find(item => item.IdBus == id).Start;
            if (realCheck.Seconds<=0&&realCheck.Hours==0&&realCheck.Minutes==0)
            {
                objet.TimeBeforeArrival = new TimeSpan(0, objet.Freq, 0);
            }
          
        }



    }
}
