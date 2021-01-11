﻿using System;
using System.Collections.Generic;
using BL.BO;
namespace BL
{
    public interface IBl
    {
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
        StationLine findlineForStation(StationLine l);

        bool deleteStationLine();
        bool isStationLinexists(StationLine l);

        #endregion

        #region StationFunction
        IEnumerable<Station> GetStations();
         bool deleteStations();
         IEnumerable<Station> getStationLessOne(int a);
        void modifyStation(Station a);

        Station getStation(int a);
        void addstation(Station a);

        #endregion

        #region StationConnected

        IEnumerable<Stationsconnected> getStationConnected();
        void addOneCouple(StationLine first, StationLine last);

        bool commitDistanceTime(Stationsconnected s);
   

        #endregion

        #region Users

        void addUser(User u);
         bool isExists(User u);
        void deleteUser(User u);

         bool checkpwd(User a);

         void resetpwd(User a);
        #endregion
    }
}