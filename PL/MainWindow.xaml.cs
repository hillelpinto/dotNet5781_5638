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
using System.Windows.Shapes;
using PL.WorkerWindow;
using PL.TravelerWindow;
using BL;
namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IBl instance = BLFactory.Instance;
        SimulatorClock s = SimulatorClock.Instance;
        public MainWindow()
        {
            
           // instance.init();
            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            Uri myl = new Uri("https://drive.google.com/uc?export=download&id=1FfUReOLnVnSq07QuIeJU-wdBk4oDiNX5", UriKind.RelativeOrAbsolute);
            var image = new BitmapImage(myl);
            myLogo.Source = image;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          Window1 mywindow = new Window1(instance);
            mywindow.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PlanningTrip win = new PlanningTrip(instance);
            win.ShowDialog();
        }
    }
}
