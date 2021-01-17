using DAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DAL
{
    public class DLXML:IDAL
    {
        private static IDAL instance = null;
        public static IDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new DLXML();

                return instance;
            }
        }
        #region Location
        string BusPath=@"BusXml.xml";
        string UserPath = @"UserXml.xml";



        #endregion
        public void addBus(Bus a)
        {
            
        }

        public void addLine(Line l)
        {
            throw new NotImplementedException();
        }

        public void addOnecouple(StationLine l, StationLine s)
        {
            throw new NotImplementedException();
        }

        public void addStation(Station a)
        {
            throw new NotImplementedException();
        }

        public void addStationL(StationLine l)
        {
            throw new NotImplementedException();
        }

        public void addUser(User u)
        {
            throw new NotImplementedException();
        }

        public void checkstatus()
        {
            throw new NotImplementedException();
        }

        public bool commitTime(Stationsconnected s)
        {
            throw new NotImplementedException();
        }

        public bool deletebuses()
        {
            throw new NotImplementedException();
        }

        public bool DeleteLines()
        {
            throw new NotImplementedException();
        }

        public bool deleteStationLine()
        {
            throw new NotImplementedException();
        }

        public bool deleteStations()
        {
            throw new NotImplementedException();
        }

        public void deleteUser(User u)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Line> getAllAllLine()
        {
            throw new NotImplementedException();
        }

        public List<StationLine> getAllStationsLines()
        {
            throw new NotImplementedException();
        }

        public List<Line> getLines()
        {
            throw new NotImplementedException();
        }

        public List<Bus> getmyBuses()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExitLine> getmySchedules()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> getmyUsers()
        {
           
            XElement userRoot = XMLTools.LoadListFromXMLElement(UserPath);
            return (from p in userRoot.Elements()
                    select new User()
                    {
                        ID = Int32.Parse(p.Element("ID").Value),
                        username=p.Element("username").Value,
                        pwd=p.Element("pwd").Value,
                        email=p.Element("email").Value,
               
                    }
                   );
        }


        public List<Station> GetStation()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Stationsconnected> getStationConnected()
        {
            throw new NotImplementedException();
        }

        public List<StationLine> getstationslines()
        {
            throw new NotImplementedException();
        }

        public void modifyBus(Bus a)
        {
            throw new NotImplementedException();
        }

        public void modifyLine(Line l)
        {
            throw new NotImplementedException();
        }

        public void modifySchedule(ExitLine l)
        {
            throw new NotImplementedException();
        }

        public void modifyStation(Station a)
        {
            throw new NotImplementedException();
        }

        public void modifystationline(StationLine l)
        {
            throw new NotImplementedException();
        }
    }

}
