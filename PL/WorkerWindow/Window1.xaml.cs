using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Shapes;
using BL;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
  
    public partial class Window1 : Window
    {
        BL.IBl instance = BLFactory.Instance;
        SimulatorClock simulatorClock = SimulatorClock.Instance;


        public Window1()
        {
            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            BusesWindow window = new BusesWindow();
            window.Show();
            this.Close();
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            LinesWindow window = new LinesWindow();
            window.Show();
            this.Close();
        }
     
        

        private void StationButton_Click(object sender, RoutedEventArgs e)
        {
           StationsWindow window = new StationsWindow();
            window.Show();
            this.Close();

        }
        private void simulationstart(object sender, RoutedEventArgs e)
        {
            SimulationParameters win = new SimulationParameters();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            win.ShowDialog();
            if(simulatorClock.Time.Seconds!=-1)
            {
                label.Visibility = Visibility.Visible;
                label.Background = null;
                this.DataContext = simulatorClock;
            }
           

        }
        private void Check(object sender, TextChangedEventArgs e)
        {
            if (simulatorClock.Time.Seconds == -1)
                label.Visibility = Visibility.Hidden;
        }
        private void stopsimulation(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            instance.StopSimulator();
        }
    }
}
