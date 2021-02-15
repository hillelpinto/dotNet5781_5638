using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using BL.BO;
using DAL;
using System.Net.Mail;
using System.Collections;
using System.Net;
using System.Xml.Linq;

namespace BL
{

    public class MyBL : IBl
    {
        IDAL dalData = DALFactory.GetDL("data");
        TimeSpan hours;

        public void init()
        {
            dalData.init();
        }
        #region BusFunction

        public BO.Bus convertBusToBO(DAL.DO.Bus a)
        {
            Bus b = new Bus();
            a.CopyPropertiesTo(b);
            return b;
        }

        public void checkStatus()
        {
            dalData.checkstatus();

        }
        public void addBus(Bus a)
        {
            try
            {
                DAL.DO.Bus b = new DAL.DO.Bus();
                a.CopyPropertiesTo(b);
                dalData.addBus(b);
            }
            catch(DLException ex)
            {
                throw new BLException(ex.Message);
            }
        }
        public List<Bus> getBusesFilteringbyFuel()
        {
            List<Bus> ListFiltered = GetBuses();
            var toreturn = from bus in ListFiltered orderby bus.Fuel descending select bus;
            return toreturn.ToList();
        }
        public List<Bus> getBusesFilteringByKmC()
        {
            var Listfiltered = from bus in GetBuses() orderby bus.KmAfterLastMaintenance descending select bus;

            return Listfiltered.ToList();
        }
        public List<Bus> getBusesFilteringbyKm()
        {
            IEnumerable<Bus> Listfiltered = from bus in GetBuses() orderby bus.Km descending select bus;
            return Listfiltered.ToList();
        }
        public List<Bus> getBusesFilteringBylicense(string a)
        {
            bool check = GetBuses().Exists(item => item.license.ToString().Substring(0, a.Length) == a);
            if (!check)
                throw new BLException("This license number doesn't exists ,try again !");
            IEnumerable<Bus> Listfiltered = GetBuses().Where(bus => bus.license.ToString().Substring(0, a.Length) == a);
            return Listfiltered.ToList();

        }
        public List<Bus> GetBuses()
        {
            List<DAL.DO.Bus> myList = dalData.getmyBuses().ToList();
            List<Bus> MyList = new List<Bus>();
            foreach (DAL.DO.Bus bus in myList)
            {
                MyList.Add(convertBusToBO(bus));
            }
            return MyList;
        }
     
        public bool deleteBuses()
        {
            return dalData.deletebuses();

        }
      
        public void modifyBus(Bus b)
        {

            DAL.DO.Bus c = new DAL.DO.Bus();
            b.CopyPropertiesTo(c);
            dalData.modifyBus(c);
        }
        #endregion

        #region LineFunction

        public void modifyLine(Line l)
        {
            DAL.DO.Line s = Deepcopy.convertToDOLine(l);
            dalData.modifyLine(s);
        }
    

        public List<Line> getLines()//It search all the lines saved and add it all the stationLine which passing through 
        {
            List<Line> t = new List<Line>();
            dalData.getLines().ToList().ForEach(line => t.Add(Deepcopy.convertToBOLine(line)));
            foreach(Line l in t.ToList())
            {
                foreach(StationLine s in getmyStationsLines())
                {
                    if (s.LineHere == l.ID)
                    {
                        if (s.positioninmyLine!=null)
                        {
                            l.listStations.Insert(int.Parse(s.positioninmyLine), s);
                        }
                        else
                        {
                            l.listStations.Add(s);
                        }
                    }
                }
                if (l.listStations.Count() >= 2)
                {

                    setIndexinLine(l);
                }
                else
                {
                    l.listStations[0].CheckedOrNot = true;
                    modifyStationline(l.listStations[0]);
                    l.CheckedOrNot = true;
                    modifyLine(l);
                    t.Remove(l);
                }
                getMyHours(l);
                l.listStations[l.listStations.Count - 1].Distance = 0;

                l.listStations[l.listStations.Count - 1].Temps = new TimeSpan(0,0,0);

            }


            return t;
        }
      

