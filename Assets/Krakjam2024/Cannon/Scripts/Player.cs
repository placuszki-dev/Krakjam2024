using System;
using DG.Tweening;
using Placuszki.Krakjam2024.Server;
using UnityEngine;
using UnityEngine.UI;

namespace Placuszki.Krakjam2024
{
    public class Player : MonoBehaviour
    {
        public int Points { get; set; }
        public UserInfo UserInfo;
        
        [Header("References")]
        [SerializeField] private MeshRenderer _inner;
        [SerializeField] private Transform _innerScaleTransform;
        [SerializeField] private Transform _launcherTransform;
        [SerializeField] private Transform _view;
        [SerializeField] private Image _cheeseIconImage;

        [Header("Prefabs")]
        [SerializeField] private Cheese _cheesePrefab;
        
        [Header("Prefabs")]
        [SerializeField] private Sprite _goudaSprite;
        [SerializeField] private Sprite _cheddarSprite;
        
        [Header("Settings")]
        [SerializeField] private float _cooldown = 0.5f;
        
        [Header("Animations")]
        [SerializeField] private float _shakeStrength = 0.1f;
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private int _shakeVibrato = 50;

        private float _cooldownLeft = 0;

        private void Update()
        {
            if (_cooldownLeft > 0)
            {
                _cooldownLeft -= Time.deltaTime;
                UpdateCooldownGfx();
            }
        }

        private void Start()
        {
            GameManager.Instance.RegisterPlayer(this);
        }

        private void OnDestroy()
        {
            GameManager.Instance.DeregisterPlayer(this);
        }

        public void HandleDataPacket(DataPacket dataPacket)
        {
            if (_cooldownLeft <= 0)
            {
                Shoot(dataPacket.X, dataPacket.Y);
            }
        }

        private void Shoot(float x, float y)
        {
            CheeseType cheeseType = (CheeseType) UserInfo.CheeseType;
            PlayShootAnimation(x, y);
            Cheese cheese = Instantiate(_cheesePrefab, _launcherTransform);
            
            cheese.transform.localPosition = Vector3.zero;
            cheese.transform.localRotation = Quaternion.identity;

            cheese.Launch(this, x, y, cheeseType);
            _cooldownLeft = _cooldown;
        }

        private void PlayShootAnimation(float x, float y)
        {
            _view.DOShakePosition(_shakeDuration, new Vector3(x, 0, y) * _shakeStrength, _shakeVibrato);
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
            return UserInfo.PlayerId;
        }
        
        public Color GetPlayerColor()
        {
            return ColorParser.HexToColor(UserInfo.PhoneColor);
        }

        public void SetupPlayer(UserInfo userInfo)
        {
            UserInfo = userInfo;
            SetColor(userInfo.PhoneColor);
            SetupCheeseIcon(userInfo.CheeseType);
        }

        private void SetupCheeseIcon(int cheeseTypeInt)
        {
            Debug.Log($"cheeseTypeInt: {cheeseTypeInt}");
            CheeseType cheeseType = (CheeseType)cheeseTypeInt;
            switch (cheeseType)
            {
                case CheeseType.Unknown:
                case CheeseType.Gouda:
                    _cheeseIconImage.sprite = _goudaSprite;
                    break;
                case CheeseType.Cheddar:
                    _cheeseIconImage.sprite = _cheddarSprite;
                    break;
            }
        }
    }
}