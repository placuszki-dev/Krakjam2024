using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class Cheese : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Settings")]
        [SerializeField] private float _xMultiplier = 1;
        [SerializeField] private float _yMultiplier = 1;

        public void Launch(float xVector, float yVector)
        {
            xVector *= _xMultiplier;
            yVector *= _yMultiplier;

            Vector3 force = transform.TransformDirection(new Vector3(xVector, 0, yVector));
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}