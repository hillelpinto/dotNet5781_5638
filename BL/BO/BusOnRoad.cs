using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
    public class BusOnRoad
    {
        public int IdBus { get; set; }
        public int busLineNumber { get; set; }
        public string license { get; set; }
        public DateTime ExitEstimated { get; set; }
        public DateTime ExitHours { get; set; }
        public int ShellterNumberoftheLastStation { get; set; }
        public DateTime TimePreviousStation { get; set; }
        public DateTime timetoNextStation { get; set; }

    }
}
