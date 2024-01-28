using Placuszki.Krakjam2024;
using UnityEngine;

public class CheeseDamageReceiver : MonoBehaviour
{
    [SerializeField] private Cat _cat;
    [Header("SFX")]
    [SerializeField] private AudioSource _sfx;

    private void OnCollisionEnter(Collision other)
    {
        var cheese = other.gameObject.GetComponent<Cheese>();
        if (cheese && cheese.IsDeadly)
        {
            cheese.transform.SetParent(transform);

            if (!enabled)
            {
                return;
            }
            
            enabled = false;
            
            var playerId = cheese.GetPlayerId();
            _cat.Hit(playerId);

            if (_sfx.isActiveAndEnabled)
            {
                _sfx.pitch = Random.Range(0.7f, 1.2f);
                _sfx.Play();
            }
        }
    }
}
