using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;
namespace DAL.DO
{
    [Serializable]
    public class Station:ICloneable
    {
        Random r = new Random();
        public bool CheckedOrNot { get; set; }
       
        public int ID { get; set; }
        public int shelterNumber{ get; set;}
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string address { get; set; }
        public string stationName { get; set; }
        public bool HandicappedAccess { get; set; }
        public bool DigitPanel { get; set; }
        public override string ToString()
        {
            return String.Format("Shelter Number: {0}", shelterNumber);
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

        public Station(string s)
        {
            HandicappedAccess = true;
            DigitPanel = false;
          

        }
        public Station() { }
    }
  
}
