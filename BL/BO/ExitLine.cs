﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
   public  class ExitLine
    {
        public int ID { get; set; }
        public int IdBus { get; set; }
        public int FrequenceinMN { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public ExitLine(int a)
        {
            IdBus= a;
        }
        public ExitLine()
        {
            Start = new TimeSpan(6, 0, 0);
            End = new TimeSpan(22, 0, 0);
            FrequenceinMN = 10;
        }

    }
}
