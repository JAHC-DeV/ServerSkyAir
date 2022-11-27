using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace AirModels
{
    public abstract class ObjectModel : IDarkRiftSerializable
    {
        public float DestroyIn { get; set; }
        public float P_X { get; set; }
        public float P_Y { get; set; }
        public float P_Z {get; set; }
        public abstract void Deserialize(DeserializeEvent e);

        public abstract void Serialize(SerializeEvent e);
    }
}
