using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class Player : MonoBehaviour
    {
        public string Id;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        private float _baseSpeed = 3;

        public float _xSpeed;
        public int _ySpeed;

        private void Awake()
        {
            SetRandomColor();
        }

        private void Update()
        {
            Move();
        }

        private void SetRandomColor()
        {
            Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
            _spriteRenderer.color = randomColor;
        }

        private void Move()
        {
            Vector2 translation = new Vector2(_xSpeed, _ySpeed) * (Time.deltaTime * _baseSpeed);
            Vector2 currentPosition = transform.position;

            Vector2 newPos = currentPosition + translation;
            float clampedX = Mathf.Clamp(newPos.x, -5.0f, 5.0f);
            float clampedY = Mathf.Clamp(newPos.y, -5.0f, 5.0f);

            transform.position = new Vector2(clampedX, clampedY);
        }

        public void HandleDataPacket(DataPacket dataPacket)
        {
            Debug.Log($"Player {Id.Substring(0, 4)}...: {dataPacket.Key} - {dataPacket.Value}");
            string key = dataPacket.Key;
            int value = dataPacket.Value;

            switch (key)
            {
                case "0":
                    _xSpeed = value == 0 ? 0 : -1;
                    break;
                case "1":
                    _xSpeed = value == 0 ? 0 : 1;
                    break;
                case "2":
                    _ySpeed = value == 0 ? 0 : -1;
                    break;
                case "3":
                    _ySpeed = value == 0 ? 0 : 1;
                    break;
            }
        }

        private void Stop()
        {
            _xSpeed = 0;
            _ySpeed = 0;
        }
    }
}