using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class DLException:Exception
    {
        public DLException()
        {

        }
        public DLException(string message):base(message)
        {

        }
    }
}