        public void setIndexinLine(Line l)//It set al the needed value in a line (distance,time,first station,last station..) 
            {
            
                for (int a = 0; a < l.listStations.Count(); a++)
                {
                
                    if (a == 0)
                    {
                        l.listStations[a].prevStation = 0;
                       
                    }
                    else
                    {
                        l.listStations[a].prevStation = l.listStations[a - 1].shelterNumber;
                    }
                    l.listStations[a].positioninmyLine = a.ToString();

                    if (a == l.listStations.Count() - 1)
                        l.listStations[a].nextStation = l.listStations[0].shelterNumber;
                    else
                        l.listStations[a].nextStation = l.listStations[a + 1].shelterNumber;
                }
            l.listStations[l.listStations.Count - 1].Distance = 0;

            l.listStations[l.listStations.Count - 1].Temps = new TimeSpan(0, 0, 0);
            if (l.listStations.Count != 0)
                {
                    l.firstStation = l.listStations[0].shelterNumber;
                    l.lastStation = l.listStations[l.listStations.Count - 1].shelterNumber;
                }
                else
                {
                    l.firstStation = 0;
                    l.lastStation = 0;
                }

            }
            public void addLine(Line l)
            {
            try
            {
                DAL.DO.Line s = new DAL.DO.Line();
                l.CopyPropertiesTo(s);

                ExitLine schedule = new ExitLine();
                schedule.IdBus = l.ID;
                addLineSchedule(schedule);
                dalData.addLine(s);
            }
            catch(DLException ex)
            {
                throw new BLException(ex.Message);
            }
            }
        public void getMyHours(Line l)
        {

            int index = dalData.getmySchedules().ToList().FindIndex(item => item.IdBus == l.ID);
            if (index != -1)
            {
                l.BeginService = getmySchedules().ToList()[index].Start;
                l.EndService = getmySchedules().ToList()[index].End;
               
            }
        }
        public void addLineSchedule(ExitLine l)
        {
            DAL.DO.ExitLine S = new DAL.DO.ExitLine();
            l.CopyPropertiesTo(S);
            S.IdBus = l.IdBus;
            dalData.addLineSchedule(S);
        }

        public IEnumerable<Line> getAllAllLine()//get the line without the stations in each of them
        {
            List<Line> t = new List<Line>();
            dalData.getAllAllLine().ToList().ForEach(line => t.Add(Deepcopy.convertToBOLine(line)));
            return t;
        }
        //public List<Line> GetLinewithoutSchedules()
        //{

        //}


        public bool deleteLines()//It delete all the stations in the line too
            {
            IEnumerable<Line> lineInDeletion = getAllAllLine().ToList().Where(line => line.CheckedOrNot == true).ToList();
            IEnumerable<StationLine> deleteStationFirst = getmyStationsLines().Where(station => lineInDeletion.ToList().Exists(line => line.ID == station.LineHere));
            deleteStationFirst.ToList().ForEach(station => station.CheckedOrNot = true);
            deleteStationFirst.ToList().ForEach(station => modifyStationline(station));
         
       
                return dalData.DeleteLines();
            }

            #endregion

        #region StationLineFunction
            public StationLine convertSLinetoBO(DAL.DO.StationLine s)
            {
                StationLine l = new StationLine();
      
                s.CopyPropertiesTo(l);
           
            l.LineHere = s.LineHere;
            l.ID = s.ID;
            return l;
            }
        public DAL.DO.StationLine convertSLinetoDO(StationLine s)
        {
            DAL.DO.StationLine l = new DAL.DO.StationLine();

            s.CopyPropertiesTo(l);
            l.LineHere = s.LineHere;
            l.ID = s.ID;
            return l;
        }
        public List<StationLine> getAllStationsLines()
        {
            List<StationLine> toReturn = new List<StationLine>();
            dalData.getAllStationsLines().ForEach(station => toReturn.Add(convertSLinetoBO(station)));
            return toReturn;
        }

            public bool deleteStationLine()
            {

                return dalData.deleteStationLine();
            }
            public List<StationLine> getmyStationsLines()//It returns all the stationsLine with setting it time and distance acording to stationsConected 
            {
                List<StationLine> list = new List<StationLine>();
            
            dalData.getstationslines().ForEach(station => list.Add(convertSLinetoBO(station)));
            list.ForEach(station => getmyTime(station));

           
            return list.ToList();
            }

        public void getmyTime(StationLine i)
        {
            int index = getStationConnected().ToList().FindIndex(station => station.numeroUno == i.ID);
            if (index != -1)
            {
                i.Distance = getStationConnected().ToList()[index].distance;

                i.Temps = getStationConnected().ToList()[index].timeBetween;
            }
        }
        public StationLine findlineForStation(StationLine i)//This function will give all the line pasing through a station
        {
            List<StationLine> ss = getmyStationsLines().Where(station => station.shelterNumber == i.shelterNumber).ToList();
            StationLine s = i;
            foreach(StationLine l in ss)
            {
                s.myLines.Add(getLines().Find(line => line.ID == l.LineHere));
            }
            
            return s;
        }


