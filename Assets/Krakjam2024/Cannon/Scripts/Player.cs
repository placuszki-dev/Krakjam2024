using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class Player : MonoBehaviour
    {
        public string Id;
        
        [Header("References")]
        [SerializeField] private MeshRenderer _inner;
        [SerializeField] private Transform _innerScaleTransform;
        [SerializeField] private Transform _launcherTransform;

        [Header("Prefabs")]
        [SerializeField] private Cheese _cheesePrefab;
        
        [Header("Settings")]
        [SerializeField] private float _cooldown = 0.5f;

        private float _cooldownLeft = 0;

        private void Update()
        {
            if (_cooldownLeft > 0)
            {
                _cooldownLeft -= Time.deltaTime;
                UpdateCooldownGfx();
            }
        }

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
            if (_cooldownLeft <= 0)
            {
                Shoot(dataPacket.X, dataPacket.Y);
            }
        }

        private void Shoot(float x, float y)
        {
            Cheese cheese = Instantiate(_cheesePrefab, _launcherTransform);
            
            cheese.transform.localPosition = Vector3.zero;
            cheese.transform.localRotation = Quaternion.identity;
            
            cheese.Launch(this, x, y);
            _cooldownLeft = _cooldown;
        }

        private void UpdateCooldownGfx()
        {
            float cooldownPercentage = 1 - _cooldownLeft / _cooldown;
            var scale = _innerScaleTransform.localScale;
            scale.z = cooldownPercentage > 0.1f ? cooldownPercentage : 0.1f;
            _innerScaleTransform.localScale = scale;
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