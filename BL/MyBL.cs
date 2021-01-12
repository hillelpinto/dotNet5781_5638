using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using BL.BO;
using DAL;
using System.Net.Mail;

namespace BL
{

    public class MyBL : IBl
    {
        IDAL dal = DALFactory.GetDL("data");
        #region BusFunction
      
        public BO.Bus convertBusToBO(DAL.DO.Bus a)
        {
            Bus b = new Bus();
            a.CopyPropertiesTo(b);
            return b;
        }

        public void checkStatus()
        {
            dal.checkstatus();

        }
        public void addBus(Bus a)
        {
            try
            {
                DAL.DO.Bus b = new DAL.DO.Bus();
                a.CopyPropertiesTo(b);
                dal.addBus(b);
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

            IEnumerable<Bus> Listfiltered = GetBuses().Where(bus => bus.license.ToString().Substring(0, a.Length) == a);
            return Listfiltered.ToList();

        }
        public List<Bus> GetBuses()
        {
            List<DAL.DO.Bus> myList = dal.getmyBuses();
            List<Bus> MyList = new List<Bus>();
            foreach (DAL.DO.Bus bus in myList)
            {
                MyList.Add(convertBusToBO(bus));
            }
            return MyList;
        }
     
        public bool deleteBuses()
        {
            return dal.deletebuses();

        }
      
        public void modifyBus(Bus b)
        {

            DAL.DO.Bus c = new DAL.DO.Bus();
            b.CopyPropertiesTo(c);
            dal.modifyBus(c);
        }
        #endregion

        #region LineFunction

        public void modifyLine(Line l)
        {
            DAL.DO.Line s = Deepcopy.convertToDOLine(l);
            dal.modifyLine(s);
        }
    

        public List<Line> getLines()//It search all the lines saved and add it all the stationLine which passing through 
        {
            List<Line> t = new List<Line>();
            dal.getLines().ForEach(line => t.Add(Deepcopy.convertToBOLine(line)));
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
                    l.CheckedOrNot = true;
                    modifyLine(l);

                    t.Remove(l);
                }
            }
            
            return t;
        }

            public void setIndexinLine(Line l)//It set al the needed value in a line (distance,time,first station,last station..) 
            {
            
                for (int a = 0; a < l.listStations.Count(); a++)
                {
                
                    l.listStations[a].Temps = getStationConnected().ToList().Find(station => station.numeroUno.ID == l.listStations[a].ID|| station.numeroDeuzio.ID == l.listStations[a].ID).timeBetween;
                    l.listStations[a].Distance = getStationConnected().ToList().Find(station => station.numeroUno.ID == l.listStations[a].ID|| station.numeroDeuzio.ID == l.listStations[a].ID).distance;


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

                dal.addLine(s);
            }
            catch(DLException ex)
            {
                throw new BLException(ex.Message);
            }
            }
        public IEnumerable<Line> getAllAllLine()//get the line without the stations in each of them
        {
            List<Line> t = new List<Line>();
            dal.getAllAllLine().ToList().ForEach(line => t.Add(Deepcopy.convertToBOLine(line)));
            return t;
        }

