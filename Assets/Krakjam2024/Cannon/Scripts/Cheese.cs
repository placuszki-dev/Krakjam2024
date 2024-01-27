using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Placuszki.Krakjam2024
{
    public class Cheese : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Rigidbody _rigidbody;

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

        public void Launch(float xVector, float yVector)
        {
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
            SetRigidbodyValuesAfterCollision(other);
            Destroy(gameObject, _destroyDelay);
        }

        private void SetRigidbodyValuesAfterCollision(Collision other)
        {
            if(other.gameObject.GetComponent<Cheese>()) // cheese with cheese -> nothing happens
                return;
            
            _rigidbody.angularDrag = _rigidbodySettingsAfterCollision.AngularDrag;
            _rigidbody.drag = _rigidbodySettingsAfterCollision.Drag;
        }
    }

    [Serializable]
    public class RigidbodySettings
    {
        public float AngularDrag = 20;
        public float Drag = 20;
    }
}