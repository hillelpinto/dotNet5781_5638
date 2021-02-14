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
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour AddStationLine.xaml
    /// </summary>
    public partial class AddStationLine : Window
    {
        BL.IBl instance;
        BL.BO.Line temp = new BL.BO.Line();
        Random r = new Random();
        List<Station> myList;
        public AddStationLine(BL.BO.Line l,IBl b)
        {
           myList = new List<Station>();
            instance = b;
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            temp = l;
            myList = instance.GetStations().Where(station => l.listStations.Exists(stationL => stationL.shelterNumber == station.shelterNumber) == false).ToList();//Get all the stations except the stations already in the line
            InitializeComponent();
            mycombo.ItemsSource = myList;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StationLine s = instance.fromStation(instance.GetStations().ToList().Find(station => station.shelterNumber == myList[mycombo.SelectedIndex].shelterNumber));
            int num = s.shelterNumber;
           
            s.LineHere = temp.ID;
          
            int test;
            bool check = int.TryParse(indexx.Text, out test);
            if(check)
            {
                if (test > temp.listStations.Count)
                    MessageBox.Show("Error of index !");
                else
                {
                    s.positioninmyLine = test.ToString();
                    instance.addStationl(s);
                    s = instance.getAllStationsLines().Find(item => item.shelterNumber == s.shelterNumber&&item.LineHere==temp.ID);

                    if (test - 1 >= 0)
                    {
                        instance.addOneCouple(s, temp.listStations[test - 1]);

                    }
                    else
                        instance.addOneCouple(s, temp.listStations[temp.listStations.Count - 1]);
                    
                    MessageBox.Show("Station added successfully !");
                }

            }
            else
            {
                MessageBox.Show("The index must be an integer !");
            }
           
            this.Close();
        }
    }
}
