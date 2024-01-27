using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public static class ColorParser
    {
        public static Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color))
            {
                return color;
            }

            Debug.LogError("Invalid hex color: " + hex);
            return Color.white;
        }
    }
}