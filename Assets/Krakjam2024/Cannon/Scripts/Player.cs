using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class Player : MonoBehaviour
    {
        public string Id;
        [SerializeField] private MeshRenderer _inner;

        public void HandleDataPacket(DataPacket dataPacket)
        {
            Debug.Log(dataPacket.PlayerId);
            Debug.Log(dataPacket.PhoneColor);
            Debug.Log(dataPacket.X);
            Debug.Log(dataPacket.Y);

            SetColor(dataPacket.PhoneColor);
        }

        private void SetColor(string hexColor)
        {
            Color newColor = HexToColor(hexColor);
            _inner.material.color = newColor;
        }

        private static Color HexToColor(string hex)
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