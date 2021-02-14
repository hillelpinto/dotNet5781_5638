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
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour AddBus.xaml
    /// </summary>
    public partial class AddBus : Window
    {
        IBl instance;
        private Bus busToSend = null;
        public Bus BusToSend { get => busToSend; }
        bool flag = false;
        bool OldBus;
        public AddBus(IBl bl)
        {
            InitializeComponent();
            busToSend = new Bus();
            instance = bl;
            this.DataContext = busToSend;
            Old.IsChecked = true;
            OldBus = true;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {


            if (!flag || LicenseN.Text.Length == 0)
            {
                MessageBox.Show("You didn't put all the needed data !");
            }
            else if (OldBus && int.Parse(kmTotal.Text) == 0)
            {
                MessageBox.Show("You didn't put all the needed data !");
            }
            
            else
            {
                busToSend.Km += busToSend.KmAfterLastMaintenance;
                busToSend.Fuel = 1200;
                busToSend.startDate = (DateTime)startDate.SelectedDate;
               
                if (OldBus)
                {
                    busToSend.Checkup = (DateTime)CheckupD.SelectedDate;
                }
                if (startDate.SelectedDate.Value.Year >= 2018 && LicenseN.Text.Length != 8 || startDate.SelectedDate.Value.Year < 2018 && LicenseN.Text.Length != 7)
                {
                    MessageBox.Show("Invalid license format");
                }
                else

                {
                    try
                    {
                       
                        int test;
                        bool check = int.TryParse(LicenseN.Text, out test);
                        if (check)
                        {
                            busToSend.license = test;
                            instance.addBus(busToSend);
                            MessageBox.Show("Bus saved succcessfully");
                            this.Close();
                        }
                        else
                            MessageBox.Show("The license must be an integer !");
                    }
                    catch(BLException s)
                    {
                        MessageBox.Show(s.Message);
                    }
                }
            }
          
        }

        private void New_Checked(object sender, RoutedEventArgs e)
        {
            flag = true;
            CheckupD.Visibility = Visibility.Hidden;
            OldBus = false;
            KmAfterLastMaintenance.IsReadOnly = true;
            KmAfterLastMaintenance.Visibility = Visibility.Hidden;
            kmTotal.IsReadOnly = true;
            KmText.Visibility = Visibility.Hidden;
            KmAfterText.Visibility = Visibility.Hidden;
            CDText.Visibility = Visibility.Hidden;
            kmTotal.Visibility = Visibility.Hidden;
            OldBus = false;
            busToSend.Km = 0;
            busToSend.KmAfterLastMaintenance = 0;
            busToSend.Checkup = DateTime.Now;
            kmafterlabel.Visibility = Visibility.Hidden;
            kmlabel.Visibility = Visibility.Hidden;
            checkuplabel.Visibility = Visibility.Hidden;

        }
        private void Old_Checked(object sender, RoutedEventArgs e)
        {
            OldBus = true;
            flag = true;
            KmAfterLastMaintenance.IsReadOnly = false;
            kmTotal.IsReadOnly = false;
            KmAfterLastMaintenance.Visibility = Visibility.Visible;
            kmTotal.Visibility = Visibility.Visible;
            CheckupD.Visibility = Visibility.Visible;
            kmlabel.Visibility = Visibility.Visible;
            CDText.Visibility = Visibility.Visible;
            kmafterlabel.Visibility = Visibility.Visible;
            KmText.Visibility = Visibility.Visible;
            KmAfterText.Visibility = Visibility.Visible;
            checkuplabel.Visibility = Visibility.Visible;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
