using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class BLFactory
    {
        private static IBl instance = null;

        static BLFactory() { }

        public static IBl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyBL();
                }
                return instance;
            }
        }
    }
}
