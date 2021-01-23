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
        BL.IBl instance;
        Line temp;
        List<string> comboSource = new List<string>();

        public ScheduleLine(Line l,IBl b)
        {
            InitializeComponent();
            temp = l;
            instance = b;
            comboSource.Add("5");
            comboSource.Add("10");
            comboSource.Add("15");
            comboSource.Add("20");
            this.DataContext = l;
           int a = instance.getmySchedules().ToList().Find(item => item.IdBus==l.ID).FrequenceinMN;
            int index= comboSource.FindIndex(item => item == a.ToString());
            comboFrq.ItemsSource = comboSource;
            comboFrq.SelectedIndex = index;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan begin = new TimeSpan();
            TimeSpan end = new TimeSpan();
            bool check = TimeSpan.TryParse(begintxt.Text, out begin);
            bool check2 = TimeSpan.TryParse(endtxt.Text, out end);
            if (!check || !check2)
            {
                MessageBox.Show("Error of time format !");
                return;
            }
            else
            {
                int test = int.Parse(comboSource[comboFrq.SelectedIndex]);
                ExitLine s = instance.getmySchedules().ToList().Find(item => item.IdBus == temp.ID);
                s.Start = begin;
                s.End = end;
                s.FrequenceinMN = test;
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
