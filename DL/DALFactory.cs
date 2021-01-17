using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DAL
{
    public static class DALFactory
    {
        public static IDAL GetDL(string type)
        {

            switch (type)
            {

                case "data": return MyDAL.Instance;


                case "xml": return DLXML.Instance;

                default: throw new ArgumentException("bad Dal type to use");

            };
        }
      
    }
}
