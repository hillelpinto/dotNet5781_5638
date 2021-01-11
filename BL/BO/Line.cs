using System;
using System.Collections.Generic;
using System.Text;


namespace BL.BO
{
    public class Line
    {
        Random r = new Random();
        public int ID { get; set; }
        public int busLineNumber { get; set; }
        public int firstStation { get; set; }
        public int lastStation { get; set; }
        public Area area { get; set; }
        public bool CheckedOrNot { get; set; }
        public string getArea { get => area.ToString(); set { } }
        public List<StationLine> listStations = new List<StationLine>();
        public TimeSpan BeginService { get; set; }
        public TimeSpan EndService { get; set; }
        public int speed { get; set; }
        public override string ToString()
        {
            return String.Format("{0}, Area :{1}", busLineNumber, getArea);
        }
        public Line()
        {
            
            BeginService = new TimeSpan(6, 0, 0);
            EndService = new TimeSpan(22, 0, 0);
           
           
        }
    }
}
