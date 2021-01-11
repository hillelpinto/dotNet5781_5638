using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
namespace DAL.DO
{
    [Serializable]
    public class StationLine:Station,ICloneable
    {
        private Random r = new Random();
       public int LineHere { get; set; }
        public string positioninmyLine { get; set; }
        public int nextStation { get; set; }
        public int prevStation { get; set; }
        public TimeSpan Temps { get; set; }
        public double Distance { get; set; }

        public StationLine()
        {
            
        }
     
    }
}
