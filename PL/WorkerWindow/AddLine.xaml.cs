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
        BL.IBl instance;
        Line l = new Line();
        Random r = new Random();
        public AddLine(IBl bl)
        {

            InitializeComponent();
            instance = bl;
            this.DataContext = l;
            firststationcombo.ItemsSource = instance.GetStations();
            laststationcombo.ItemsSource = instance.GetStations();

            ComboArea.ItemsSource = Enum.GetValues(typeof(Area)).Cast<Area>().ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (firststationcombo.SelectedIndex == laststationcombo.SelectedIndex)
                MessageBox.Show("You have to select 2 different station !");
            else if (LicenseN.Text.Length == 0)
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
                    l.ID = r.Next(0, 10000);
                    while (instance.getAllAllLine().ToList().Exists(line => line.ID == l.ID))
                        l.ID = r.Next(0, 10000);

                    l.area = (Area)ComboArea.SelectedIndex;
                    StationLine first = instance.fromStation(instance.GetStations().ToList()[firststationcombo.SelectedIndex]);
                   
                    first.LineHere = l.ID;
                    l.CheckedOrNot = false;
                    l.firstStation = first.shelterNumber;                   
                     instance.addStationl(first);
                     StationLine last = instance.fromStation(instance.GetStations().ToList()[laststationcombo.SelectedIndex]);
                     last.LineHere = l.ID;
                    l.lastStation = last.shelterNumber;
                  
                    instance.addStationl(last);
                    first = instance.getAllStationsLines().Find(item => item.shelterNumber == first.shelterNumber);
                    last = instance.getAllStationsLines().Find(item => item.shelterNumber == last.shelterNumber);

                    instance.addOneCouple(first, last);
                    instance.addOneCouple(last, first);

                    try
                    {
                        instance.addLine(l);
                        MessageBox.Show("Line added successfully !");
                        this.Close();
                    }
                    catch(BLException b)
                    {
                        MessageBox.Show(b.Message);
                    }
                  
                  
                }
            }
        }

     
    }
}
