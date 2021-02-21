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

namespace PL.TravelerWindow
{
    /// <summary>
    /// Logique d'interaction pour PlanningTrip.xaml
    /// </summary>
    public partial class PlanningTrip : Window
    {
        IBl instance;
        public PlanningTrip(IBl bl)
        {
            InitializeComponent();
            instance = bl;
        }
       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int test;
            int test2;
            bool check = int.TryParse(DepartureStation.Text, out test);
            bool check2 = int.TryParse(ArrivalStation.Text, out test2); 
            StationLine departure = new StationLine();
            StationLine arrival = new StationLine();
            if (check)
            {
                departure = instance.getAllStationsLines().Find(item => item.shelterNumber == test);
                if (departure==null)
                {
                    MessageBox.Show("Station of departure not found !");
                   
                }
            }
            else
            {
                departure = instance.getAllStationsLines().Find(item => item.address == DepartureStation.Text);
                if (departure == null)
                {
                    MessageBox.Show("Station of departure not found !");
                  
                }

            }
            if(check2&&departure!=null)
            {
                arrival = instance.getAllStationsLines().Find(item => item.shelterNumber == test2 );
                if (arrival == null)
                {
                    MessageBox.Show("Station of arrival not found !");
                }
               
            }
            if(departure!=null)
            {
                arrival = instance.getAllStationsLines().Find(item => item.address == arrival.address);
                if (arrival == null)
                {
                    MessageBox.Show("Station of arrival not found !");
                }
                else
                {
                    if (departure.shelterNumber == arrival.shelterNumber)
                        MessageBox.Show("Departure must be different from arrival !");
                    else
                        myList.DataContext = instance.getmyTrips(departure, arrival);
                }
            }



        }
    }
}
