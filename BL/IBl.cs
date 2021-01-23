using System;
using System.Collections.Generic;
using BL.BO;
namespace BL
{
    public interface IBl
    {
        void init();
       
        #region BusFunction
      
        void addBus(Bus a);
        List<Bus> GetBuses();
        void modifyBus(Bus b);
        void checkStatus();
        bool deleteBuses();
        List<Bus> getBusesFilteringbyFuel();
        List<Bus> getBusesFilteringbyKm();
        List<Bus> getBusesFilteringBylicense(string a);
        List<Bus> getBusesFilteringByKmC();
        #endregion

        #region LineFunction
        List<Line> getLines();

        void addLineSchedule(ExitLine l);
        void setIndexinLine(Line l);
        IEnumerable<Line> getAllAllLine();

        void addLine(Line l);
        bool deleteLines();
        void modifyLine(Line l);

        #endregion

        #region StationLineFunction

        List<StationLine> getmyStationsLines();
        void addStationl(StationLine l);
        StationLine fromStation(Station l);
        List<StationLine> getAllStationsLines();

       void modifyStationline(StationLine l);
        void modifyOnlyOneStation(StationLine l);
        StationLine findlineForStation(StationLine l);

        bool deleteStationLine();
        bool isStationLinexists(StationLine l);

        #endregion

        #region Simulation
        void StartSimulator(TimeSpan startTime, int Rate, Action<TimeSpan> updateTime);
        void StopSimulator();

        TimeSpan getHours();

        #endregion

        #region StationFunction
        IEnumerable<Station> GetStations();
         bool deleteStations();
         IEnumerable<Station> getStationLessOne(int a);
        void modifyStation(Station a);

        void getmyTime(StationLine i);


        Station getStation(int a);
        void addstation(Station a);

        #endregion

        #region Schedule

        IEnumerable<ExitLine> getmySchedules();

        void modifySchedule(ExitLine s);

        #endregion

        #region StationConnected

        IEnumerable<Stationsconnected> getStationConnected();
        void addOneCouple(StationLine first, StationLine last);

        bool commitDistanceTime(Stationsconnected s);
   

        #endregion

        #region Users

        void addUser(User u);
         bool isExists(User u);
        IEnumerable<User> getmyUser();
        void deleteUser(User u);

         bool checkpwd(User a);

         void resetpwd(User a);
        #endregion
    }
}
