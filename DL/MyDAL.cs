using System;
using System.Collections.Generic;
using System.Text;
using DAL.DS;
using DAL.DO;
using System.Linq;
using System.Net.Mail;

namespace DAL
{
    public class MyDAL : IDAL
    {

        private static IDAL instance = null;
        public static IDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new MyDAL();

                return instance;
            }
        }
        #region BusFuction
       
        public List<Bus> getmyBuses()//We have to catch only the bus which are available 
        {
            IEnumerable<Bus> mylist = DataSource.Buses.Where(bus => bus.CheckedOrNot == false);
            return mylist.ToList();
        }
        public void modifyBus(Bus a)//It goes to the place where the bus is and change it with the new one 
        {
            var commit = DataSource.Buses.FindIndex(bus => bus.license == a.license);
            DataSource.Buses[commit] = a;
        }
      
        public void addBus(Bus a)//Check if the bus already exists ,if not it add the bus 
        {
            if (DataSource.Buses.Exists(bus => bus.license == a.license))
                throw new DLException("This bus already exists !");
            else
            {
                DataSource.Buses.Add(a);
            }
        }
        public void checkstatus()
        {
            for (int a = 0; a < DataSource.Buses.Count(); a++)
            {
                if ((DateTime.Compare(DataSource.Buses[a].Checkup.Date, DateTime.Now.AddYears(-1))) < 0 || DataSource.Buses[a].KmAfterLastMaintenance >= 20000)
                {
                    DataSource.Buses[a].returnStatus = "NeedMaintenance";
                    DataSource.Buses[a].Percent = 0;
                }

                else if (DataSource.Buses[a].Fuel < 8) 
                {
                    DataSource.Buses[a].returnStatus = "NeedRefuel";
                    DataSource.Buses[a].Percent = 8;
                }
            }
        }
        public bool deletebuses()//It checks if we have already bus to delete 
        {
            bool inside = DataSource.Buses.Exists(bus => bus.CheckedOrNot == true);
            return inside;
        }
        #endregion


        #region LineFunction
       
        public List<Line> getLines()
        {
            List<Line> lines = DataSource.Lines.Where(line=>line.CheckedOrNot==false).ToList();
            return lines;
        }
        public IEnumerable<Line> getAllAllLine()
        {
            IEnumerable <Line> lines= DataSource.Lines;
            return lines;
        }
     
        public void addLine(Line l)
        {
            List<Line> checkMany = DataSource.Lines.Where(line => line.busLineNumber == l.busLineNumber).ToList();
            if (checkMany.Count==2)
                throw new DLException("This line already exists !");
            else if (checkMany.Count == 1 && l.firstStation == checkMany[0].lastStation && l.lastStation == checkMany[0].firstStation)
            {
                DataSource.Lines.Add(l);
            }
            else
            {
                throw new DLException("You can add this line only according to the two-way laws !");
            }

        }
     
        public bool DeleteLines()
        {
            bool inside = false;
            IEnumerable<Line> before = DataSource.Lines;
            IEnumerable<Line> after = before.Where(item => item.CheckedOrNot == false).ToList();
            if (before.Count() != after.Count())
                inside = true;
            return inside;
        }
        public void modifyLine(Line l)
        {
            var commit = DataSource.Lines.FindIndex(line => line.busLineNumber == l.busLineNumber);
            DataSource.Lines[commit] = l;
        }


        #endregion

        #region StationFunction
        public bool deleteStations()
        {
            bool inside = false;
            IEnumerable<Station> myStation = DataSource.Stations;
            int i = DataSource.Stations.Where(item => item.CheckedOrNot == false).ToList().Count;
            if (myStation.Count() != i)
                inside = true;
            foreach (StationLine s in DataSource.StationLines.ToList())
            {
                if (DataSource.Stations.Where(station=>station.CheckedOrNot==false).ToList().Exists(station => station.shelterNumber == s.shelterNumber) == false)
                {
                    s.CheckedOrNot = true;
                    modifystationline(s);
                }
            }
            return inside;

        }
        public void addStation(Station a)
        {
            if (DataSource.Stations.Exists(station => station.shelterNumber == a.shelterNumber))
                throw new DLException("This stations already exists !");
            DataSource.Stations.Add(a);
        }
      

        public List<Station> GetStation()
        {

            List<Station> t = DataSource.Stations;
            return t;
        }
        public void modifyStation(Station s)
        {
            int index = DataSource.Stations.FindIndex(station => station.shelterNumber == s.shelterNumber);

            DataSource.Stations[index] = s;
        }
        #endregion

        #region StationLineFunction

        public List<StationLine> getstationslines()//It returns only the Station avilable 
        {
            
            List<StationLine> mylist = DataSource.StationLines.Where(station=>station.CheckedOrNot==false).ToList();
            return mylist;
        }
        public List<StationLine> getAllStationsLines()
        {
            return DataSource.StationLines;
        }
        public void modifystationline(StationLine l)//Same the commit in Line/Bus 
        {
            int index = DataSource.StationLines.FindIndex(stationline => stationline.ID == l.ID);
            DataSource.StationLines[index] = l;
        }
      
        public bool deleteStationLine()//It checks if we have the s me number before and after-> if yes ? -> no deletion made so it returns false
        {
            bool inside = false;
            IEnumerable<StationLine> myb = DataSource.StationLines;
           int count = myb.Where(item => item.CheckedOrNot == false).ToList().Count;
            if (myb.Count() != count)
                inside = true;
            return inside;
        }
        public void addStationL(StationLine l)
        {
           
            DataSource.StationLines.Add(l);
        }




        #endregion

        #region StationConnected

        public IEnumerable<Stationsconnected> getStationConnected()
        {

            IEnumerable<Stationsconnected> toReturn = DataSource.StationsConnecteds;
            return toReturn;
        }
    
       public void addOnecouple(StationLine l, StationLine s)//The ctor will give a random time and distance to the journey from station"l" to station"s"
        {
            DataSource.StationsConnecteds.Add(new Stationsconnected(l, s));
            DataSource.StationsConnecteds.Add(new Stationsconnected(l, s));
 
        }

        public bool commitTime(Stationsconnected s)//if in the index we have the same value so no commit made ->return false
        {
            bool commit = false;
            int index = DataSource.StationsConnecteds.FindIndex(station => station.numeroUno.shelterNumber == s.numeroUno.shelterNumber);
            if (DataSource.StationsConnecteds[index].timeBetween != s.timeBetween || DataSource.StationsConnecteds[index].distance != s.distance)
            {
                DataSource.StationsConnecteds[index] = s;
                commit = true;
            }
            return commit;
        }
        #endregion

        #region User
        public void addUser(User a)
        {
            ;
            using (MailMessage mymessage = new MailMessage())
            {
                mymessage.From = new MailAddress("hillelpinto5@gmail.com", "From MoovitProject");
                try
                {
                    mymessage.To.Add(a.email);
                    mymessage.Subject = "You registration has been made successfully";
                    mymessage.Body = string.Format("Welcome to the team {0}, you are invited to access our data now. Your badge will be sent to you within 3-4 days.Good luck for the future !", a.username);
                    mymessage.IsBodyHtml = true;
                    using (SmtpClient smpt = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smpt.Credentials = new System.Net.NetworkCredential("hillelpinto5@gmail.com", "Hhillel2652");
                        smpt.EnableSsl = true;
                        smpt.Send(mymessage);
                        DataSource.Users.Add(a);
                    }
                }
                catch (Exception )//There's a fake mail ...
                {
                    DataSource.Users.Add(a);
                }

            }


        }
        public void deleteUser(User u)
        {
            DataSource.Users.RemoveAll(user => user.username == u.username);
        }
        public List<User> getmyUsers()
        {
            List<User> u = DataSource.Users;
            return u;
        }






        #endregion
    }
}
