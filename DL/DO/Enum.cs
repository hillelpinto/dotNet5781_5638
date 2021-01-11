using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DO
{
    
        public enum Area { Jerusalem, TelAviv, Haifa, Ashdod, Eilat, BeerSheva, Netanya, Hertzilya };
        public enum Status
        {
            Ready, OnRoad, Refueling, InMaintenance, NeedMaintenance, NeedRefuel
        };
    
}
