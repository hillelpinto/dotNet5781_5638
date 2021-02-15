using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
    public class Trip
    {
        public int buslineNumber { get; set; }
        public TimeSpan time { get; set; }
        public override string ToString()
        {
            return string.Format("The line {0} make the trip in {1}", buslineNumber, time);
        }
        public Trip(Line l ,TimeSpan s)
        {
            buslineNumber = l.busLineNumber;
            time = s;
        }
    }
}
