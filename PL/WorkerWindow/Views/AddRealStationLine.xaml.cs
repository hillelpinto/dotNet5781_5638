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
        BL.IBl instance;
        List<Station> finallist;
        public AddRealStationLine(IBl bl)
        {
            instance = bl;
            InitializeComponent();
           mycomboLine.ItemsSource = instance.getLines();
        }
        /// <summary>
        /// Adding the station
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            StationLine s = instance.fromStation(finallist[mycomboStation.SelectedIndex]);
           
          
            if (checkIndex(s))
            {
                s.LineHere = instance.getLines()[mycomboLine.SelectedIndex].ID;
                s.positioninmyLine = indexx.Text;
                instance.addStationl(s);
                
                s = instance.getAllStationsLines().Find(item => item.shelterNumber == s.shelterNumber&&item.LineHere==s.LineHere);
            

                //instance.addOneCouple(s, instance.getLines()[mycomboLine.SelectedIndex].listStations[int.Parse(indexx.Text)]);
                if (int.Parse(indexx.Text) - 1 >= 0)
                {
                    instance.addOneCouple(s, instance.getLines()[mycomboLine.SelectedIndex].listStations[int.Parse(indexx.Text) - 1]);

                }
                else 
                    instance.addOneCouple(s, instance.getLines()[mycomboLine.SelectedIndex].listStations[instance.getLines()[mycomboLine.SelectedIndex].listStations.Count - 1]);

                MessageBox.Show("Station added successfully !");

                this.Close();
            }
            else
                MessageBox.Show("Error of index !");
        }
        /// <summary>
        /// It checks if the index is correct acording to the stations present in the line
        /// </summary>
        bool checkIndex(StationLine i)
        {

            int test;
            bool check = int.TryParse(indexx.Text, out test);
            int count = instance.getLines()[mycomboLine.SelectedIndex].listStations.Count;
            if (check && test<=count)
                return true;
            else
                return false;
        }
        /// <summary>
        /// When we select a line ,we have to present the stations which's not already belong to the line
        /// </summary>

        private void mycomboLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mycomboLine.SelectedIndex != -1)
            {
                Line toExcept = instance.getLines()[mycomboLine.SelectedIndex];
                finallist = instance.GetStations().Where(station => toExcept.listStations.Exists(stationS => stationS.shelterNumber == station.shelterNumber) == false).ToList();
                mycomboStation.ItemsSource = finallist;
            }
        }
        private void stationchanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

    }
}
