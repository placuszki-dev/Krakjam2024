using System;

namespace Placuszki.Krakjam2024.Server
{
    [Serializable]
    public class DataPacket
    {
        public string PlayerId { get; set; }
        public string PhoneColor { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}