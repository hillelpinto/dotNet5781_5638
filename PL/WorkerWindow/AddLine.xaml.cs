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
using BL.BO;
using BL;
using System.Linq;

namespace PL.WorkerWindow
{
       
    /// <summary>
    /// Logique d'interaction pour AddLine.xaml
    /// </summary>
    public partial class AddLine : Window
    {
        BL.IBl instance = BLFactory.Instance;
        Line l = new Line();
        Random r = new Random();
        public AddLine()
        {

            InitializeComponent();

            this.DataContext = l;
            firststationcombo.ItemsSource = instance.GetStations();
            ComboArea.ItemsSource = Enum.GetValues(typeof(Area)).Cast<Area>().ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LicenseN.Text.Length == 0)
                MessageBox.Show("You have to type a line number !");
            else
            {
                int f = 0;
                bool check = int.TryParse(LicenseN.Text, out f);
                if (!check)
                {
                    MessageBox.Show("The bus line number must be an integer !");
                }
                else
                {
                    l.busLineNumber = f;
                    l.area = (Area)ComboArea.SelectedIndex;
                    StationLine first = instance.fromStation(instance.GetStations().ToList()[firststationcombo.SelectedIndex]);
                    first.ID = r.Next(0, 10000);
                    while(instance.getAllStationsLines().Exists(station=>station.ID==first.ID))
                    {
                        first.ID= r.Next(0, 10000);
                    }
                    first.LineHere = f;
                    l.CheckedOrNot = false;
                    l.firstStation = first.shelterNumber;                   
                     instance.addStationl(first);
                      StationLine last = instance.fromStation(instance.GetStations().ToList()[laststationcombo.SelectedIndex + 1]);
                      last.LineHere = f;
                      l.lastStation = last.shelterNumber;
                    last.ID = r.Next(0, 10000);
                    while (instance.getAllStationsLines().Exists(station => station.ID == last.ID))
                    {
                        last.ID = r.Next(0, 10000);
                    }
                    instance.addStationl(last);
                    instance.addOneCouple(first, last);     
                    instance.addLine(l);
                    this.Close();
                        
                  
                  
                }
            }
        }

        private void firststationcombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            laststationcombo.ItemsSource = instance.getStationLessOne(instance.getStation(firststationcombo.SelectedIndex).shelterNumber);
        }

        private void laststationcombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
