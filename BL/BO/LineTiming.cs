using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
    public class LineTiming
    {
        BL.IBl instance = BLFactory.Instance;

        public int ID { get; set; }
        public int BusLineNumber { get; set; }
        private int Freq { get; set; }
        public TimeSpan timeBeforeArrival { get; set; }
        public TimeSpan StartingAt { get; set; }
        public string lastStation { get; set; }
        public LineTiming() { }
        public LineTiming(Line l)
        {
            BusLineNumber = l.busLineNumber;
            StartingAt = l.BeginService;
            lastStation = l.listStations[l.listStations.Count - 1].address;
        }
        public List<LineTiming> getmyTimings(StationLine i,TimeSpan currentH)
        {
            List<LineTiming> mylist = new List<LineTiming>();
            List<Line> lineofStation = i.myLines;
            
            lineofStation.ForEach(line => mylist.Add(new LineTiming(line)));
           foreach(LineTiming l in mylist)
           {
                foreach(ExitLine exit in instance.getmySchedules())
                {
                    if (l.BusLineNumber == exit.IdBus)
                    {
                        l.Freq = exit.FrequenceinMN;
                        break;
                    }
                }
           }
           foreach(Line l in lineofStation)
            {
                for(int a=1;a<l.listStations.Count;a++)
                {
                    l.listStations[a].Temps =l.listStations[a].Temps.Add(l.listStations[a - 1].Temps);
                }
                l.listStations[0].Temps = new TimeSpan(0, 0, 0);

            }//Set the time all from the start



            foreach (Line l in lineofStation)

            {
                int index = mylist.FindIndex(item => item.BusLineNumber == l.busLineNumber);
                TimeSpan timeleft = new TimeSpan(0, 0, 0);
                foreach(StationLine s in l.listStations)
                {
                    if (s.shelterNumber == i.shelterNumber)
                    {
                        timeleft = s.Temps;
                        break;
                    }
                }
                int departures = currentH.Minutes%mylist[index].Freq;
                TimeSpan finalTime;
                if (departures>timeleft.Minutes)
                {
                     finalTime= new TimeSpan(0, mylist[index].Freq - departures+timeleft.Minutes, 0);
                }
                else
                {
                    finalTime = new TimeSpan(0, timeleft.Minutes - departures, 0);
                }
                mylist[index].timeBeforeArrival = finalTime;
                 
            }
           
           
           


            return mylist;
        }

    }
}
