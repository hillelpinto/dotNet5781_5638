using System;
using System.Collections.Generic;
using System.Text;

namespace BL.BO
{
    public class User
    {
        public int ID { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
        public string email { get; set; }
        public override string ToString()
        {
            return String.Format("pwd{0}", pwd);
        }
    }
}
