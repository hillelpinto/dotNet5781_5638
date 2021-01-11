using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class BLException : Exception
    {
        public BLException()
        {
            }
        public BLException(string message):base(message)
            {

        }
    }
}
