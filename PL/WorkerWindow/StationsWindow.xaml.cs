using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PL.WorkerWindow.ViewModels;
using BL;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour StationsWindow.xaml
    /// </summary>
    public partial class StationsWindow : Window
    {
        BL.IBl instance;
        SimulatorClock simulatorClock;
        List<string> combosource = new List<string>();
        public StationsWindow(IBl b,SimulatorClock s)
        {
          
            InitializeComponent();
            instance = b;
            simulatorClock = s;
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            BusButton.IsChecked = true;
            myTime.DataContext= simulatorClock;
            if (simulatorClock.Time.Seconds != -1)
            {
                currentHour.Visibility = Visibility.Visible;
                Hourstxt.Visibility = Visibility.Visible;
            }
            else
            {
                currentHour.Visibility = Visibility.Hidden;
                Hourstxt.Visibility = Visibility.Hidden;

            }
            DataContext = new StationModels();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1(instance);
            window.Show();
            this.Close();

        }
        /// <summary>
        /// Launch the stations line view
        /// </summary>
        private void LinesChecked(object sender, RoutedEventArgs e)
        {
            DataContext = new StationLineModels();
        }

        private void LinesUnchecked(object sender, RoutedEventArgs e)
        {
            BusButton.IsChecked = true;

        }
        /// <summary>
        /// Launch the stations view
        /// </summary>
        private void BusesChecked(object sender, RoutedEventArgs e)
        {
            DataContext = new StationModels();
        }
        private void BusesUnchecked(object sender, RoutedEventArgs e)
        {
            LineButton.IsChecked = true;
        }

     
    }
}
