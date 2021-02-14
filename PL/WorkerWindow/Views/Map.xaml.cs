using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL.WorkerWindow.Views
{
    /// <summary>
    /// Logique d'interaction pour Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        public Map(string add)
        {
            InitializeComponent();

            webB.Navigated += webBrowser1_Navigated;
                webB.Navigate(add);
            
           
        }
        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {


            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fiComWebBrowser == null) return;


            object objComWebBrowser = fiComWebBrowser.GetValue(wb);

            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember(



            "Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });

        }


        void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {

            HideScriptErrors(webB,true);

        }
    }
}
