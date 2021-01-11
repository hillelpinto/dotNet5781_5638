using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace DAL.DO
{
    [Serializable]
    public class Bus: INotifyPropertyChanged,ICloneable
    {
        Random random = new Random();
        public DateTime startDate { get; set; }
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
        public int license { get; set; }
  
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
        public int ID { get; set; }
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
            return String.Format(license.ToString());
        }

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
                return null;
            }
        }

        public Bus(string str)
        {
            Fuel = 1200;
            startDate = new DateTime(random.Next(1995, 2018), random.Next(1, 13), random.Next(1, 28));
            Km = random.Next(0, 20000);
            Checkup = DateTime.Now;
            license = random.Next(1000000,10000000);
            KmAfterLastMaintenance = 0;
            DriverName = "";
            status = Status.Ready;

        }
        public Bus() {
          
        
        }
    }
}
