using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BL;
using BL.BO;
namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour ScheduleLine.xaml
    /// </summary>
    public partial class ScheduleLine : Window
    {
        BL.IBl instance = BLFactory.Instance;
        Line temp;
        List<string> comboSource = new List<string>();

        public ScheduleLine(Line l)
        {
            InitializeComponent();
            temp = l;
            comboSource.Add("5");
            comboSource.Add("10");
            comboSource.Add("15");
            comboSource.Add("20");
            this.DataContext = l;
           int a = instance.getmySchedules().ToList().Find(item => item.IdBus == l.busLineNumber).FrequenceinMN;
            int index= comboSource.FindIndex(item => item == a.ToString());
            comboFrq.ItemsSource = comboSource;
            comboFrq.SelectedIndex = index;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan begin = new TimeSpan();
            bool check = TimeSpan.TryParse(begintxt.Text, out begin);
            if (!check)
                MessageBox.Show("Error of time format !");
            check = TimeSpan.TryParse(endtxt.Text, out begin);

             if (!check)
                MessageBox.Show("Error of time format !");
            int test = 0;
            check = int.TryParse(comboSource[comboFrq.SelectedIndex], out test);
            if(!check)
                MessageBox.Show("Error of time format !");
            else
            {
                temp.BeginService = TimeSpan.Parse(begintxt.Text);
                temp.EndService = TimeSpan.Parse(endtxt.Text);
                ExitLine s = instance.getmySchedules().ToList().Find(item => item.IdBus == temp.busLineNumber);
                s.FrequenceinMN = test;
                instance.modifyLine(temp);
                instance.modifySchedule(s);
                this.Close();

            }



        }
        private void quit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
