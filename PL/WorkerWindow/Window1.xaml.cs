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
        BL.IBl instance;
        SimulatorClock simulatorClock = SimulatorClock.Instance;

        public Window1(IBl bl)
        {
            InitializeComponent();
            instance = bl;
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            this.DataContext = simulatorClock;
            if (simulatorClock != null && simulatorClock.Time.Seconds != -1)
            {
                label.Visibility = Visibility.Visible;
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                StopButton.Foreground = Brushes.Red;

            }
            else

            {
                label.Visibility = Visibility.Hidden;
                StopButton.IsEnabled = false;
                StartButton.IsEnabled = true;
                StartButton.Foreground = Brushes.GreenYellow;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            BusesWindow window = new BusesWindow(instance,simulatorClock);
            window.Show();
            this.Close();
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            LinesWindow window = new LinesWindow(instance,simulatorClock);
            window.Show();
            this.Close();
        }
     
        

        private void StationButton_Click(object sender, RoutedEventArgs e)
        {
           StationsWindow window = new StationsWindow(instance,simulatorClock);
            window.Show();
            this.Close();

        }
        private void simulationstart(object sender, RoutedEventArgs e)
        {
            SimulationParameters win = new SimulationParameters(instance);
           
            win.ShowDialog();
            if (win.flag == true)
            {
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                label.Visibility = Visibility.Visible;
                StopButton.Foreground = Brushes.Red;

            }

        }

        private void stopsimulation(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            StartButton.Foreground = Brushes.GreenYellow;
            label.Visibility = Visibility.Hidden;
            instance.StopSimulator();
        }
    }
}
