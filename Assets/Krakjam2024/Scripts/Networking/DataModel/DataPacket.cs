using System;

namespace Placuszki.Krakjam2024.Server
{
    [Serializable]
    public class DataPacket
    {
        public string PlayerId { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }
    }
}