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
using PL.WorkerWindow.ViewModels;
using BL;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour StationsWindow.xaml
    /// </summary>
    public partial class StationsWindow : Window
    {
        
        List<string> combosource = new List<string>();
        public StationsWindow()
        {
          
            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            combosource.Add("Stations Autobus");
            combosource.Add("Stations of Lines");
            comboChoice.ItemsSource = combosource;
            comboChoice.SelectedIndex = 0;
            DataContext = new StationModels();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
            this.Close();

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboChoice.SelectedIndex == 0)
                DataContext = new StationModels();
            else if (comboChoice.SelectedIndex == 1)
            {
                DataContext = new StationLineModels();
            }
           
        }
    }
}
