using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class Player : MonoBehaviour
    {
        public string Id;
        
        [Header("References")]
        [SerializeField] private MeshRenderer _inner;
        [SerializeField] private Transform _launcherTransform;

        [Header("Prefabs")]
        [SerializeField] private Cheese _cheesePrefab;

        private void OnDestroy()
        {
            GameManager.Instance.DeregisterPlayer(Id);
        }

        public void HandleDataPacket(DataPacket dataPacket)
        {
            Debug.Log(dataPacket.PlayerId);
            Debug.Log(dataPacket.PhoneColor);
            Debug.Log(dataPacket.X);
            Debug.Log(dataPacket.Y);

            SetColor(dataPacket.PhoneColor);
            GameManager.Instance.RegisterPlayer(dataPacket);
            Shoot(dataPacket.X, dataPacket.Y);
        }

        private void Shoot(float x, float y)
        {
            Cheese cheese = Instantiate(_cheesePrefab, _launcherTransform);
            
            cheese.transform.localPosition = Vector3.zero;
            cheese.transform.localRotation = Quaternion.identity;
            
            cheese.Launch(this, x, y);
        }

        private void SetColor(string hexColor)
        {
            Color newColor = ColorParser.HexToColor(hexColor);
            _inner.material.color = newColor;
        }

        public string GetPlayerId()
        {
            return Id;
        }
    }
}