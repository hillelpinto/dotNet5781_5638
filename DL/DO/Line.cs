using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace DAL.DO
{
    [Serializable]
    public class Line:ICloneable
    {
        Random r = new Random();
        public int ID { get; set; }
        public int busLineNumber { get; set; }
        public int firstStation { get; set; }
        public int lastStation { get; set; }
        public Area area { get; set; }
        public bool CheckedOrNot { get; set; }
        public string getArea { get => area.ToString(); set { } }
                   
        public int speed { get; set; }
        public Line() {  }
        public override string ToString()
        {
            return String.Format("{0}, Area :{1}", busLineNumber, getArea);
        }

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

        public Line(string n)
        {
            speed = r.Next(20, 51);
            busLineNumber = r.Next(100, 999);
           
            area = (Area)r.Next(1, 7);
            CheckedOrNot = false;
           
        }
    }
}
