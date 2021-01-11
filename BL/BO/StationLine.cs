using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
    public class StationLine:Station
    {
        private Random r = new Random();
        public int LineHere { get; set; }
        public string positioninmyLine { get; set; }
        public int nextStation { get; set; }
        public int prevStation { get; set; }
        public TimeSpan Temps { get; set; }
        public double Distance { get; set; }
        public List<Line> myLines { get; set; }
        public List<Line> getmyLines()
        {
            return myLines;
        }
        public StationLine()
        {

            myLines = new List<Line>();
        }
     
    }
}
