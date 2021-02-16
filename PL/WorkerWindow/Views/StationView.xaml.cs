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
        private void addpopup(object sender, TextChangedEventArgs e)
        {
            Popupadd.IsOpen = false;
        }
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

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            AddStation window = new AddStation(instance);
            window.ShowDialog();
            ListBus.DataContext = instance.GetStations();
            ListBus.Items.Refresh();
        }

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
