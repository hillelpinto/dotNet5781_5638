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
        /// <summary>
        /// When we have input a correct time we launch a thread which call a function which sets the hours in the system
        /// </summary>
        public void Validation(object sender, RoutedEventArgs e)
        {
            TimeSpan temp = new TimeSpan();
            bool check = TimeSpan.TryParse(Hours.Text + Minutes.Text + Seconds.Text, out temp);
            if (!check)
            {
                MessageBox.Show("Error of time format !");
                return;
            }
            int test = 0;
            check = int.TryParse(stoms.Text, out test);
            if (!check)
            {
                MessageBox.Show("Error of rate format !");
                return;
            }
            else
            {
                BackgroundWorker bg = new BackgroundWorker();
                flag = true;
                bg.DoWork += Simulator;
                bg.RunWorkerAsync();
                this.Close();

            }


        }
        /// <summary>
        /// The doWork function of the thread to launch the simulation
        /// </summary>

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
        /// <summary>
        /// It catch the data in hours field and also the rate
        /// </summary>
        private void GetValue(out TimeSpan myTime,out int Rate)
        {
            myTime = new TimeSpan(int.Parse(Hours.Text), int.Parse(Minutes.Text), int.Parse(Seconds.Text));
            int.TryParse(stoms.Text,out Rate);
        }
      
    }
    
}
