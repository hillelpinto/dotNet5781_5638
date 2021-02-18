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
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour LinesWindow.xaml
    /// </summary>
    public partial class LinesWindow : Window
    {
        BL.IBl instance;
        int count;
        int countS;
        SimulatorClock simulatorClock;
        public LinesWindow(IBl b, SimulatorClock s)
        {

            InitializeComponent();
            simulatorClock = s;
            instance = b;
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            myTime.DataContext = simulatorClock;
            if (simulatorClock.Time.Seconds != -1)
            {
                CurrentHour.Visibility = Visibility.Visible;
                Hourstxt.Visibility = Visibility.Visible;
            }
            else
            {
                CurrentHour.Visibility = Visibility.Hidden;
                Hourstxt.Visibility = Visibility.Hidden;
            }

            ListLine.DataContext = instance.getLines();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1(instance);
            window.Show();
            this.Close();
        }
        /// <summary>
        /// Print the details about the simulation between time and arrival
        /// </summary>

        private void ScheduleClicked(object sender, RoutedEventArgs e)
        {
            var thisLine = (sender as Button).DataContext as Line;
            ScheduleLine win = new ScheduleLine(thisLine,instance);
            win.ShowDialog();

            ListLine.DataContext = instance.getLines();
         }
        /// <summary>
        /// Print the stations in the line we clicked on
        /// </summary>

        private void ListBus_SelectionDetail(object sender, MouseButtonEventArgs e)
        {
            if (ListLine.SelectedIndex != -1)
            {
                int index = ListLine.SelectedIndex;
                BL.BO.Line l = instance.getLines()[index];
                linepresented.Text = l.busLineNumber.ToString();
                instance.setIndexinLine(l);
                myDetails.DataContext = l.listStations;
                myDetails.Items.Refresh();
            }
        }
        /// <summary>
        /// In order to delete a station in a line
        /// </summary>
        private void stationchecked(object sender, RoutedEventArgs e)
        {
            DeleetStationbutton.IsEnabled = true;
            DeleetStationbutton.Background = Brushes.DeepSkyBlue;
            var cb = sender as CheckBox;
            var thisStation = cb.DataContext as BL.BO.StationLine;
            if (instance.getLines()[ListLine.SelectedIndex].listStations.Count == 2)
            {
                MessageBox.Show("Can't have less than 2 stations !");
                DeleetStationbutton.IsEnabled = false;
                DeleetStationbutton.Background = null; cb.IsChecked = false;
                return;
            }
            else
            {
                thisStation.CheckedOrNot = true;
                instance.modifyOnlyOneStation(thisStation);
                countS++;
            }
        }
        /// <summary>
        /// In order to deselect a station in deletion
        /// </summary>
        private void stationunchecked(object sender, RoutedEventArgs e)
        {
            countS--;
            if (countS == 0)
            {
                DeleetStationbutton.IsEnabled = false;
                DeleetStationbutton.Background = null;
                DeleetStationbutton.Foreground = Brushes.Black;

            }
            var cb = sender as CheckBox;
            var thisStation = cb.DataContext as BL.BO.StationLine;
            thisStation.CheckedOrNot = false;
            instance.modifyOnlyOneStation(thisStation);
        }
        /// <summary>
        /// Delete a line
        /// </summary>
        private void LineChecked(object sender, RoutedEventArgs e)
        {
            DeleteLineButton.IsEnabled = true;
            DeleteLineButton.Background = Brushes.Black;
            DeleteLineButton.Foreground = Brushes.LimeGreen;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as BL.BO.Line;
  
                thisbus.CheckedOrNot = true;
                instance.modifyLine(thisbus);
                count++;
            
        }
        /// <summary>
        /// Deselect a line for deletion
        /// </summary>
        private void LineUnchecked(object sender, RoutedEventArgs e)
        {
            count--;
            if (count == 0)
            {
                DeleteLineButton.IsEnabled = false;
                DeleteLineButton.Background = null;
                DeleteLineButton.Foreground = Brushes.Black;
            }
            else
            {
                var cb = sender as CheckBox;
                var thisLine = cb.DataContext as BL.BO.Line;
                thisLine.CheckedOrNot = false;
                instance.modifyLine(thisLine);
            }
        }
        /// <summary>
        /// Open the window to add a line
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLine window = new AddLine(instance);
            window.ShowDialog();
            linepresented.Text = null;
            ListLine.DataContext = instance.getLines();

        }
        /// <summary>
        /// Event when we delete a line
        /// </summary>
        private void DeleteLineButton_Click(object sender, RoutedEventArgs e)
        {
            var data = (BL.BO.Line)myData.DataContext;
            if (data != null && data.CheckedOrNot == true)
            {
                myData.DataContext = null;
                myDetails.Items.Refresh();
            }
            if (instance.deleteLines())
            {
                linepresented.Text = null;
                DeleteLineButton.Background = null;
                ListLine.DataContext = instance.getLines();
                DeleteLineButton.IsEnabled = false;
                DeleteLineButton.Foreground = Brushes.Black;
                myDetails.DataContext = null;
                ListLine.SelectedItem = null;
                MessageBox.Show("Your Line(s) is well deleted !");
             
            }
        }
        /// <summary>
        /// Event when we click to add a station in line
        /// </summary>
        private void AddStationpressed(object sender, RoutedEventArgs e)
        {
            var cb = sender as Button;
            var thisline = cb.DataContext as BL.BO.Line;
            AddStationLine window = new AddStationLine(thisline,instance);
            window.ShowDialog();
            ListLine.DataContext = instance.getLines();
            myData.DataContext =myDetails.DataContext=linepresented.Text= null;

        }
        /// <summary>
        /// Evenet when we click on the delete button for station
        /// </summary>

        private void DeleetStationbutton_Click(object sender, RoutedEventArgs e)
        {
            instance.deleteStationLine();
            DeleetStationbutton.Background = null;
            DeleetStationbutton.Foreground = Brushes.Black;
            DeleetStationbutton.IsEnabled = false;
            myDetails.DataContext = myData.DataContext =linepresented.Text= null;
            ListLine.DataContext = instance.getLines();
            MessageBox.Show("Your Station is well deleted from the Line");


        }
    }
}
