using System.Collections.Generic;
using UnityEngine;

namespace Krakjam2024.Scripts
{
    public static class Extensions
    {
        public static List<Transform> GetAllChildren(this Transform t)
        {
            List<Transform> res = new();
            foreach (Transform child in t)
            {
                res.Add(child);
            }

            return res;
        }
    }
}