using System;
using UnityEngine;

namespace WebglExample
{
    [Serializable]
    public class PlayerInformation
    {
        public string Id;
        public string Name;
        public string GameId;
        public Vector3 Position;
        public float LookAt;
    }
}