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
namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);
            Uri myl = new Uri("https://drive.google.com/uc?export=download&id=1FfUReOLnVnSq07QuIeJU-wdBk4oDiNX5", UriKind.RelativeOrAbsolute);
            var image = new BitmapImage(myl);
            myLogo.Source = image;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow mywindow = new LoginWindow();
            mywindow.ShowDialog();
        }
    }
}
