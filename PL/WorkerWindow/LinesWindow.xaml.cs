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
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour LinesWindow.xaml
    /// </summary>
    public partial class LinesWindow : Window
    {
        BL.IBl instance = BLFactory.Instance;
        int count;
        int countS;
        public LinesWindow()
        {
           
            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);

            ListLine.DataContext = instance.getLines();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
            this.Close();
        }
        private void ListBus_SelectionDetail(object sender, MouseButtonEventArgs e)
        {
            if (ListLine.SelectedIndex != -1)
            {
                int index = ListLine.SelectedIndex;
                BL.BO.Line l = instance.getLines()[index];
                linepresented.Text = l.busLineNumber.ToString();
                instance.setIndexinLine(l);
                myDetails.DataContext = l.listStations;
                myDetails.Items.Refresh();
            }
        }
        private void stationchecked(object sender, RoutedEventArgs e)
        {
            DeleetStationbutton.IsEnabled = true;
            DeleetStationbutton.Background = Brushes.Aquamarine;
            var cb = sender as CheckBox;
            var thisStation = cb.DataContext as BL.BO.StationLine;
            if (instance.getLines()[ListLine.SelectedIndex].listStations.Count == 2)
            {
                MessageBox.Show("Can't have less than 2 stations !");
                DeleetStationbutton.IsEnabled = false;
                DeleetStationbutton.Background = null; cb.IsChecked = false;
                return;
            }
            else
            {
                thisStation.CheckedOrNot = true;
                instance.modifyStationline(thisStation);
                countS++;
            }
        }
        private void stationunchecked(object sender, RoutedEventArgs e)
        {
            countS--;
            if (countS == 0)
            {
                DeleetStationbutton.IsEnabled = false;
                DeleetStationbutton.Background = null;
                DeleetStationbutton.Foreground = Brushes.Black;

            }
            var cb = sender as CheckBox;
            var thisStation = cb.DataContext as BL.BO.StationLine;
            thisStation.CheckedOrNot = false;
            instance.modifyStationline(thisStation);
        }
        private void LineChecked(object sender, RoutedEventArgs e)
        {
            DeleteLineButton.IsEnabled = true;
            DeleteLineButton.Background = Brushes.Gray;
            DeleteLineButton.Foreground = Brushes.GreenYellow;
            var cb = sender as CheckBox;
            var thisbus = cb.DataContext as BL.BO.Line;
  
                thisbus.CheckedOrNot = true;
                instance.modifyLine(thisbus);
                count++;
            
        }
        private void LineUnchecked(object sender, RoutedEventArgs e)
        {
            count--;
            if (count == 0)
            {
                DeleteLineButton.IsEnabled = false;
                DeleteLineButton.Background = null;
                DeleteLineButton.Foreground = Brushes.Black;
            }
            else
            {
                var cb = sender as CheckBox;
                var thisLine = cb.DataContext as BL.BO.Line;
                thisLine.CheckedOrNot = false;
                instance.modifyLine(thisLine);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLine window = new AddLine();
            window.ShowDialog();
            linepresented.Text = null;
            ListLine.DataContext = instance.getLines();

        }

        private void DeleteLineButton_Click(object sender, RoutedEventArgs e)
        {
            var data = (BL.BO.Line)myData.DataContext;
            if (data != null && data.CheckedOrNot == true)
            {
                myData.DataContext = null;
                myDetails.Items.Refresh();
            }
            if (instance.deleteLines())
            {
                linepresented.Text = null;
                DeleteLineButton.Background = null;
                ListLine.DataContext = instance.getLines();
                DeleteLineButton.IsEnabled = false;
                DeleteLineButton.Foreground = Brushes.Black;
                myDetails.DataContext = null;
                ListLine.SelectedItem = null;
                MessageBox.Show("Your Line(s) is well deleted !");
             
            }
        }
        private void AddStationpressed(object sender, RoutedEventArgs e)
        {
            var cb = sender as Button;
            var thisline = cb.DataContext as BL.BO.Line;
            AddStationLine window = new AddStationLine(thisline);
            window.ShowDialog();
            ListLine.DataContext = instance.getLines();
            myData.DataContext =myDetails.DataContext= null;

        }

        private void DeleetStationbutton_Click(object sender, RoutedEventArgs e)
        {
            instance.deleteStationLine();
            DeleetStationbutton.Background = null;
            DeleetStationbutton.Foreground = Brushes.Black;
            DeleetStationbutton.IsEnabled = false;
            myDetails.DataContext = myData.DataContext = null;
            ListLine.DataContext = instance.getLines();
            MessageBox.Show("Your Station is well deleted from the Line");


        }
    }
}