            public bool deleteLines()//It delete all the stations in the line too
            {
            IEnumerable<Line> lineInDeletion = getAllAllLine().ToList().Where(line => line.CheckedOrNot == true).ToList();
            IEnumerable<StationLine> deleteStationFirst = getmyStationsLines().Where(station => lineInDeletion.ToList().Exists(line => line.ID == station.LineHere));
            deleteStationFirst.ToList().ForEach(station => station.CheckedOrNot = true);
            deleteStationFirst.ToList().ForEach(station => modifyStationline(station));
         
       
                return dal.DeleteLines();
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
        public List<StationLine> getAllStationsLines()
        {
            List<StationLine> toReturn = new List<StationLine>();
            dal.getAllStationsLines().ForEach(station => toReturn.Add(convertSLinetoBO(station)));
            return toReturn;
        }

            public bool deleteStationLine()
            {

                return dal.deleteStationLine();
            }
            public List<StationLine> getmyStationsLines()//It returns all the stationsLine with setting it time and distance acording to stationsConected 
            {
                List<StationLine> list = new List<StationLine>();
            
            dal.getstationslines().ForEach(station => list.Add(convertSLinetoBO(station)));

            foreach (StationLine sl in list)
            {

                foreach (Stationsconnected s in getStationConnected())
                {
                    if (s.numeroUno.ID == sl.ID)
                    {
                        sl.Temps = s.timeBetween;
                        sl.Distance = s.distance;
                        //modifyStationline(sl);
                    }
                }
            }
            return list.ToList();
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
                dal.addStationL(s);
         
            }
            public void modifyStationline(StationLine l)
            {
                DAL.DO.StationLine s = new DAL.DO.StationLine();
                l.CopyPropertiesTo(s);
            int verif = s.LineHere;
                dal.modifystationline(s);
            }
            public bool isStationLinexists(StationLine l)
            {
                return dal.getstationslines().Exists(station => station.shelterNumber == l.shelterNumber);
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
                dal.GetStation().ForEach(station => temp.Add(convertStationtoBo(station)));
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
                return (dal.deleteStations());
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
                dal.addStation(s);
            }
            catch(DLException ex)
            {
                throw new BLException(ex.Message);
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
                dal.modifyStation(s);
       }

        #endregion


        #region StationConnected
       
        public IEnumerable<Stationsconnected> getStationConnected()
        {
            List<Stationsconnected> toreturn = new List<Stationsconnected>();
            dal.getStationConnected().ToList().ForEach(station =>toreturn.Add(Deepcopy.convertSConnectedTOBO(station)));
            IEnumerable<Stationsconnected> end = toreturn;
            return end;

        }
        public bool commitDistanceTime(Stationsconnected s)
        {
            DAL.DO.Stationsconnected l = new DAL.DO.Stationsconnected();
            l = Deepcopy.convertSConnectedTODO(s);
            float f = l.distance;
            return dal.commitTime(l);
        }
      

public void addOneCouple(StationLine s,StationLine l)
        {
            DAL.DO.StationLine temp = new DAL.DO.StationLine();
            DAL.DO.StationLine tempp = new DAL.DO.StationLine();
            s.CopyPropertiesTo(temp);
            l.CopyPropertiesTo(tempp);
            int i = temp.shelterNumber;
            int j= tempp.shelterNumber;
            dal.addOnecouple(temp,tempp);
        }
        #endregion

        #region User
        public void addUser(User u)
            {
                DAL.DO.User b = new DAL.DO.User();
                u.CopyPropertiesTo(b);
                dal.addUser(b);
            }
            public void deleteUser(User u)
            {
                DAL.DO.User b = new DAL.DO.User();
                u.CopyPropertiesTo(b);
                dal.deleteUser(b);
            }
            public bool isExists(User u)
            {
                return dal.getmyUsers().Exists(user => user.username == u.username);
            }
            public bool checkpwd(User a)
            {
                User b = new User();
                b.pwd = dal.getmyUsers().Find(user => user.username == a.username).pwd;
                if (b.pwd == a.pwd)
                    return true;
                else
                    return false;
            }
            public void resetpwd(User j)
            {
                DAL.DO.User a = new DAL.DO.User();
                j.CopyPropertiesTo(a);
                DAL.DO.User b = dal.getmyUsers().Find(user => user.username == a.username);
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
                        dal.deleteUser(a);
                        dal.addUser(b);
                    }
                }
                catch (Exception )
                {
                    b.pwd = "0000";
                    dal.deleteUser(a);
                    dal.addUser(b);
              
                    throw new BLException("Your mail is not valid we can't send you a new password so by default it's 0000 now !");
                }
            }
            #endregion
     }
}