            public StationLine fromStation(Station l)//This function returns the data corresponding to the physical station ,in order to create a stationLine corresponding to it
            {
                StationLine b = new StationLine();
                b.shelterNumber = l.shelterNumber;
                b.longitude = l.longitude;
                b.latitude = l.latitude;
                b.DigitPanel = l.DigitPanel;
                b.HandicappedAccess = l.HandicappedAccess;
                b.address = l.address;
                return b;

            }
            public void addStationl(StationLine l)
            {
           
                DAL.DO.StationLine s = new DAL.DO.StationLine();
                l.CopyPropertiesTo(s);
                dalData.addStationL(s);
         
            }
            public void modifyStationline(StationLine l)//
            {
            List<StationLine> tonotDelete = new List<StationLine>();
            tonotDelete = getAllStationsLines().Where(station => station.shelterNumber == l.shelterNumber).ToList();
            tonotDelete.ForEach(station => station.CheckedOrNot = l.CheckedOrNot);
            tonotDelete.ForEach(station => station.address = l.address);
            List<DAL.DO.StationLine> cloning = new List<DAL.DO.StationLine>();
            tonotDelete.ForEach(station => cloning.Add(convertSLinetoDO(station)));
            cloning.ForEach(station => dalData.modifystationline(station));
            }
        public void modifyOnlyOneStation(StationLine l)
        {
            DAL.DO.StationLine s = convertSLinetoDO(l);
            dalData.modifystationline(s);
        }

        public bool isStationLinexists(StationLine l)
            {
                return dalData.getstationslines().Exists(station => station.shelterNumber == l.shelterNumber);
            }
           

            #endregion

        #region StationFunction
            public Station convertStationtoBo(DAL.DO.Station s)
            {
                Station a = new Station();
                s.CopyPropertiesTo(a);

                return a;
            }
            public IEnumerable<Station> GetStations()
            {
            List<Station> temp = new List<Station>(); 
                dalData.GetStation().ToList().ForEach(station => temp.Add(convertStationtoBo(station)));
            IEnumerable<Station> myList = temp.Where(station => station.CheckedOrNot == false);
                return myList;
            }
            public IEnumerable<Station> getStationLessOne(int a)
            {
                IEnumerable<Station> mystation = GetStations().Where(station => station.shelterNumber != a);
                return mystation;

            }
           
            public bool deleteStations()
            {
                return (dalData.deleteStations());
            }
            public Station getStation(int a)
            {
                if (a == -1)
                    throw new Exception("Double click on a station !");
                else

                {
                    List<Station> mylist = GetStations().ToList();
                    return (mylist[a]);
                }
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
            public void addstation(Station a)
            {
            try
            {
                DAL.DO.Station s = new DAL.DO.Station();
                a.CopyPropertiesTo(s);
                s.longitude = a.longitude;
                s.latitude = a.latitude;
                if(s.longitude=="0"||s.latitude=="0")
                {
                    setLontLat(s);
                }
                dalData.addStation(s);
            }
            catch(DLException ex)
            {
                throw new BLException(ex.Message);
            }
            }

        void setRandomValue(DAL.DO.Station s)
        {
            Random r = new Random();
            double latitude;
            double longitude;
            latitude = r.NextDouble() * 2.3 + 31;
            latitude = Math.Round(latitude, 7);
            longitude = r.NextDouble() * 1.2 + 34.3;
            longitude = Math.Round(longitude, 7);
            s.latitude = latitude.ToString();
            s.longitude = longitude.ToString();
        }
        void setLontLat(DAL.DO.Station s)
        {
            bool inIsrael = false;

            string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(s.address), "AIzaSyABpOYvrsQpVn7Nm48o5fJ3Fuj7salDLN4");
            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());
            XElement check = xdoc.Element("GeocodeResponse").Element("status");
            if (check.Value != "ZERO_RESULTS")//We have to check if the address is real 
            {
                XElement lastcheck = xdoc.Element("GeocodeResponse").Element("result");
                var country = lastcheck.Elements("address_component");
                foreach (XElement str in country)
                {
                    if (str.Element("short_name").Value == "IL")
                    {
                        inIsrael = true;
                    }

                }
                if (inIsrael)
                {
                    XElement result = xdoc.Element("GeocodeResponse").Element("result");
                    XElement locationElement = result.Element("geometry").Element("location");
                    s.latitude = locationElement.Element("lat").Value;
                    s.longitude = locationElement.Element("lng").Value;
                }
                else
                {
                    setRandomValue(s);
                }
            }
            else
            {
                setRandomValue(s);
            }
        }
       public void modifyStation(Station a)
       {
            if (getmyStationsLines().Exists(station => station.shelterNumber == a.shelterNumber))
            {
                StationLine station = getmyStationsLines().Find(sl=> sl.shelterNumber == a.shelterNumber);
                station.address = a.address;
                modifyStationline(station);
            }
           
            DAL.DO.Station s = new DAL.DO.Station();
            a.CopyPropertiesTo(s);
            setAddress(s);
            dalData.modifyStation(s);
       }
        void setAddress(DAL.DO.Station s)
        {
            DAL.DO.Station temp = dalData.GetStation().ToList().Find(item => item.ID == s.ID);
            if(s.address!=temp.address)
            {
                setLontLat(s);

            }
        }
        #endregion

