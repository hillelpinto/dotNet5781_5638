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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BL;
using BL.BO;
namespace PL.WorkerWindow.Views
{
    /// <summary>
    /// Logique d'interaction pour StationView.xaml
    /// </summary>
    public partial class StationView : UserControl
    {
        BL.IBl instance = BLFactory.Instance;
        string comparaison = null;
        int count;
        public StationView()
        {
            InitializeComponent();
          
            ListBus.DataContext = instance.GetStations();

        }
        /// <summary>
        ///  It sets all the value on the right side about the stations physic
        /// </summary>
        private void ListBus_SelectionDetail(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int index = ListBus.SelectedIndex;
                Station i = instance.getStation(index);
                myData.DataContext = i;
             
                updatebutton.Foreground = Brushes.Black;
                updatebutton.Background = Brushes.DeepSkyBlue;

                updatebutton.IsEnabled = true;
                Popupadd.IsOpen = true;

                myDetails.Items.Refresh();
                AdText.IsReadOnly = false;
                comparaison = AdText.Text;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        ///This event is the callback of the checkbox selected when we want to delte a station
        /// </summary>
        private void changed(object sender, RoutedEventArgs e)
        {
            count++;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as Station;
            thisbus.CheckedOrNot = true;
            instance.modifyStation(thisbus);
            DeleteButton.IsEnabled = true;
            DeleteButton.Foreground = Brushes.Black;
            DeleteButton.Background = Brushes.DeepSkyBlue;


        }
        /// <summary>
        ///Same as the function below but when we deselect
        /// </summary>
        private void unchanged(object sender, RoutedEventArgs e)
        {
            count--;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as Station;
            thisbus.CheckedOrNot = false;
            instance.modifyStation(thisbus);

            if (count == 0)
            {
                DeleteButton.IsEnabled = false;
                DeleteButton.Foreground = Brushes.Black;
            }

        }
        /// <summary>
        ///Popup shown
        /// </summary>
        private void addpopup(object sender, TextChangedEventArgs e)
        {
            Popupadd.IsOpen = false;
        }
        /// <summary>
        ///This function is launched when we click on the delte button,it erased all the stations selected
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

           
            if (instance.deleteStations())
            {
                ListBus.DataContext = instance.GetStations();
                MessageBox.Show("Your station(s) is well deleted !");
                myData.DataContext = myDetails.DataContext = null;
                DeleteButton.IsEnabled = false;
                updatebutton.IsEnabled = false;
                DeleteButton.Foreground = Brushes.Black;

            }
        }
        /// <summary>
        ///It opens the window to add a station
        /// </summary>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            AddStation window = new AddStation(instance);
            window.ShowDialog();
            ListBus.DataContext = instance.GetStations();
            ListBus.Items.Refresh();
        }
        /// <summary>
        ///It gets the data in address field and update it according to a new lat lon value ,and set it to the station selected
        /// </summary>

        private void updatebutton_Click(object sender, RoutedEventArgs e)
        {
            if (comparaison != AdText.Text)
            {
                int index = ListBus.SelectedIndex;
                Station i = instance.getStation(index);
                i.address = AdText.Text;
                instance.modifyStation(i);
                MessageBox.Show("Commit saved !");
                ListBus.DataContext = instance.GetStations();
                updatebutton.IsEnabled = false;
                updatebutton.Foreground = Brushes.Black;
                ListBus.SelectedItem = null;
                myData.DataContext = null;
            }
        }
    }
}
