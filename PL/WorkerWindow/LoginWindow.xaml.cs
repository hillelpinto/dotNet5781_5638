using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using BL;
using BL.BO;

namespace PL.WorkerWindow
{
    /// <summary>
    /// Logique d'interaction pour LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        BL.IBl instance = BLFactory.Instance;
        BackgroundWorker b = new BackgroundWorker();
        public LoginWindow()
        {
            InitializeComponent();
            //b.DoWork += bgw_DoWork;
            //b.RunWorkerCompleted += bgw_RunWorkerCompleted;
            Uri myiconWindow = new Uri("https://drive.google.com/uc?export=download&id=1hwgmilcmFib-ksoihuhaKbwrmDFguA0G", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(myiconWindow);


        }

       
        

        private void NeedAcount_Checked(object sender, RoutedEventArgs e)
        {
            pwdnew.Visibility = Visibility.Visible;
            newpwd.Visibility= Visibility.Visible; ;
            newmailaddress.Visibility= Visibility.Visible;
            newusername.Visibility= Visibility.Visible;
            txtboxnew.Visibility=Visibility.Visible;
            maillabel.Visibility= Visibility.Visible;
            signinbutton.Visibility= Visibility.Visible;
            userfield.Text = null;
            mypasswordBox.Password = null;
            resetpwd.Visibility = Visibility.Hidden;
            userfield.Visibility = Visibility.Hidden;
            usernamelabel.Visibility = Visibility.Hidden;
            resetpwd.Visibility = Visibility.Hidden;
            pwdlabel.Visibility = Visibility.Hidden;
            mypasswordBox.Visibility = Visibility.Hidden;
            loginbutton.Visibility = Visibility.Hidden;

        }
        private void signin(object sender, RoutedEventArgs e)
        {
            User a = new User();
            a.email = newmailaddress.Text;
            a.username = newusername.Text;
            a.pwd = newpwd.Password.ToString();
            if (newmailaddress.Text.Length == 0 || newusername.Text.Length == 0 || newpwd.Password.Length == 0)
                MessageBox.Show("Missing data !");
           else if (instance.isExists(a))
                MessageBox.Show("This account already exists !");
            else
            {
                instance.addUser(a);
                this.Close();
                Window1 window = new Window1();
                window.ShowDialog();
            }

        }

        private void NeedLogin_Checked(object sender, RoutedEventArgs e)
        {
            userfield.Visibility = Visibility.Visible;
            usernamelabel.Visibility= Visibility.Visible;
            pwdlabel.Visibility= Visibility.Visible;
            mypasswordBox.Visibility= Visibility.Visible;
            loginbutton.Visibility= Visibility.Visible;
            resetpwd.Visibility = Visibility.Visible;
           
            newpwd.Password = null;
            newusername.Text = null;
            newmailaddress.Text = null;
            pwdnew.Visibility = Visibility.Hidden;
            newpwd.Visibility = Visibility.Hidden;
            newmailaddress.Visibility = Visibility.Hidden;
            newusername.Visibility = Visibility.Hidden;
            txtboxnew.Visibility = Visibility.Hidden;
            maillabel.Visibility = Visibility.Hidden;
            signinbutton.Visibility = Visibility.Hidden;

        }
        private void Loginbutton(object sender, RoutedEventArgs e)
        {
            User a = new User();
            a.pwd = mypasswordBox.Password.ToString();
            a.username = userfield.Text;
            if (mypasswordBox.Password.Length == 0 || userfield.Text.Length == 0)
                MessageBox.Show("Missing data !");
            if (instance.isExists(a)&&instance.checkpwd(a))
            {
                this.Close();
                Window1 window = new Window1();
                window.ShowDialog();
            }
            else
                MessageBox.Show("ERROR");
        }
        private void resetPassword(object sender,RoutedEventArgs e)
        {
            User a = new User();
            a.username = userfield.Text;
            if (userfield.Text.Length == 0)
                MessageBox.Show("Type your username please");
            else if (instance.isExists(a))
            {
                try
                {
                    instance.resetpwd(a);
                    MessageBox.Show("Your password is now in your mail message");
                }
                catch(BLException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("You're not from us !");

        }
    }
}
