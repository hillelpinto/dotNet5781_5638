using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
    public class Station
    {
        Random r = new Random();
        public bool CheckedOrNot { get; set; }
        public int ID{ get; set; }
        public int shelterNumber{ get; set;}
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string address { get; set; }
        public string stationName { get; set; }
        public bool HandicappedAccess { get; set; }
        public bool DigitPanel { get; set; }
        public override string ToString()
        {
            return String.Format("Station ID: {0}", shelterNumber);
        }
        public Station(string s)
        {
            shelterNumber = r.Next(1000, 10000);
            HandicappedAccess = true;
            DigitPanel = false;
            latitude = "0";
            longitude = "0";
            //latitude = r.NextDouble() * 2.3 + 31;
            //latitude = Math.Round(latitude, 5);
            //longitude = r.NextDouble() * 1.2 + 34.3;
            //longitude = Math.Round(longitude, 5);

        }
        public Station()
        {
            latitude = "0";
            longitude = "0";
        }
    }
  
}
