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
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour BusesWindow.xaml
    /// </summary>
    public partial class BusesWindow : Window
    {
        string comp1, comp2 = null;
        int count;
        BL.IBl instance = BL.BLFactory.Instance;
        BackgroundWorker threadMaintenance;
        BackgroundWorker threadRefuel;
       List<string> comboSource =new List<string>();
        Bus b = new Bus();
        bool inLicenseFilter = false;

        public BusesWindow()
        {
            
            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            ListBus.DataContext = instance.GetBuses();
           
            comboSource.Add("Higher fuel to lower");
            comboSource.Add("Higher Km since checkup to lower");
            comboSource.Add("Higher Km to lower");
            comboSource.Add("No Filter");
            Combofilter.ItemsSource = comboSource;
            Combofilter.SelectedIndex = 3;

        }
        
        private void ListBus_SelectionDetail(object sender, MouseButtonEventArgs e)
        {
            int index = ListBus.SelectedIndex;
            if (index != -1)
            {
                if (Combofilter.SelectedIndex == 0)
                {
                    b = instance.getBusesFilteringbyFuel()[index];
                }
                if (Combofilter.SelectedIndex == 1)
                {
                    b = instance.getBusesFilteringByKmC()[index];

                }
                if (Combofilter.SelectedIndex == 2)
                {
                    b = instance.getBusesFilteringbyKm()[index];
                }
                else if (inLicenseFilter == true)
                {
                    b = instance.getBusesFilteringBylicense(LicenseTosearch.Text)[index];
                }

                else
                    b = instance.GetBuses()[index];
                PopupDriver.IsOpen = true;
                PopupSeat.IsOpen = true;

                myData.DataContext = b;
                DriverField.Text = b.DriverName;
                SeatField.Text = b.SeatAvailable.ToString();
                DriverField.IsReadOnly = false;
                SeatField.IsReadOnly = false;
                updatebutton.IsEnabled = true;
                myDetails.Items.Refresh();
                comp1 = DriverField.Text;
                comp2 = SeatField.Text;
            }
        }
        private void MaintenanceClicked(object sender, RoutedEventArgs e)
        {
            var b = (sender as Button).DataContext as Bus;
            threadMaintenance = new BackgroundWorker();
            threadMaintenance.DoWork += threadMaintenance_DoWork;
            threadMaintenance.WorkerReportsProgress = true;
            threadMaintenance.RunWorkerCompleted += threadMaintenance_RunWorkerCompleted;
            threadMaintenance.RunWorkerAsync(b);
        }

        private void threadMaintenance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var item = (Bus)e.Result;
            if (e.Error == null)//If all's good so we make the maintenance
            {
                item.returnStatus = "Ready";
                
                item.Checkup = DateTime.Now;
                item.KmAfterLastMaintenance = 0;
                item.Fuel = 1200;
                instance.modifyBus(item);
                ListBus.Items.Refresh();
            }
            else
                MessageBox.Show("Error !");
        }

        private void threadMaintenance_DoWork(object sender, DoWorkEventArgs e)
        {
            var mine = (Bus)e.Argument;//mine will be the bus we passed in parameter of the runworkerasynch
            mine.returnStatus = "InMaintenance";
            instance.modifyBus(mine);
            for (int i = 0; i < 100; i++)
            {
                mine.Percent = i;
                Thread.Sleep(10);
            }
           
            mine.Percent = 100;
            e.Result = mine;
        }

        private void RefuelClicked(object sender, RoutedEventArgs e)
        {
            var data = (sender as Button).DataContext as Bus;
            threadRefuel = new BackgroundWorker();
            threadRefuel.DoWork += threadRefuel_DoWork;
            threadRefuel.WorkerReportsProgress = true;
            threadRefuel.RunWorkerCompleted += threadRefuel_RunWorkerCompleted;
            threadRefuel.RunWorkerAsync(data);

        }

        private void threadRefuel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var item = (Bus)e.Result;
            if (e.Error == null)//If all's good so we make the maintenance
            {
                item.returnStatus = "Ready";
                item.Fuel = 1200;
                instance.modifyBus(item);
                ListBus.Items.Refresh();
            }
            else
                MessageBox.Show("Error !");
        }

        private void threadRefuel_DoWork(object sender, DoWorkEventArgs e)
        {
            var mine = (Bus)e.Argument;
            mine.returnStatus = "Refueling";
            instance.modifyBus(mine);
            for (int i = 0; i < 100; i++)
            {

                Thread.Sleep(12000 / 100);
                mine.Percent = i;

            }
            mine.Percent = 100;
            e.Result = mine;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddBus window = new AddBus();
            window.ShowDialog();
            instance.checkStatus();
            Combofilter.SelectedIndex = 3;
            ListBus.DataContext = instance.GetBuses();
           
        }


        private void Update(object sender, RoutedEventArgs e)
        {
            int index = 0;
            bool flag = int.TryParse(SeatField.Text, out index);
            if (comp1 != DriverField.Text || comp2 != SeatField.Text)
            {
                Bus i = b;
                int verif;
                i.DriverName = DriverField.Text;
                bool essai = int.TryParse(SeatField.Text, out verif);
                if (!essai)
                {
                    MessageBox.Show("Only numbers !");
                }
                else
                {
                    instance.modifyBus(i);
                    MessageBox.Show("Commit saved !");
                    ListBus.SelectedItem = null;
                    myDetails.DataContext = null;
                    myData.DataContext = null;
                    updatebutton.IsEnabled = false;
                }
            
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
            this.Close();
           
        }
        private void changed(object sender, RoutedEventArgs e)
        {
            DeleteButton.IsEnabled = true;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as Bus;
            thisbus.CheckedOrNot = true;
            instance.modifyBus(thisbus);
            count++;
            
        }
        private void unchanged(object sender, RoutedEventArgs e)
        {
            count--;
           if(count==0)
                DeleteButton.IsEnabled = false;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as Bus;
            thisbus.CheckedOrNot = false;
            instance.modifyBus(thisbus);
        }

        private void Combofilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Combofilter.SelectedIndex == 0)
            {
                ListBus.DataContext = instance.getBusesFilteringbyFuel();
                myDetails.DataContext = null;
                inLicenseFilter = false;
                ListBus.Items.Refresh();
            }
            else if (Combofilter.SelectedIndex == 1)
            {
                ListBus.DataContext = instance.getBusesFilteringByKmC();
                myDetails.DataContext = null;
                ListBus.Items.Refresh();
                inLicenseFilter = false;
            }
            else if (Combofilter.SelectedIndex == 2)
            {
               ListBus.DataContext = instance.getBusesFilteringbyKm();
                myDetails.DataContext = null;
                ListBus.Items.Refresh();
                inLicenseFilter = false;
            }
            else
            {
                ListBus.DataContext = instance.GetBuses();
                myDetails.DataContext = myData.DataContext = null;
                inLicenseFilter = false;
                ListBus.Items.Refresh();
            }


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void seatpopup(object sender, TextChangedEventArgs e)
        {
            PopupDriver.IsOpen = false;
            PopupSeat.IsOpen = false;
        }
        private void driverpopup(object sender, TextChangedEventArgs e)
        {
            PopupDriver.IsOpen = false;
            PopupSeat.IsOpen = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LicenseTosearch.Text.Length != 0)
            {
                inLicenseFilter = true;
            }
            myDetails.DataContext = null;
            ListBus.DataContext = instance.getBusesFilteringBylicense(LicenseTosearch.Text);
            ListBus.Items.Refresh();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (instance.deleteBuses())
            {
                ListBus.DataContext = instance.GetBuses();
                MessageBox.Show("Bus(es) deleted successfully !");
                myData.DataContext = null;
                myDetails.DataContext = null;
                DeleteButton.IsEnabled = false;
                updatebutton.IsEnabled = false;
            }

          }



        

    
    }
}
