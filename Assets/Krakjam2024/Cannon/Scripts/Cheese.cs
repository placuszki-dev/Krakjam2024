using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Placuszki.Krakjam2024
{
    public class Cheese : MonoBehaviour
    {
        public bool IsDeadly { get; private set; } = true;

        [Header("References")] 
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private MeshRenderer _meshRenderer;

        [Header("Settings - trajectory")]
        [SerializeField] private float _inputVectorYClampMin = 0.1f;
        [SerializeField] private float _inputVectorsClampMax = 1;
        
        [Header("Settings - trajectory")]
        [SerializeField] private float _xMultiplier = 100;
        [SerializeField] private float _yMultiplier = 100;
        [SerializeField] private float _minTorque = -0.1f;
        [SerializeField] private float _maxTorque = 0.1f;
        [SerializeField] private RigidbodySettings _rigidbodySettingsAfterCollision;
        [SerializeField] private float _destroyDelay = 3f;

        [Header("Particle")]
        public GameObject _collisionParticle;
        
        [Header("Materials")]
        public Material _goudaMaterial;
        public Material _cheddarMaterial;

        private Player _player;
        private bool _alreadyPlayedCollisionParticle;

        public void Launch(Player player, float xVector, float yVector, CheeseType cheeseType)
        {
            switch (cheeseType)
            {
                case CheeseType.Unknown:
                    _meshRenderer.material = _goudaMaterial;
                    break;
                case CheeseType.Gouda:
                    _meshRenderer.material = _goudaMaterial;
                    break;
                case CheeseType.Cheddar:
                    _meshRenderer.material = _cheddarMaterial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cheeseType), cheeseType, null);
            }
            _player = player;
            // Force
            xVector = Mathf.Clamp(xVector, -_inputVectorsClampMax, _inputVectorsClampMax);
            yVector = Mathf.Clamp(yVector, _inputVectorYClampMin, _inputVectorsClampMax);
            
            xVector *= _xMultiplier;
            yVector *= _yMultiplier;
            Vector3 force = transform.TransformDirection(new Vector3(xVector, 0,yVector));
            
            Quaternion targetRotation = Quaternion.LookRotation(force, Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);
            
            _rigidbody.AddForce(force, ForceMode.Impulse);

            // Torque
            float torqueX = Random.Range(_minTorque, _maxTorque);
            float torqueY = Random.Range(_minTorque, _maxTorque);
            float torqueZ = Random.Range(_minTorque, _maxTorque);
            Vector3 torque = new Vector3(torqueX, torqueY, torqueZ);
            _rigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision other)
        {
            var cheeseDamageReceiver = other.gameObject.GetComponent<CheeseDamageReceiver>();
            if (!cheeseDamageReceiver)
            {
                IsDeadly = false;
            }
            
            var cheese = other.gameObject.GetComponent<Cheese>();
            if (cheese)
            {
                return; 
            }

            SetRigidbodyValuesAfterCollision(other);
            Destroy(gameObject, _destroyDelay);
        }
        
        private void SetRigidbodyValuesAfterCollision(Collision other)
        {
            if(other.gameObject.GetComponent<Cheese>()) // cheese with cheese -> nothing happens
                return;

            if (!_alreadyPlayedCollisionParticle)
            {
                var part = Instantiate(_collisionParticle);
                part.transform.position = transform.position;
                _alreadyPlayedCollisionParticle = true;
            }

            _rigidbody.angularDrag = _rigidbodySettingsAfterCollision.AngularDrag;
            _rigidbody.drag = _rigidbodySettingsAfterCollision.Drag;
        }

        public string GetPlayerId()
        {
            return _player.GetPlayerId();
        }
    }

    [Serializable]
    public class RigidbodySettings
    {
        public float AngularDrag = 20;
        public float Drag = 20;
    }
}