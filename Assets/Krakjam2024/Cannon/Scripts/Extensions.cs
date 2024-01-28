using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Placuszki.Krakjam2024.Scripts
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

        public static List<T> GetRandomElement<T>(this IEnumerable<T> l)
        {
            List<T> elements = l.ToList();

            Random random = new Random();
            List<T> randomElements = new List<T>();

            while (randomElements.Count < l.Count())
            {
                int index = random.Next(elements.Count);
                randomElements.Add(elements[index]);
            }

            return randomElements;
        }
    }
}