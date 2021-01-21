using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DO
{
   public class Stationsconnected
    {
        Random r = new Random();
        public int ID { get; set; }
        public int numeroUno { get; set; }
        public int numeroDeuzio { get; set; }
        public float distance { get; set; }
        public TimeSpan timeBetween { get; set; }
        public bool sameZone; 
        public Stationsconnected(StationLine s,StationLine i)
        {
            numeroUno = s.ID;
            numeroDeuzio = i.ID;
            distance = r.Next(100, 600);
            if (distance > 300)
            {

                timeBetween = new TimeSpan(0, r.Next(3, 6), r.Next(0, 60));
            }
            else
                timeBetween = new TimeSpan(0, r.Next(0, 3), r.Next(0, 60));
        }
        public Stationsconnected()
        { }


    }
}
