using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DO
{
   public  class ExitLine
    {
        public int IdBus { get; set; }
        public int Frequence { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }


    }
}