        #region Simulation
        public void StartSimulator(TimeSpan startTime, int Rate, Action<TimeSpan> updateTime)
        {
            SimulatorClock.Instance.Cancel = false;

            SimulatorClock simulatorClock = SimulatorClock.Instance;
            simulatorClock.Rate = Rate;
            simulatorClock.stopWatch.Restart();
            simulatorClock.ClockObserver += updateTime;
            while (simulatorClock.Cancel != true)
            {
                simulatorClock.Time = startTime + new TimeSpan(simulatorClock.stopWatch.ElapsedTicks * simulatorClock.Rate);
                if (simulatorClock.Time.Hours == 23 && simulatorClock.Time.Minutes == 59 && simulatorClock.Time.Seconds == 59)
                    simulatorClock.Time = new TimeSpan(0, 0, 0);
                hours = simulatorClock.Time;
                Thread.Sleep(100);
            }

        }
        public TimeSpan getHours()
        {
            return hours;
        }
        public void StopSimulator()
        {
            SimulatorClock.Instance.Cancel = true;
            SimulatorClock.Instance.Time = new TimeSpan(0, 0, -1);
        }
 
      

   


        #endregion

        #region StationConnected

        public IEnumerable<Stationsconnected> getStationConnected()
        {
            List<Stationsconnected> toreturn = new List<Stationsconnected>();
            dalData.getStationConnected().ToList().ForEach(station =>toreturn.Add(Deepcopy.convertSConnectedTOBO(station)));
            IEnumerable<Stationsconnected> end = toreturn;
            return end;

        }
        public bool commitDistanceTime(Stationsconnected s)
        {
            DAL.DO.Stationsconnected l = new DAL.DO.Stationsconnected();
            l.numeroUno = s.numeroUno;
            l.numeroDeuzio = s.numeroDeuzio;
            l.ID = s.ID;
            l.timeBetween = s.timeBetween;
            l.distance = s.distance;
          
           
            return dalData.commitTime(l);
        }
      

        public void addOneCouple(StationLine s,StationLine l)
        {
            DAL.DO.StationLine temp = new DAL.DO.StationLine();
            DAL.DO.StationLine tempp = new DAL.DO.StationLine();
            s.CopyPropertiesTo(temp);
            l.CopyPropertiesTo(tempp);
        
            dalData.addOnecouple(temp,tempp);
        }
        #endregion

        #region User

        public User convertUserToBO(DAL.DO.User u)
        {
            User s = new User();
            u.CopyPropertiesTo(s);
            return s;
        }
        public void addUser(User u)
            {
                DAL.DO.User b = new DAL.DO.User();
                u.CopyPropertiesTo(b);
                dalData.addUser(b);
            }

        public IEnumerable<User> getmyUser()
        {
            List<User> toR = new List<User>();
            dalData.getmyUsers().ToList().ForEach(user => toR.Add(convertUserToBO(user)));
            return toR;
        }

