using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;
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
    /// Logique d'interaction pour AddStation.xaml
    /// </summary>
    public partial class AddStation : Window
    {
        BL.IBl instance;
        Station b = new Station();
        public AddStation(IBl bl)
        {
            InitializeComponent();
            instance = bl;
            this.DataContext = b;
        }
        bool isDigit()
        {
            return YesDigit.IsChecked==true;
        }

        bool isAccess()
        {
            return yesacessdisable.IsChecked == true;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            b.ID = r.Next(0, 10000);
            while(instance.GetStations().ToList().Exists(station=>station.ID==b.ID)==true)
           {
                b.ID = r.Next(0, 10000);
            }
            b.DigitPanel = isDigit();
            b.HandicappedAccess = isAccess();
            try
            {
                instance.addstation(b);
                MessageBox.Show("Your Station is added successfully !");
                this.Close();
            }
            catch(BLException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
