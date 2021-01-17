using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Shapes;
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour Timing.xaml
    /// </summary>
    public partial class Timing : Window
    {
        BL.IBl instance = BLFactory.Instance;

        SimulatorClock simulatorClock = SimulatorClock.Instance;
       
        LineTiming s;
        StationLine temp;
        public Timing(StationLine station)
        {
            InitializeComponent();
             s = new LineTiming();
            
            temp = station;
            myTime.DataContext = simulatorClock;
            if (simulatorClock.Time.Seconds != -1)
            {
                hour.Visibility = Visibility.Visible;
            }
            else
                hour.Visibility = Visibility.Hidden;
          
            
                Info.DataContext = s.getmyTimings(instance.findlineForStation(temp),simulatorClock.Time);
           

        }

      
          

        
    }
}
