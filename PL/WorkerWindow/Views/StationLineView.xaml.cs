using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BL;
using BL.BO;
namespace PL.WorkerWindow.Views
{
    /// <summary>
    /// Logique d'interaction pour StationLineView.xaml
    /// </summary>
    public partial class StationLineView : UserControl
    {
        BL.IBl instance = BLFactory.Instance;
        int count = 0;
        SimulatorClock simulatorClock = SimulatorClock.Instance;
        public StationLineView()
        {

            InitializeComponent();
            
            ListBus.DataContext = instance.getmyStationsLines().GroupBy(id => id.shelterNumber).Select(y => y.First());
        }
        private void ChangeColor(object sender,MouseEventArgs e)
        {

           
        }
        private void ListBus_SelectionDetail(object sender, MouseButtonEventArgs e)
        {
            if (ListBus.SelectedIndex != -1)
            {
               
               
                updatebutton.IsEnabled = true;
                updatebutton.Foreground = Brushes.Black;
                updatebutton.Background=Brushes.DeepSkyBlue ;
                distancetxt.IsReadOnly = false;
                timeText.IsReadOnly = false;
                Popupdistance.IsOpen = true;
                Popuptime.IsOpen = true;
                
                StationLine i = instance.getmyStationsLines()[ListBus.SelectedIndex];
                i = instance.findlineForStation(i);
                mycomboLine.ItemsSource = i.myLines;
                mycomboLine.SelectedIndex = 0;
                myData.DataContext = i.myLines[0].listStations.Find(station => station.shelterNumber == i.shelterNumber);
                instance.getmyTime(i);
                distancetxt.Text = i.Distance.ToString();
                timeText.Text = i.Temps.ToString();
                terminus.Text = instance.getmyStationsLines().Find(station => station.shelterNumber == i.myLines[0].lastStation).address;
            }
        }

        private void MapRequest(object sender, RoutedEventArgs e)
        {
            var cb = sender as Button;
            var thisStation = cb.DataContext as StationLine;
            string lat = thisStation.latitude;
            string lon= thisStation.longitude;
           lat= lat.Replace(",", ".");
            lon=lon.Replace(",", ".");
            string address = "https://www.google.com/maps/search/?api=1&query=" + lat + "," + lon;
            //new Map(address).ShowDialog();
            Map map = new Map(address);
            map.ShowDialog();
           
        }
        private void SimulationClicked(object sender, RoutedEventArgs e)
        {
            if (simulatorClock.Time.Seconds != -1)
            {
                Timing win = new Timing((sender as Button).DataContext as StationLine,instance,simulatorClock);
                win.ShowDialog();
            }
            else
                MessageBox.Show("Please activate the simulation in the main Menu !");
        
        }

        private void unchanged(object sender, RoutedEventArgs e)
        {

            count--;
            if (count == 0)
            {
                DeleteButton.IsEnabled = false;
                DeleteButton.Foreground = Brushes.Black;
            }
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as StationLine;
            thisbus.CheckedOrNot = false;
            instance.modifyStationline(thisbus);
           

        }
       
        private void changed(object sender, RoutedEventArgs e)
        {

            DeleteButton.IsEnabled = true;
            DeleteButton.Foreground = Brushes.Black;
            DeleteButton.Background = Brushes.DeepSkyBlue;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as StationLine;
            thisbus.CheckedOrNot = true;
            instance.modifyStationline(thisbus);
            count++;

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Popupdistance.IsOpen = false;
            Popuptime.IsOpen = false;
            if (instance.deleteStationLine())
            {
                MessageBox.Show("This station was deleted from all the buses who passing through !");
                myData.DataContext = myDetails.DataContext = null;
                DeleteButton.IsEnabled = false;
                DeleteButton.Foreground = Brushes.Black;
                updatebutton.Foreground = Brushes.Black;
                updatebutton.IsEnabled = false;
                myData.DataContext = myDetails.DataContext = mycomboLine.ItemsSource = timeText.Text = terminus.Text = distancetxt.Text = null;

                ListBus.DataContext = instance.getmyStationsLines();
            }

        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            Popupdistance.IsOpen = false;
            Popuptime.IsOpen = false;
            AddRealStationLine win = new AddRealStationLine(instance);

            win.ShowDialog();
            ListBus.DataContext = instance.getmyStationsLines().GroupBy(id => id.shelterNumber).Select(y => y.First());
        }
        private void updatebutton_Click(object sender, RoutedEventArgs e)
        {
            Popupdistance.IsOpen = false;
            Popuptime.IsOpen = false;
            StationLine i = instance.getmyStationsLines()[ListBus.SelectedIndex];
            instance.findlineForStation(i);
            i = i.myLines[mycomboLine.SelectedIndex].listStations.Find(station => station.shelterNumber == i.shelterNumber);
            float distancee = float.Parse(distancetxt.Text);
            Stationsconnected s = instance.getStationConnected().ToList().Find(st => st.numeroUno == i.ID);

            s.distance = distancee;
            TimeSpan essai = new TimeSpan();
            bool check = TimeSpan.TryParse(timeText.Text, out essai);
            if (check)
            {
                s.timeBetween = essai;
                if (instance.commitDistanceTime(s))
                {
                    ListBus.SelectedItem = null;
                    updatebutton.Foreground = Brushes.Black;
                    myData.DataContext =mycomboLine.ItemsSource=terminus.Text=numnext.Text=distancetxt.Text=timeText.Text= null;
                    updatebutton.IsEnabled = false;
                    MessageBox.Show("Commit saved !");
                }
            }
            else
                MessageBox.Show("Error of time format !");
        }

        private void mycomboLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mycomboLine.SelectedIndex != -1)
            {
                StationLine i = instance.getmyStationsLines()[ListBus.SelectedIndex];
                i = instance.findlineForStation(i);
                if (instance.getmyStationsLines().Exists(station => station.shelterNumber == i.myLines[mycomboLine.SelectedIndex].lastStation))
                {
                    terminus.Text = instance.getmyStationsLines().Find(station => station.shelterNumber == i.myLines[mycomboLine.SelectedIndex].lastStation).address;
                    i = i.myLines[mycomboLine.SelectedIndex].listStations.Find(station => station.shelterNumber == i.shelterNumber);
                    myData.DataContext = i;
                    distancetxt.Text = i.Distance.ToString();
                    timeText.Text = i.Temps.ToString();
                }

            }

        }
    }
}
