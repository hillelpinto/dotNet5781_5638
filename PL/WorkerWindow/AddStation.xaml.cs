﻿using System;
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
        /// <summary>
        /// Event when we add the station: Check the input ,and also if its already exists then add it
        /// </summary>
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (LicenseN.Text.Length == 0 || Chauffeurname.Text.Length == 0)
                MessageBox.Show("Missing data !");
            else if (kmTotal.Text.Length>0&&Seat.Text.Length>0&&(!checkInteger(kmTotal.Text) || !checkInteger(Seat.Text)))
            {
                MessageBox.Show("Lont/Lat must be an integer !");
            }
            else if (!checkInteger(LicenseN.Text))
            {
                MessageBox.Show("Station's code must be an integer !");
            }
            else
            {

               
                b.DigitPanel = isDigit();
                b.HandicappedAccess = isAccess();
                try
                {
                    instance.addstation(b);
                    MessageBox.Show("Your Station is added successfully !");
                    this.Close();
                }
                catch (BLException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

        }
        bool checkInteger(string a)
        {
            int esai;
            return int.TryParse(a, out esai);
        }
    }
}