        public void deleteUser(User u)
            {
                DAL.DO.User b = new DAL.DO.User();
                u.CopyPropertiesTo(b);
                dalData.deleteUser(b);
            }
            public bool isExists(User u)
            {
          
            
                return dalData.getmyUsers().ToList().Exists(user => user.username == u.username);
            }
            public bool checkpwd(User a)
            {
                User b = new User();
                b.pwd = dalData.getmyUsers().ToList().Find(user => user.username == a.username).pwd;
                if (b.pwd == a.pwd)
                    return true;
                else
                    return false;
            }
            public void resetpwd(User j)
            {
                DAL.DO.User a = new DAL.DO.User();
                j.CopyPropertiesTo(a);
            int index = a.ID;
            index = j.ID;
                DAL.DO.User b = dalData.getmyUsers().ToList().Find(user => user.username == a.username);
                string pass = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                string newpwd = "";
                Random r = new Random();
                for (int i = 0; i < 8; i++)
                    newpwd += pass[r.Next(0, 52)];
                b.pwd = newpwd;
                try
                {
                    using (MailMessage mymessage = new MailMessage())
                    {
                        mymessage.From = new MailAddress("hillelpinto5@gmail.com", "From MoovitProject");

                        mymessage.To.Add(b.email);
                        mymessage.Subject = "Reinitializing your password";
                        mymessage.Body = string.Format("Hi {0} here's your new password :<b> {1} </b> ,it let you access again to our system , try it !", a.username, b.pwd);
                        mymessage.IsBodyHtml = true;
                        using (SmtpClient smpt = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smpt.Credentials = new System.Net.NetworkCredential("hillelpinto5@gmail.com", "Hhillel2652");
                            smpt.EnableSsl = true;

                            smpt.Send(mymessage);

                        }
                        dalData.deleteUser(a);
                        dalData.addUser(b);
                    }
                }
                catch (Exception )
                {
                    b.pwd = "0000";
                    dalData.deleteUser(a);
                    dalData.addUser(b);
              
                    throw new BLException("Your mail is not valid we can't send you a new password so by default it's 0000 now !");
                }
            }
        #endregion


        #region Schedule
        public ExitLine convertSchedule(DAL.DO.ExitLine s)
        {
            ExitLine l = new ExitLine();
            s.CopyPropertiesTo(l);
            return l;
        }
        public IEnumerable<ExitLine> getmySchedules()
        {
            IEnumerable<DAL.DO.ExitLine> mylist = dalData.getmySchedules();
            List<ExitLine> toreturn = new List<ExitLine>();
            mylist.ToList().ForEach(item => toreturn.Add(convertSchedule(item)));
            return toreturn;
        }
        public void modifySchedule(ExitLine s)
        {
            DAL.DO.ExitLine l = new DAL.DO.ExitLine();
            s.CopyPropertiesTo(l);
            dalData.modifySchedule(l);
        }


        #endregion



        #region TripClient
        public List<Trip> getmyTrips(StationLine depart,StationLine arrive)
        {
            List<Trip> myList = new List<Trip>();
            IEnumerable<Line> myLines = getLines().Where(line => line.listStations.Exists(station => station.shelterNumber == depart.shelterNumber));
            List<Line> myRealLine = myLines.ToList();
            foreach(Line l in myRealLine.ToList())
            {
                int firstIndex =l.listStations.FindIndex(station => station.shelterNumber == depart.shelterNumber);
                int lastindex = l.listStations.FindIndex(station => station.shelterNumber == arrive.shelterNumber);
                if(lastindex!=-1)
                {
                    if (lastindex < firstIndex)
                        myRealLine.Remove(l);
                }
            }
            List<Trip> ListToreturn = new List<Trip>();
            myRealLine.ForEach(item => ListToreturn.Add(new Trip(item, HelptheTrip(item, depart, arrive))));
            var i=from item in ListToreturn orderby item.time ascending select item;
            return i.ToList();
        }
        private TimeSpan HelptheTrip(Line l,StationLine d,StationLine f)
        {
            TimeSpan timefinal = new TimeSpan(0, 0, 0);
            int index = l.listStations.FindIndex(item => item.shelterNumber == d.shelterNumber);
            int lastindex = l.listStations.FindIndex(station => station.shelterNumber == f.shelterNumber);

            for (int a=index;a<lastindex;a++)
            {
                timefinal = timefinal.Add(TimeSpan.FromHours(l.listStations[a].Temps.Hours));
                timefinal = timefinal.Add(TimeSpan.FromMinutes(l.listStations[a].Temps.Minutes));
                timefinal = timefinal.Add(TimeSpan.FromSeconds(l.listStations[a].Temps.Seconds));

            }
            return timefinal;
        }


        #endregion
    }
}

