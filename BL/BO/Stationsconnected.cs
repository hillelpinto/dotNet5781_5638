using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
   public class Stationsconnected
    {
        public int ID { get; set; }
        public int numeroUno { get; set; }
        public int numeroDeuzio { get; set; }
        public float distance { get; set; }
        public TimeSpan timeBetween { get; set; }
        public bool sameZone; 
        public Stationsconnected() { }


    }
}
