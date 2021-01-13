using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using dotNet5781_02_5638;
namespace dotNet5781_03A_5638
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Line currentDisplayBusLine;        
        public HandleCollectionBus h;
        public MainWindow()
        {
            InitializeComponent();
            h = new HandleCollectionBus();                                  // initialisation of 10lines and 4stations/lines in the ctor
            cbBusLines.ItemsSource = h.listLine;
            cbBusLines.DisplayMemberPath = "BusLineNumber";
            Uri myiconbus = new Uri("https://drive.google.com/uc?export=download&id=1Zr869QzmcUEFupc0Lds2s_peichAIG4T", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconbus);                      // icon by link
        }

        private void cbBusLines_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ShowBusLine((cbBusLines.SelectedValue as Line).BusLineNumber);
        }
        private void ShowBusLine(int index)
        {
            currentDisplayBusLine = h[index];
            UpGrid.DataContext = currentDisplayBusLine.BusLineNumber;
            lbBusLineStations.DataContext = currentDisplayBusLine.listStations;
            tbArea.Text = h[index].getArea().ToString();

        }

        private void lbBusLineStations_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void tbArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            // print only
        }
    }
}
