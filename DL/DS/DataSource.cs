using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Spire.Xls;
using System.IO;
using DAL.DO;
namespace DAL.DS
{
    public static class DataSource
    {

        public static List<Bus> Buses = new List<Bus>();
        public static List<Line> Lines = new List<Line>();
        public static List<StationLine> StationLines = new List<StationLine>();
        public static List<Station> Stations = new List<Station>();
        public static List<User> Users = new List<User>();
        public static List<ExitLine> ExitLines = new List<ExitLine>();
        public static List<UserTrip> UserTrips = new List<UserTrip>();
        public static List<BusOnRoad> BuseonRoad = new List<BusOnRoad>();
        public static List<Stationsconnected> StationsConnecteds = new List<Stationsconnected>();
        
        static DataSource()
        {
            InitAllLists();
        
        }
        
        static void InitAllLists()
        {
            int forID = 0;
            #region init Bus
            for (int a = 0; a < 20; a++)
            {
                DataSource.Buses.Add(new Bus("s"));
                DataSource.Buses[a].ID = forID;
                forID ++;
            }
            #endregion

            #region init Line
            for (int a = 0; a < 10; a++)
            {
                
                DataSource.Lines.Add(new Line("s"));
                DataSource.Lines[a].ID = forID;
                forID++;
            }
            #endregion

            #region init  Station
            Workbook newBook = new Workbook();
            Worksheet newSheet = newBook.Worksheets[0];
            Workbook workbook = new Workbook();
            string directory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString();

            workbook.LoadFromFile(directory+@"\StationBus.xlsx");
            int row = 2;
            int columns = 2;
            int columnsName = 3;
            int columnsLongitude = 6;
            int columnsLatitude = 5;
            Worksheet sheet = workbook.Worksheets[0];

            for (int a = 0; a < 40; a++)
            {
                DataSource.Stations.Add(new Station("s"));
           
                CellRange StationCode = sheet.Range[row, columns];
                CellRange Stationname = sheet.Range[row, columnsName];
                CellRange StationLongitude = sheet.Range[row, columnsLongitude];
                CellRange StationLatitude = sheet.Range[row, columnsLatitude];
                DataSource.Stations[a].latitude = double.Parse(StationLatitude.Value);
                DataSource.Stations[a].longitude = double.Parse(StationLongitude.Value);
                DataSource.Stations[a].shelterNumber = int.Parse(StationCode.Value);
                DataSource.Stations[a].ID = forID;
                DataSource.Stations[a].address = Stationname.Value.ToString();
                row++;
                forID++;
            }
           

            #endregion

            #region init StationLine
            int index = 0;
            for (int a = 0; a < 40; a++)
            {
               
                DataSource.StationLines.Add(new StationLine());
                DataSource.StationLines[a].CheckedOrNot = false;
                DataSource.StationLines[a].address = DataSource.Stations[a].address;
                DataSource.StationLines[a].DigitPanel =DataSource.Stations[a].DigitPanel;
                DataSource.StationLines[a].latitude = DataSource.Stations[a].latitude;
                DataSource.StationLines[a].longitude = DataSource.Stations[a].longitude;
                DataSource.StationLines[a].shelterNumber = DataSource.Stations[a].shelterNumber;
                DataSource.StationLines[a].HandicappedAccess =DataSource.Stations[a].HandicappedAccess;
                DataSource.StationLines[a].ID = forID;
                forID++;
            }
            for (int a = 0; a < 40; a++)
            {
                for (int b = 0; b < 4; a++, b++)
                {
                    if (b == 0)
                    {
                        DataSource.Lines[index].firstStation = DataSource.StationLines[b].shelterNumber;
                    }
                    DataSource.StationLines[a].LineHere = DataSource.Lines[index].ID;
                }
                a--;
                DataSource.Lines[index].lastStation = DataSource.StationLines[a].shelterNumber;

                index++;
            }
            #endregion

            #region init StationC
            for (int a = 0; a < 40; a++)
            {
                if (a == 39)
                {
                    DataSource.StationsConnecteds.Add(new Stationsconnected(DataSource.StationLines[a], DataSource.StationLines[0]));
                    DataSource.StationsConnecteds[a].ID = forID;
                    break;
                }

                DataSource.StationsConnecteds.Add(new Stationsconnected(DataSource.StationLines[a], DataSource.StationLines[a + 1]));
                DataSource.StationsConnecteds[a].ID = forID;
                forID++;
            }
            for (int a = 0; a < 40; a++)
            {
                DataSource.StationLines[a].Temps = DataSource.StationsConnecteds[a].timeBetween;
                DataSource.StationLines[a].Distance = DataSource.StationsConnecteds[a].distance;
            }

            #endregion



        }

    }

    
}
