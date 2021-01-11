using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace BL.BO
{
    public class Bus: INotifyPropertyChanged
    {
        Random random = new Random();
        public DateTime startDate { get; set; }
        public int license { get; set; }
        private Status status;
        public string returnStatus
        {
            get => status.ToString();
            set
            {
                status = (Status)Enum.Parse(typeof(Status), value);
                OnPropertyChanged("returnStatus");
            }

        }
      
        public int Km
        {
            get;set;
        }
        public int Fuel
        {
            get;set;
        }

        
        private int percent = 100;
        public int Percent//Value of our progressbar need to be bind and changed in synhcronization from the mainWindow
        {
            get { return percent; }
            set
            {
                percent = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Percent"));

            }
        }
        private DateTime Maintenance;
        public DateTime Checkup
        {
            get { return Maintenance; }
            set
            {
                Maintenance = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Checkup"));

            }
        }
        public int SeatAvailable
        {
            get;set;
        }
        private int kmafterlastmaintenance;
        public int KmAfterLastMaintenance {
            get { return kmafterlastmaintenance; }
            set
            {
                kmafterlastmaintenance = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("KmAfterLastMaintenance"));

            }
        }
        public string GFormat
        {
            get
            {
                string License = license.ToString();
                string first, middle, last;
                if (License == null)
                    return null;
                if (License.Length == 7)
                {
                    // xx-xxx-xx
                    first = License.Substring(0, 2);
                    middle = License.Substring(2, 3);
                    last = License.Substring(5, 2);
                    return string.Format("{0}-{1}-{2}", first, middle, last);
                }
                else
                {
                    // xxx-xx-xxx
                    first = License.Substring(0, 3);
                    middle = License.Substring(3, 2);
                    last = License.Substring(5, 3);
                    return string.Format("{0}-{1}-{2}", first, middle, last);
                }
            }
            set { }

        }
        public string DriverName
        {
            get;set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public bool CheckedOrNot { get; set; }
        public override string ToString()
        {
            return String.Format(GFormat);
        }
        public Bus(string str)
        {
            Fuel = 1200;
            startDate = new DateTime(random.Next(1995, 2018), random.Next(1, 13), random.Next(1, 28));
            Km = random.Next(0, 20000); // 1 to 1000 km au compteur
            Checkup = DateTime.Now;
            license = random.Next(1000000,10000000);
            KmAfterLastMaintenance = 0;
            DriverName = "";
            status = Status.Ready;

        }
        public int ID { get; set; }

        public Bus() { }
    }
}
