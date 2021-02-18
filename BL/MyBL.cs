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
        /// <summary>
        ///For adapt the do bus to bo bus which we present it in the PL
        /// </summary>
        public BO.Bus convertBusToBO(DAL.DO.Bus a)
        {
            Bus b = new Bus();
            a.CopyPropertiesTo(b);
            return b;
        }
        /// <summary>
        ///Set the status about the need of mainteance /refuel etc...
        /// </summary>
        public void checkStatus()
        {
            dalData.checkstatus();

        }
        /// <summary>
        ///If the bus didnt exist ,it add him
        /// </summary>
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
        /// <summary>
        ///LINQ Sort
        /// </summary>
        public List<Bus> getBusesFilteringbyFuel()//Ordering by LINQ
        {
            List<Bus> ListFiltered = GetBuses();
            var toreturn = from bus in ListFiltered orderby bus.Fuel descending select bus;
            return toreturn.ToList();
        }
        /// <summary>
        ///Same
        /// </summary>
        public List<Bus> getBusesFilteringByKmC()
        {
            var Listfiltered = from bus in GetBuses() orderby bus.KmAfterLastMaintenance descending select bus;

            return Listfiltered.ToList();
        }
        /// <summary>
        ///Same
        /// </summary>
        public List<Bus> getBusesFilteringbyKm()
        {
            IEnumerable<Bus> Listfiltered = from bus in GetBuses() orderby bus.Km descending select bus;
            return Listfiltered.ToList();
        }
        /// <summary>
        ///Catch a string and print all the license starting with this value
        /// </summary>
        public List<Bus> getBusesFilteringBylicense(string a)
        {
            bool check = GetBuses().Exists(item => item.license.ToString().Substring(0, a.Length) == a);
            if (!check)
                throw new BLException("This license number doesn't exists ,try again !");
            IEnumerable<Bus> Listfiltered = GetBuses().Where(bus => bus.license.ToString().Substring(0, a.Length) == a);
            return Listfiltered.ToList();

        }
        /// <summary>
        ///Get the data
        /// </summary>
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
        /// <summary>
        ///Set the boolean value to disable
        /// </summary>
        public bool deleteBuses()
        {
            return dalData.deletebuses();

        }
        /// <summary>
        ///If we change the driver's name or the number of the avilable seat ,the commit is made here(call for commit because its dl more precisely..)
        /// </summary>

        public void modifyBus(Bus b)
        {

            DAL.DO.Bus c = new DAL.DO.Bus();
            b.CopyPropertiesTo(c);
            dalData.modifyBus(c);
        }
        #endregion

        #region LineFunction
        /// <summary>
        ///It set all the value of the line in parameter to the line already existing in the system
        /// </summary>
        public void modifyLine(Line l)
        {
            DAL.DO.Line s = Deepcopy.convertToDOLine(l);
            dalData.modifyLine(s);
        }

        /// <summary>
        ///Get the line but with all the data corresponding (Hours of service , station's list ...)
        /// </summary>
        public List<Line> getLines()//It search all the lines saved and add it all the stationLine which passing through 
        {
            List<Line> t = new List<Line>();
            dalData.getLines().ToList().ForEach(line => t.Add(Deepcopy.convertToBOLine(line)));//here we just have the line
            foreach(Line l in t.ToList())
            {
                foreach(StationLine s in getmyStationsLines())
                {
                    if (s.LineHere == l.ID)//It means that this stations is passed by the line l
                    {
                        if (s.positioninmyLine!=null)//We check if there's an index to pay attentio for
                        {
                            l.listStations.Insert(int.Parse(s.positioninmyLine), s);
                        }
                        else
                        {
                            l.listStations.Add(s);
                        }
                    }
                }
                if (l.listStations.Count() >= 2)//If the line contains - than 2 stations it's not "a line"
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
                getMyHours(l);//Get the time of start/end/
                l.listStations[l.listStations.Count - 1].Distance = 0;

                l.listStations[l.listStations.Count - 1].Temps = new TimeSpan(0,0,0);//because there's no next stop at the terminus

            }


            return t;
        }
        /// <summary>
        ///Set the index of the station's line in the line 
        /// </summary>

        public void setIndexinLine(Line l)//It set all the needed value in a line (distance,time,first station,last station..) 
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
        /// <summary>
        ///If the stations dint exist we add it 
        /// </summary>
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
        /// <summary>
        ///It takes a line and set it the hours of service ccording to its data in ExitLine
        /// </summary>
        public void getMyHours(Line l)
        {

            int index = dalData.getmySchedules().ToList().FindIndex(item => item.IdBus == l.ID);
            if (index != -1)
            {
                l.BeginService = getmySchedules().ToList()[index].Start;
                l.EndService = getmySchedules().ToList()[index].End;
               
            }
        }
        /// <summary>
        ///If we want to add a line we add also an objet with his ID and the hours of service
        /// </summary>
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

        /// <summary>
        ///Delete a line and all the stations inside him
        /// </summary>
        public bool deleteLines()//It delete all the stations in the line too
            {
            IEnumerable<Line> lineInDeletion = getAllAllLine().ToList().Where(line => line.CheckedOrNot == true).ToList();
            IEnumerable<StationLine> deleteStationFirst = getmyStationsLines().Where(station => lineInDeletion.ToList().Exists(line => line.ID == station.LineHere));//To catch the stations in the line in order to update them
            deleteStationFirst.ToList().ForEach(station => station.CheckedOrNot = true);
            deleteStationFirst.ToList().ForEach(station => modifyStationline(station));
         
       
                return dalData.DeleteLines();
            }

        #endregion

        #region StationLineFunction
        /// <summary>
        ///Adapt the DO line to BO line that will be represented in PL
        /// </summary>
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
        /// <summary>
        ///It returns the stations line without the line passing through them
        /// </summary>
        public List<StationLine> getAllStationsLines()
        {
            List<StationLine> toReturn = new List<StationLine>();
            dalData.getAllStationsLines().ForEach(station => toReturn.Add(convertSLinetoBO(station)));
            return toReturn;
        }
        /// <summary>
        ///
        /// </summary>
        public bool deleteStationLine()
            {

                return dalData.deleteStationLine();
            }
        /// <summary>
        ///This function returns the stations line with in each of them a list of lines passing through
        /// </summary>
        public List<StationLine> getmyStationsLines()//It returns all the stationsLine with setting it time and distance acording to stationsConected 
            {
                List<StationLine> list = new List<StationLine>();
            
            dalData.getstationslines().ForEach(station => list.Add(convertSLinetoBO(station)));
            list.ForEach(station => getmyTime(station));//Linq request to get the time and distance from the data in StatiosConnected

           
            return list.ToList();
            }
        /// <summary>
        ///Take a station and set its time and distance o the next station according to the StationsConnected
        /// </summary>
        public void getmyTime(StationLine i)
        {
            int index = getStationConnected().ToList().FindIndex(station => station.numeroUno == i.ID);
            if (index != -1)
            {
                i.Distance = getStationConnected().ToList()[index].distance;

                i.Temps = getStationConnected().ToList()[index].timeBetween;
            }
        }
        /// <summary>
        ///Take a station and set it his list of line passing through
        /// </summary>
        public StationLine findlineForStation(StationLine i)//This function will give all the line passing through a specific station
        {
            List<StationLine> ss = getmyStationsLines().Where(station => station.shelterNumber == i.shelterNumber).ToList();
            StationLine s = i;
            foreach(StationLine l in ss)
            {
                s.myLines.Add(getLines().Find(line => line.ID == l.LineHere));
            }
            
            return s;
        }

        /// <summary>
        ///Clone a caracteristique physic'station bus to the stationLine corresponding(Same ID code)
        /// </summary>
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
        /// <summary>
        ///
        /// </summary>
        public void addStationl(StationLine l)
            {
           
                DAL.DO.StationLine s = new DAL.DO.StationLine();
                l.CopyPropertiesTo(s);
                dalData.addStationL(s);
         
            }
        /// <summary>
        ///We set the new value of address and longitude and latitude 
        /// </summary>
        public void modifyStationline(StationLine l)//In the system if the station 321 is passyng by the line 1 and 2 then we'll have 2 stationline so when we want to delete the station 321 we have to delete all of them
            {
            List<StationLine> tonotDelete = new List<StationLine>();
            tonotDelete = getAllStationsLines().Where(station => station.shelterNumber == l.shelterNumber).ToList();
            tonotDelete.ForEach(station => station.CheckedOrNot = l.CheckedOrNot);
            tonotDelete.ForEach(station => station.address = l.address);
            tonotDelete.ForEach(station => station.longitude = l.longitude);
            tonotDelete.ForEach(station => station.latitude = l.latitude);


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
        /// <summary>
        ///It gets all the stations marked as enable 
        /// </summary>
        public IEnumerable<Station> GetStations()
            {
            List<Station> temp = new List<Station>(); 
                dalData.GetStation().ToList().ForEach(station => temp.Add(convertStationtoBo(station)));
            IEnumerable<Station> myList = temp.Where(station => station.CheckedOrNot == false);
                return myList;
            }
        /// <summary>
        ///It gets all the station except the one with a specific station's code
        /// </summary>
        /// 
        public IEnumerable<Station> getStationLessOne(int a)
            {
                IEnumerable<Station> mystation = GetStations().Where(station => station.shelterNumber != a);
                return mystation;

            }


        /// <sum+
       
        ///
        /// </summary>
        public bool deleteStations()
            {
                return (dalData.deleteStations());
            }
        /// <summary>
        ///This function returns a station at a specific index
        /// </summary>
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

        /// <summary>
        ///If the stations already exists we throw a error if not,we add it
        /// </summary>
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
        /// <summary>
        ///In case that the address is incorrect
        /// </summary>
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
        /// <summary>
        ///Get a reponses in XML format and parse it to get the value about the addresse(longitude and latitude)
        /// </summary>
        void setLontLat(DAL.DO.Station s)//Request google and parse the xml response to check if the address is real and in israel ,if its not =>we add to the station random values in latitude and longitude
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

        /// <summary>
        ///When we modify the address so the lat lon value are also updated
        /// </summary>
        public void modifyStation(Station a)
       {
          
           
            DAL.DO.Station s = new DAL.DO.Station();
            a.CopyPropertiesTo(s);
            setAddress(s);
            if (getmyStationsLines().Exists(station => station.shelterNumber == s.shelterNumber))
            {
                StationLine station = getmyStationsLines().Find(sl => sl.shelterNumber == s.shelterNumber);
                station.address = s.address;
                station.longitude = s.longitude;
                station.latitude = s.latitude;
                modifyStationline(station);
            }
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
        /// <summary>
        ///This function (will be launched in a thread) set a time all the time
        /// </summary>
        public void StartSimulator(TimeSpan startTime, int Rate, Action<TimeSpan> updateTime)
        {
            SimulatorClock.Instance.Cancel = false;

            SimulatorClock simulatorClock = SimulatorClock.Instance;
            simulatorClock.Rate = Rate;
            simulatorClock.stopWatch.Restart();
            simulatorClock.ClockObserver += updateTime;
            while (simulatorClock.Cancel != true)//Until we didnt click on stop simlation the time in our simulatorClock still working on adding the timeElapsed
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
        /// <summary>
        ///It set the cancel field in the instance of the simulator,at true
        /// </summary>
        public void StopSimulator()
        {
            SimulatorClock.Instance.Cancel = true;
            SimulatorClock.Instance.Time = new TimeSpan(0, 0, -1);
        }






        #endregion

        #region StationConnected
        /// <summary>
        ///Get the data
        /// </summary>
        public IEnumerable<Stationsconnected> getStationConnected()
        {
            List<Stationsconnected> toreturn = new List<Stationsconnected>();
            dalData.getStationConnected().ToList().ForEach(station =>toreturn.Add(Deepcopy.convertSConnectedTOBO(station)));
            IEnumerable<Stationsconnected> end = toreturn;
            return end;

        }
        /// <summary>
        ///When we update the time between 2 stations ,we commit it to the stations connected object corresponding to those stations
        /// </summary>
        public bool commitDistanceTime(Stationsconnected s)//When we want to commit distance/time between 2 stationline we add this commit to the pbject which contains those value 
        {
            DAL.DO.Stationsconnected l = new DAL.DO.Stationsconnected();
            l.numeroUno = s.numeroUno;
            l.numeroDeuzio = s.numeroDeuzio;
            l.ID = s.ID;
            l.timeBetween = s.timeBetween;
            l.distance = s.distance;
          
           
            return dalData.commitTime(l);
        }

        /// <summary>
        ///When we add a station ,we also add to it a time between him and its adjacent station
        /// </summary>
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
        /// <summary>
        ///Get the data
        /// </summary>
        public IEnumerable<User> getmyUser()
        {
            List<User> toR = new List<User>();
            dalData.getmyUsers().ToList().ForEach(user => toR.Add(convertUserToBO(user)));

            return from user in dalData.getmyUsers()
                   select new User()
                   {
                       pwd = user.pwd,
                       email=user.email,
                       username=user.username            
                   };
           
        }
        /// <summary>
        ///When we modify the pwd the user is deleted and created with a new password 
        /// </summary>
        public void deleteUser(User u)
            {
                DAL.DO.User b = new DAL.DO.User();
                u.CopyPropertiesTo(b);
                dalData.deleteUser(b);
            }
        /// <summary>
        ///In the login we have to check if the user already exists
        /// </summary>

        public bool isExists(User u)
            {
          
            
                return dalData.getmyUsers().ToList().Exists(user => user.username == u.username);
            }
        /// <summary>
        ///Check if the username and pwd are linked
        /// </summary>
        public bool checkpwd(User a)
            {
                User b = new User();
                b.pwd = dalData.getmyUsers().ToList().Find(user => user.username == a.username).pwd;
                if (b.pwd == a.pwd)
                    return true;
                else
                    return false;
            }

        /// <summary>
        ///If the mail is correct we send to the user a new password ,if not we update his pwd by default to 0000
        /// </summary>
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
        /// <summary>
        ///Get the hours of service of all the line
        /// </summary>
        public IEnumerable<ExitLine> getmySchedules()
        {
            IEnumerable<DAL.DO.ExitLine> mylist = dalData.getmySchedules();
            List<ExitLine> toreturn = new List<ExitLine>();
            mylist.ToList().ForEach(item => toreturn.Add(convertSchedule(item)));
            return toreturn;
        }
        /// <summary>
        ///This function will update the hours of service before with the new one
        /// </summary>
        public void modifySchedule(ExitLine s)
        {
            DAL.DO.ExitLine l = new DAL.DO.ExitLine();
            s.CopyPropertiesTo(l);
            dalData.modifySchedule(l);
        }


        #endregion



        #region TripClient
        /// <summary>
        ///This function will get all the line with a specifif station inside , get the time elapsed between them and return a list of the line ordered by the time they take to make the journey 
        /// </summary>
        public List<Trip> getmyTrips(StationLine depart,StationLine arrive)
        {
            List<Trip> myList = new List<Trip>();
            IEnumerable<Line> myLines = getLines().Where(line => line.listStations.Exists(station => station.shelterNumber == depart.shelterNumber));//We get a list with all the line which contains the deeparture station
            List<Line> myRealLine = myLines.ToList();
            foreach(Line l in myRealLine.ToList())
            {
                int firstIndex =l.listStations.FindIndex(station => station.shelterNumber == depart.shelterNumber);
                int lastindex = l.listStations.FindIndex(station => station.shelterNumber == arrive.shelterNumber);
                if(lastindex!=-1)
                {
                    if (lastindex < firstIndex)//It means that the arrival station is before the departure ...so no way,we remove it
                        myRealLine.Remove(l);
                }
            }
            List<Trip> ListToreturn = new List<Trip>();
            myRealLine.ForEach(item => ListToreturn.Add(new Trip(item, HelptheTrip(item, depart, arrive))));//Here we get a list of line with time they made to make the journey
            var i=from item in ListToreturn orderby item.time ascending select item;
            return i.ToList();
        }

        /// <summary>
        ///It make the calcul of the timespan elapsed between the stations of departure to the station of arrival
        /// </summary>
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

