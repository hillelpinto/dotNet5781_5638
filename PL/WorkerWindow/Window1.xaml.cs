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

namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            BusesWindow window = new BusesWindow();
            window.Show();
            this.Close();
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            LinesWindow window = new LinesWindow();
            window.Show();
            this.Close();
        }

        private void StationButton_Click(object sender, RoutedEventArgs e)
        {
           StationsWindow window = new StationsWindow();
            window.Show();
            this.Close();

        }
    }
}
