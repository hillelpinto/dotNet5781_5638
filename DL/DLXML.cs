using DAL.DO;
using DAL.DS;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DAL
{
    public class DLXML : IDAL
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
        public int forID = 0;
        #region Location


        #region DS XML Files

        string busPath =@"BusXml.xml"; //XElement        
        string linePath = @"LineXml.xml"; //XMLSerializer
        string stationPath = @"StationXml.xml"; //XMLSerializer
        string StationLinePath = @"lineStationXml.xml"; //XMLSerializer
        string ExitLinePath = @"lineTripXml.xml"; //XMLSerializer
        string StationConnected = @"AdjacentStationsXml.xml"; //XMLSerializer
        string userPath = @"User.xml"; //XMLSerializer


        #endregion

        #endregion

        

        public void init()
        {
            string temp=busPath;
            List<Bus> ListBus = new List<Bus>();//Init of 20 Bus
            for (int a = 0; a < 20; a++)
            {
                ListBus.Add(new Bus("s"));
                ListBus[a].ID = forID;
                forID++;
            }
            XMLTools.SaveListToXMLSerializer(ListBus, busPath);
            List<User> ListUser = new List<User>();
            for (int a = 0; a < 1; a++)
            {

                ListUser.Add(new User());
                ListUser[a].username = ListUser[a].pwd = ListUser[a].email = "admin";
                ListUser[a].ID = forID;
                forID++;
            }

            XMLTools.SaveListToXMLSerializer(ListUser, userPath);//Init of admin account
            List<Line> ListLine = new List<Line>();
            for (int a = 0; a < 10; a++)
            {

                ListLine.Add(new Line("s"));
                ListLine[a].ID = forID;
                forID++;
            }

            XMLTools.SaveListToXMLSerializer(ListLine, linePath);//Init of Line

            List<Station> myList = new List<Station>();
            Workbook newBook = new Workbook();
            Worksheet newSheet = newBook.Worksheets[0];
            Workbook workbook = new Workbook();
            string directory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString();

            workbook.LoadFromFile(directory + @"\StationBus.xlsx");
            int row = 2;
            int columns = 2;
            int columnsName = 3;
            int columnsLongitude = 6;
            int columnsLatitude = 5;
            Worksheet sheet = workbook.Worksheets[0];

            for (int a = 0; a < 40; a++)
            {
                myList.Add(new Station("s"));

                CellRange StationCode = sheet.Range[row, columns];
                CellRange Stationname = sheet.Range[row, columnsName];
                CellRange StationLongitude = sheet.Range[row, columnsLongitude];
                CellRange StationLatitude = sheet.Range[row, columnsLatitude];
                myList[a].latitude = StationLatitude.Value.ToString();
                myList[a].longitude = StationLongitude.Value.ToString();
                myList[a].shelterNumber = int.Parse(StationCode.Value);
                myList[a].ID = forID;
                myList[a].address = Stationname.Value.ToString();
                row++;
                forID++;
            }
            XMLTools.SaveListToXMLSerializer(myList, stationPath);//Init station


            XElement root = XMLTools.LoadListFromXMLElement(ExitLinePath);
            for(int a=0;a<10;a++)
            {
                ExitLine s = new ExitLine(ListLine[a].ID);
                s.ID = forID;
                XElement schedules = new XElement("LineTrip", new XElement("ID", s.ID),
                             new XElement("BusLineNumber", s.IdBus),
                             new XElement("FrequenciesOfExit", s.FrequenceinMN),
                             new XElement("BeginService", s.Start.ToString()),
                             new XElement("EndService", s.End.ToString()));
                root.Add(schedules);
                forID++;
            }
            XMLTools.SaveListToXMLElement(root,ExitLinePath);

            List<StationLine> stationLine = new List<StationLine>();

            int index = 0;
            for (int a = 0; a < 40; a++)
            {

                stationLine.Add(new StationLine());
                stationLine[a].CheckedOrNot = false;
                stationLine[a].address = myList[a].address;
                stationLine[a].DigitPanel = myList[a].DigitPanel;
                stationLine[a].latitude = myList[a].latitude;
                stationLine[a].longitude = myList[a].longitude;
                stationLine[a].shelterNumber = myList[a].shelterNumber;
                stationLine[a].HandicappedAccess = myList[a].HandicappedAccess;
                stationLine[a].ID = forID;
                forID++;
            }
            for (int a = 0; a < 40; a++)
            {
                for (int b = 0; b < 4; a++, b++)
                {
                    if (b == 0)
                    {
                        ListLine[index].firstStation = stationLine[b].shelterNumber;
                    }
                    stationLine[a].LineHere = ListLine[index].ID;
                }
                a--;
                ListLine[index].lastStation = stationLine[a].shelterNumber;

                index++;
            }

            XElement root2 = XMLTools.LoadListFromXMLElement(StationConnected);
            for (int a = 0; a < 40; a++)
            {
                if (a == 39)
                {
                    Stationsconnected tempy = new Stationsconnected(stationLine[a], stationLine[0]);
                    tempy.ID = forID;
                    XElement personElem = new XElement("AdjacentStation", new XElement("ID", tempy.ID),
                                  new XElement("FirstStationsID", tempy.numeroUno),
                                  new XElement("SecondStationsID", tempy.numeroDeuzio),
                                  new XElement("Distance", tempy.distance),
                                  new XElement("TimeBetween", tempy.timeBetween.ToString()));
                    root2.Add(personElem);
                    break;
                }
                Stationsconnected tempp = new Stationsconnected(stationLine[a], stationLine[a + 1]);
                tempp.ID = forID;
                XElement personElem2 = new XElement("AdjacentStation", new XElement("ID", tempp.ID),
                                new XElement("FirstStationsID", tempp.numeroUno),
                                new XElement("SecondStationsID", tempp.numeroDeuzio),
                                new XElement("Distance", tempp.distance),
                                new XElement("TimeBetween", tempp.timeBetween.ToString()));
                root2.Add(personElem2);
                forID++;
            }

            XMLTools.SaveListToXMLElement(root2, StationConnected);
            XMLTools.SaveListToXMLSerializer(stationLine, StationLinePath);



        }


        #region Bus
        public void addBus(Bus a)
        {
            List<Bus> ListBus = XMLTools.LoadListFromXMLSerializer<Bus>(busPath);
            if (ListBus.Exists(bus => bus.license == a.license))
                throw new DLException("This bus already exists !");
            else
            {
                Random r = new Random();
                a.ID = r.Next(0, 10000);
                while (getmyBuses().ToList().Exists(b => b.ID == a.ID) == true)
                {
                    a.ID = r.Next(0, 10000);
                }

                ListBus.Add(a); //no need to Clone()

                XMLTools.SaveListToXMLSerializer(ListBus, busPath);
            }
        }
        public IEnumerable<Bus> getmyBuses()
        {
           
            List<Bus> ListToReturn = XMLTools.LoadListFromXMLSerializer<Bus>(busPath);

            return from line in ListToReturn
                   where line.CheckedOrNot==false
                   select line;


        }
        public void checkstatus()
        {
            List<Bus> ListBus = XMLTools.LoadListFromXMLSerializer<Bus>(busPath);

            for (int a = 0; a < ListBus.Count(); a++)
            {
                if ((DateTime.Compare(ListBus[a].Checkup.Date, DateTime.Now.AddYears(-1))) < 0 || ListBus[a].KmAfterLastMaintenance >= 20000)
                {
                    ListBus[a].returnStatus = "NeedMaintenance";
                    ListBus[a].Percent = 0;
                }

                else if (ListBus[a].Fuel < 8)
                {
                    ListBus[a].returnStatus = "NeedRefuel";
                    ListBus[a].Percent = 8;
                }
            }
            XMLTools.SaveListToXMLSerializer(ListBus, busPath);

        }
        public void modifyBus(Bus a)
        {
            List<Bus> ListBus = XMLTools.LoadListFromXMLSerializer<Bus>(busPath);
            int index = ListBus.FindIndex(item => item.license == a.license);
            ListBus[index] = a;
            XMLTools.SaveListToXMLSerializer(ListBus, busPath);

        }
        public bool deletebuses()
        {
            List<Bus> ListBus = XMLTools.LoadListFromXMLSerializer<Bus>(busPath);

            bool inside = ListBus.Exists(bus => bus.CheckedOrNot == true);
            return inside;
        }

        #endregion


        #region Line
        public void addLine(Line person)
        {
            List<Line> checkMany = XMLTools.LoadListFromXMLSerializer<Line>(linePath).Where(line => line.busLineNumber == person.busLineNumber).ToList();

            if (checkMany.Count == 2)
                throw new DLException("This line already exists !");

            else if (checkMany.Count == 0 || checkMany.Count == 1 && person.firstStation == checkMany[0].lastStation && person.lastStation == checkMany[0].firstStation)
            {
                List<Line> myList = XMLTools.LoadListFromXMLSerializer<Line>(linePath);
                myList.Add(person);
                XMLTools.SaveListToXMLSerializer<Line>(myList, linePath);
               

            }
            else
            {
                throw new DLException("You can add this line only according to the two-way laws !");
            }
       

        }
        public bool DeleteLines()
        {
            List<Line> ListLine = XMLTools.LoadListFromXMLSerializer<Line>(linePath);

            bool inside = ListLine.Exists(bus => bus.CheckedOrNot == true);
            return inside;
        }


        public IEnumerable<Line> getAllAllLine()
        {
           
            List<Line> ListLineR = XMLTools.LoadListFromXMLSerializer<Line>(linePath);

            return from line in ListLineR
                   select line;
        }

        public IEnumerable<Line> getLines()
        {
            List<Line> ListLineR = XMLTools.LoadListFromXMLSerializer<Line>(linePath);

            return from line in ListLineR
                   where line.CheckedOrNot == false
                   select line;
        }

        public void modifyLine(Line l)
        {
            List<Line> ListLineR = XMLTools.LoadListFromXMLSerializer<Line>(linePath);
            int index = ListLineR.FindIndex(line => line.ID == l.ID);
            ListLineR[index] = l;
            XMLTools.SaveListToXMLSerializer<Line>(ListLineR,linePath);

        }
        #endregion


        #region Station

        public void addStation(Station a)
        {
            List<Station> myStation = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            if (myStation.Exists(station => station.shelterNumber == a.shelterNumber))
                throw new DLException("This stations already exists !");
            else
            {
                Random r = new Random();
                a.ID = r.Next(0, 10000);
                while (GetStation().ToList().Exists(station => station.ID == a.ID) == true)
                {
                    a.ID = r.Next(0, 10000);
                }
                myStation.Add(a);
                XMLTools.SaveListToXMLSerializer<Station>(myStation, stationPath);

            }
        }
       
        public bool deleteStations()
        {
            List<Station> myStation = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            List<StationLine> myStationLine = XMLTools.LoadListFromXMLSerializer<StationLine>(StationLinePath);
            bool inside = false;
            int i = myStation.Where(item => item.CheckedOrNot == false).ToList().Count;
            if (myStation.Count() != i)
                inside = true;
            foreach (StationLine s in myStationLine)
            {
                if (myStation.Where(station => station.CheckedOrNot == false).ToList().Exists(station => station.shelterNumber == s.shelterNumber) == false)
                {
                    s.CheckedOrNot = true;
                    modifystationline(s);
                }
            }
            return inside;

        }
        public IEnumerable<Station> GetStation()
        {
          
            List<Station> ListStation = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);

            return from line in ListStation
                   select line;

        }

        public void modifyStation(Station s)
        {
            List<Station> myStation = XMLTools.LoadListFromXMLSerializer<Station>(stationPath);
            int index = myStation.FindIndex(station => station.shelterNumber == s.shelterNumber);

            myStation[index] = s;
            XMLTools.SaveListToXMLSerializer<Station>(myStation,stationPath);
        }
        #endregion

        #region StationLine

        public List<StationLine> getAllStationsLines()
        {
            List<StationLine> mylist = XMLTools.LoadListFromXMLSerializer<StationLine>(StationLinePath);
            return mylist;
        }
        public void addStationL(StationLine l)
        {
            List<StationLine> sl = XMLTools.LoadListFromXMLSerializer<StationLine>(StationLinePath);
            Random r = new Random();
            l.ID = r.Next(0, 1000);
            while (getAllStationsLines().Exists(station => station.ID == l.ID))
            {
                l.ID = r.Next(0, 1000);
            }
            sl.Add(l);
            XMLTools.SaveListToXMLSerializer<StationLine>(sl, StationLinePath);

          }
        public bool deleteStationLine()
        {
            bool inside = false;
            IEnumerable<StationLine> myb = XMLTools.LoadListFromXMLSerializer<StationLine>(StationLinePath);
            int count = myb.Where(item => item.CheckedOrNot == false).ToList().Count;
            if (myb.Count() != count)
                inside = true;
            return inside;
        }
        public void modifystationline(StationLine l)
        {
            List<StationLine> sl = XMLTools.LoadListFromXMLSerializer<StationLine>(StationLinePath);
            int index = sl.FindIndex(stationline => stationline.ID == l.ID);
            sl[index] = l;
            XMLTools.SaveListToXMLSerializer<StationLine>(sl, StationLinePath);
        }
        public List<StationLine> getstationslines()
        {
            List<StationLine> mylist = XMLTools.LoadListFromXMLSerializer<StationLine>(StationLinePath).Where(station => station.CheckedOrNot == false).ToList();
            return mylist;
        }



        #endregion


        #region User

        public void deleteUser(User u)
        {
            int indexxxx = u.ID;
            List<User> ListToReturn = XMLTools.LoadListFromXMLSerializer<User>(userPath);
            ListToReturn.RemoveAll(user => user.username == u.username);
           
            XMLTools.SaveListToXMLSerializer(ListToReturn, userPath);

        }
        public void addUser(User person)
        {
            List<User> ListStudents = XMLTools.LoadListFromXMLSerializer<User>(userPath);
            Random r = new Random();
            person.ID = r.Next(0, 1000);
            while (getmyUsers().ToList().Exists(user => user.ID == person.ID))
                person.ID = r.Next(0, 1000);

            using (MailMessage mymessage = new MailMessage())
            {
                mymessage.From = new MailAddress("hillelpinto5@gmail.com", "From MoovitProject");
                try
                {
                    mymessage.To.Add(person.email);
                    mymessage.Subject = "You registration has been made successfully";
                    mymessage.Body = string.Format("Welcome to the team {0}, you are invited to access our data now. Your badge will be sent to you within 3-4 days.Good luck for the future !", person.username);
                    mymessage.IsBodyHtml = true;
                    using (SmtpClient smpt = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smpt.Credentials = new System.Net.NetworkCredential("hillelpinto5@gmail.com", "Hhillel2652");
                        smpt.EnableSsl = true;
                        smpt.Send(mymessage);
                        ListStudents.Add(person); //no need to Clone()

                        XMLTools.SaveListToXMLSerializer(ListStudents, userPath);
                    }
                }
                catch
                {
                    ListStudents.Add(person); //no need to Clone()

                    XMLTools.SaveListToXMLSerializer(ListStudents, userPath);
                }
             

            }

           

        }
        public IEnumerable<User> getmyUsers()
        {
            List<User> ListToReturn = XMLTools.LoadListFromXMLSerializer<User>(userPath);

            return from user in ListToReturn
                   select user;
        }

        #endregion


        #region Schedules
        public IEnumerable<ExitLine> getmySchedules()
        {
            XElement root = XMLTools.LoadListFromXMLElement(ExitLinePath);
            List<ExitLine> myList = new List<ExitLine>();
            foreach (XElement p in root.Elements())
            {
                ExitLine sc = new ExitLine();
                sc.ID = Int32.Parse(p.Element("ID").Value);
                sc.IdBus = Int32.Parse(p.Element("BusLineNumber").Value);
                sc.FrequenceinMN = Int32.Parse(p.Element("FrequenciesOfExit").Value);

                sc.Start = TimeSpan.Parse(p.Element("BeginService").Value);
                sc.End = TimeSpan.Parse(p.Element("EndService").Value);

                myList.Add(sc);
            }
            return myList;
        }
        public void addLineSchedule(ExitLine temp)
        {
            XElement root = XMLTools.LoadListFromXMLElement(ExitLinePath);
            Random r = new Random();
            temp.ID = r.Next(0, 1000);
            XElement schedules = new XElement("LineTrip", new XElement("ID", temp.ID),
                                 new XElement("BusLineNumber", temp.IdBus),
                                 new XElement("FrequenciesOfExit", temp.FrequenceinMN),
                                 new XElement("BeginService", temp.Start.ToString()),
                                 new XElement("EndService", temp.End.ToString()));
            root.Add(schedules);
            XMLTools.SaveListToXMLElement(root, ExitLinePath);

        }

        public void modifySchedule(ExitLine s)
        {
         
            XElement root = XMLTools.LoadListFromXMLElement(ExitLinePath);
            XElement toCommit = (from item in root.Elements() where item.Element("ID").Value == s.ID.ToString() select item).FirstOrDefault();
            if (toCommit != null)
            {
                toCommit.Element("BeginService").Value = s.Start.ToString();
                toCommit.Element("EndService").Value = s.End.ToString();
            }
            XMLTools.SaveListToXMLElement(root,ExitLinePath);

        }

        #endregion


        #region StationConnected



        public void addOnecouple(StationLine l, StationLine s)
        {
            XElement root = XMLTools.LoadListFromXMLElement(StationConnected);
            Stationsconnected temp = new Stationsconnected(l, s);
            Random r = new Random();
            temp.ID = r.Next(0, 1000);
            while (getStationConnected().ToList().Exists(station => station.ID == temp.ID))
            {
                temp.ID = r.Next(0, 1000);
            }
            XElement personElem = new XElement("AdjacentStation", new XElement("ID", temp.ID),
                                 new XElement("FirstStationsID", temp.numeroUno),
                                 new XElement("SecondStationsID", temp.numeroDeuzio),
                                 new XElement("Distance", temp.distance),
                                 new XElement("TimeBetween", temp.timeBetween.ToString()));          
            root.Add(personElem);
            XMLTools.SaveListToXMLElement(root, StationConnected);

        }
     


        public IEnumerable<Stationsconnected> getStationConnected()
        {
            XElement root = XMLTools.LoadListFromXMLElement(StationConnected);
            List<Stationsconnected> myList = new List<Stationsconnected>();
            foreach (XElement p in root.Elements())
            {
                Stationsconnected sc = new Stationsconnected();
                sc.ID = Int32.Parse(p.Element("ID").Value);
                sc.numeroUno = Int32.Parse(p.Element("FirstStationsID").Value);
                sc.numeroDeuzio = Int32.Parse(p.Element("SecondStationsID").Value);
                sc.distance = float.Parse(p.Element("Distance").Value);

                sc.timeBetween = TimeSpan.Parse(p.Element("TimeBetween").Value);
                myList.Add(sc);
            }
            return myList;

        }
        public bool commitTime(Stationsconnected s)
        {
            bool commit = false;
            XElement root = XMLTools.LoadListFromXMLElement(StationConnected);
            XElement toCommit = (from item in root.Elements() where item.Element("ID").Value == s.ID.ToString() select item).FirstOrDefault();
            if(toCommit!=null && (toCommit.Element("TimeBetween").Value != s.timeBetween.ToString() || toCommit.Element("Distance").Value != s.distance.ToString()))
            {
                toCommit.Element("TimeBetween").Value = s.timeBetween.ToString();
                toCommit.Element("Distance").Value = s.distance.ToString();
                commit = true;
            }
            XMLTools.SaveListToXMLElement(root, StationConnected);
       
            return commit;
        }

        #endregion











    }

}
