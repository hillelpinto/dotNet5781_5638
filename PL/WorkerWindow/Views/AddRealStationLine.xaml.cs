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
using BL;
using BL.BO;
namespace PL.WorkerWindow.Views
{
    /// <summary>
    /// Logique d'interaction pour AddRealStationLine.xaml
    /// </summary>
    public partial class AddRealStationLine : Window
    {
        BL.IBl instance = BLFactory.Instance;
        public AddRealStationLine()
        {
            
            InitializeComponent();
           mycomboLine.ItemsSource = instance.getLines();
            mycomboStation.ItemsSource = instance.GetStations();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            StationLine s = instance.fromStation(instance.GetStations().ToList()[mycomboStation.SelectedIndex]);
            Random r = new Random();
            s.ID = r.Next(0, 10000);
            while (instance.getmyStationsLines().Exists(station => station.ID == s.ID))
            {
                s.ID = r.Next(0, 10000);
            }
            int test;
            bool check = int.TryParse(indexx.Text, out test);
            if (check)
            {
                s.LineHere = instance.getLines()[mycomboLine.SelectedIndex].ID;
                s.positioninmyLine = indexx.Text;
                //instance.addOneCouple(s, instance.getLines()[mycomboLine.SelectedIndex].listStations[int.Parse(indexx.Text)]);
                if (test - 1 >= 0)
                {
                    instance.addOneCouple(s, instance.getLines()[mycomboLine.SelectedIndex].listStations[test - 1]);

                }
                else
                    instance.addOneCouple(s, instance.getLines()[mycomboLine.SelectedIndex].listStations[instance.getLines()[mycomboLine.SelectedIndex].listStations.Count - 1]);

                instance.addStationl(s);
                MessageBox.Show("Station added successfully !");

                this.Close();
            }
            else
                MessageBox.Show("Eror of index !");
        }

        private void mycomboLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(mycomboLine.SelectedIndex!=-1)
            {
                Line toExcept = instance.getLines()[mycomboLine.SelectedIndex];
                mycomboStation.ItemsSource = instance.GetStations().Where(station => toExcept.listStations.Exists(stationS => stationS.shelterNumber == station.shelterNumber) == false);
            }
        }
    }
}
