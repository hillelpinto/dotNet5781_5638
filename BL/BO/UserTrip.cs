using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DO
{
    public class UserTrip
    {
        public int IdTrip { get; set; }
        public DateTime Entry { get; set; }
        public DateTime Exit { get; set; }
        public string username { get; set; }
        public int IdBus { get; set; }
        public int IdStationofStart { get; set; }
        public int IdStationofEnd { get; set; }

    }
}
