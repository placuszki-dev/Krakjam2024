using System;

namespace Placuszki.Krakjam2024.Server
{
    [Serializable]
    public class UserInfo
    {
        public string PlayerId { get; set; }
        public string PhoneColor { get; set; }
        public int CheeseType { get; set; }
    }
}