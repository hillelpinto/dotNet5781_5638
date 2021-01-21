using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BL.BO;
namespace BL
{
    static class Deepcopy
    {
        public static void CopyPropertiesTo<T, S>(this S from, T to)
        {
            foreach (PropertyInfo propTo in to.GetType().GetProperties())
            {
                PropertyInfo propFrom = typeof(S).GetProperty(propTo.Name);
                if (propFrom == null)
                    continue;
                var value = propFrom.GetValue(from, null);
                if (value is ValueType || value is string)
                    propTo.SetValue(to, value);
            }
        }
    
        public static Line convertToBOLine(DAL.DO.Line from)
        {
            Line to = new Line();
            to.area = (Area)from.area;
            to.busLineNumber = from.busLineNumber;
            to.firstStation = from.firstStation;
            to.lastStation = from.lastStation;
            to.BeginService = from.BeginService;
            to.EndService = from.EndService;
            to.CheckedOrNot = from.CheckedOrNot;
            to.speed = from.speed;
            to.ID = from.ID;
            return to;

        }
        public static DAL.DO.Line convertToDOLine(Line from)
        {
            DAL.DO.Line to = new DAL.DO.Line();
            to.area = (DAL.DO.Area)(Area)from.area;
            to.busLineNumber = from.busLineNumber;
            to.firstStation = from.firstStation;
            to.lastStation = from.lastStation;
            to.BeginService = from.BeginService;
            to.EndService = from.EndService;
            to.CheckedOrNot = from.CheckedOrNot;
            to.speed = from.speed;
            to.ID = from.ID;
            return to;

        }
        public  static StationLine convertStationLinetoBO(DAL.DO.StationLine s)
        {
            StationLine l = new StationLine();
            s.CopyPropertiesTo(l);
            return l;
        }
        public static DAL.DO.StationLine forSave(StationLine bo)
        {
            DAL.DO.StationLine Do = new DAL.DO.StationLine();
            Do.ID = bo.ID;
            Do.LineHere = bo.LineHere;
            Do.latitude = bo.latitude;
            Do.longitude = bo.longitude;
            Do.address = bo.address;
            Do.CheckedOrNot = bo.CheckedOrNot;
            Do.DigitPanel = bo.DigitPanel;
            Do.HandicappedAccess = bo.HandicappedAccess;
            Do.nextStation = bo.nextStation;
            Do.positioninmyLine = bo.positioninmyLine;
            Do.shelterNumber = bo.shelterNumber;
            Do.stationName = bo.stationName;
        
            Do.prevStation = bo.prevStation;
            return Do;
        }
        public static DAL.DO.StationLine convertStationLinetoDO(StationLine s)
        {
            DAL.DO.StationLine l = new DAL.DO.StationLine();
            s.CopyPropertiesTo(l);
            return l;
        }
        public static BO.Stationsconnected convertSConnectedTOBO(DAL.DO.Stationsconnected l)
        {
            BL.BO.Stationsconnected s = new Stationsconnected();
            s.ID = l.ID;
            s.timeBetween = l.timeBetween;
            s.distance = l.distance;
      
            s.numeroUno = l.numeroUno;
            s.numeroDeuzio = l.numeroDeuzio;
            return s;
        }
        public static DAL.DO.Stationsconnected convertSConnectedTODO(Stationsconnected l)
        {
            DAL.DO.Stationsconnected s = new DAL.DO.Stationsconnected();
            l.CopyPropertiesTo(s);
            s.ID = l.ID;
            s.numeroUno = l.numeroUno;
            s.numeroDeuzio =l.numeroDeuzio;
            return s;
        }
    }
}
