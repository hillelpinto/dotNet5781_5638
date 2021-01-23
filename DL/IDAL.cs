using System;
using System.Collections.Generic;
using DAL.DO;
namespace DAL
{
    public interface IDAL
    {
        void init();
        #region Bus
        void addBus(Bus a);
        IEnumerable<Bus> getmyBuses();
        void modifyBus(Bus a);
        bool deletebuses();
        void checkstatus();
        #endregion

        #region LineFunction
        IEnumerable<Line> getLines();
        void addLine(Line l);
        void addLineSchedule(ExitLine l);

        IEnumerable<Line> getAllAllLine();
        void modifyLine(Line l);
        bool DeleteLines();


        #endregion

        #region StationFunction
        IEnumerable<Station> GetStation();
        bool deleteStations();
        void modifyStation(Station a);
        void addStation(Station a);
        #endregion

        #region StationLineFunction

        void addStationL(StationLine l);
        List<StationLine> getstationslines();

        List<StationLine> getAllStationsLines();
        void modifystationline(StationLine l);
        bool deleteStationLine();

        #endregion

        #region StationConnected

        IEnumerable<Stationsconnected> getStationConnected();
       
        void addOnecouple(StationLine l, StationLine s);

        bool commitTime(Stationsconnected s);

        #endregion

        #region ScheduleLine
        IEnumerable<ExitLine> getmySchedules();

        void modifySchedule(ExitLine l);
        

        #endregion

        #region User
        void addUser(User u);

        void deleteUser(User u);
        IEnumerable<User> getmyUsers();
        #endregion
    }
}
