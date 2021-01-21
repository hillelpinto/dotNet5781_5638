using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BL;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour SimulationParameters.xaml
    /// </summary>
   
    public partial class SimulationParameters : Window
    {
        public bool flag = false;
        BL.IBl instance;

        public SimulationParameters(IBl b)
        {
            InitializeComponent();
            instance = b;
        }
        public void Validation(object sender, RoutedEventArgs e)
        {
            TimeSpan temp = new TimeSpan();
            bool check = TimeSpan.TryParse(Hours.Text + Minutes.Text + Seconds.Text, out temp);
            if (!check)
                MessageBox.Show("Error of time format !");
            int test = 0;
            check = int.TryParse(stoms.Text, out test);
            if (!check)
                MessageBox.Show("Error of rate format !");
           else
            {
                BackgroundWorker bg = new BackgroundWorker();
                flag = true;
                bg.DoWork += Simulator;
                bg.RunWorkerAsync();
                this.Close();

            }


        }

        private void Simulator(object sender, DoWorkEventArgs e)
        {
            TimeSpan myTime = new TimeSpan();
            int rate = 0;
            Dispatcher.Invoke(new Action(() => GetValue(out myTime, out rate)));
            instance.StartSimulator(myTime, rate, x =>
            {
                myTime = x;
                Dispatcher.BeginInvoke(new Action(() => hours.Content=myTime.ToString()));
            });
        }
        private void GetValue(out TimeSpan myTime,out int Rate)
        {
            myTime = new TimeSpan(int.Parse(Hours.Text), int.Parse(Minutes.Text), int.Parse(Seconds.Text));
            int.TryParse(stoms.Text,out Rate);
        }
      
    }
    
}
