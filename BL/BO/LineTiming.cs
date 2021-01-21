using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
namespace BL.BO
{
    public class LineTiming:INotifyPropertyChanged
    {
        List<LineTiming> mylist;
        public int ID { get; set; }
        public int BusLineNumber { get; set; }
        public int Freq { get; set; }
        private TimeSpan timeBeforeArrival;
        public TimeSpan TimeBeforeArrival
        {
            get { return timeBeforeArrival; }
            set
            {
                timeBeforeArrival = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TimeBeforeArrival"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan StartingAt { get; set; }
        public string lastStation { get; set; }
        public LineTiming() { }
        public LineTiming(Line l)
        {
            BusLineNumber = l.busLineNumber;
            StartingAt = l.BeginService;
            lastStation = l.listStations[l.listStations.Count - 1].address;
        }
        public IEnumerable<LineTiming> getmyTimings(IBl bl,StationLine i,List<ExitLine> myShedules,TimeSpan currentH)
        {
           mylist  = new List<LineTiming>();
            List<Line> lineofTheStation = i.myLines;
            for (int c = 0; c < lineofTheStation.Count; c++)//Those 3 for loops will help us to get the time value we need to caclulate the time that the bus will arrives at every station
            {
                lineofTheStation[c].listStations[0].Temps = new TimeSpan(0, 0, 0);

                for (int a = 1; a < lineofTheStation[c].listStations.Count; a++)
                {
                   
                    for (int b = 0; b < a; b++)
                    {
                      
                        lineofTheStation[c].listStations[a].Temps= lineofTheStation[c].listStations[a].Temps.Add(TimeSpan.FromMinutes(lineofTheStation[c].listStations[b].Temps.Minutes));
                        lineofTheStation[c].listStations[a].Temps = lineofTheStation[c].listStations[a].Temps.Add(TimeSpan.FromSeconds(lineofTheStation[c].listStations[b].Temps.Seconds));
                       

                    }

                }
            }
            lineofTheStation.ForEach(line => mylist.Add(new LineTiming(line)));
            foreach(LineTiming l in mylist)
            {
                int index = myShedules.FindIndex(item => item.IdBus == l.BusLineNumber);
                l.Freq = myShedules[index].FrequenceinMN;
            }//After this loop it only miss the time left before the bus will arrive
            foreach(Line l in lineofTheStation)
            {
                int index = mylist.FindIndex(item => item.BusLineNumber == l.busLineNumber);
                int index2 = l.listStations.FindIndex(station => station.shelterNumber == i.shelterNumber);
                int minuteElpasedFromLastDeparture = currentH.Minutes % mylist[index].Freq;
                if(minuteElpasedFromLastDeparture>l.listStations[index2].Temps.Minutes)//It means that the bus already passed in tgis station and we have to wait the next
                {
                    mylist[index].timeBeforeArrival = new TimeSpan(0, mylist[index].Freq - minuteElpasedFromLastDeparture + l.listStations[index2].Temps.Minutes, 60-currentH.Seconds);
                }
                else if (minuteElpasedFromLastDeparture < l.listStations[index2].Temps.Minutes)//It means that the bus already passed in tgis station and we have to wait the next
                {
                    mylist[index].timeBeforeArrival = new TimeSpan(0, l.listStations[index2].Temps.Minutes - minuteElpasedFromLastDeparture,60- currentH.Seconds);
                }
                else
                {
                    mylist[index].timeBeforeArrival = new TimeSpan(0, 0,60-currentH.Seconds);
                }
            }

            return from timing in mylist orderby timing.timeBeforeArrival.Minutes ascending select timing;
        }

    }
}
