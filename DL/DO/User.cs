using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace DAL.DO
{
    [Serializable]
    public class User:ICloneable
    {
        public int ID { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
        public string email { get; set; }

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
                return null;
            }
        }

        public override string ToString()
        {
            return String.Format("pwd{0}", pwd);
        }
    }
}
