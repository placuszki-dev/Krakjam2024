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
            cheese.gameObject.SetActive(false);

            if (!enabled)
            {
                return;
            }
            
            enabled = false;
            
            var playerId = cheese.GetPlayerId();
            Color playerColor = cheese.GetPlayerColor();
            _cat.Hit(playerId, cheese.CheeseType, playerColor);

            if (_sfx.isActiveAndEnabled)
            {
                _sfx.pitch = Random.Range(0.7f, 1.2f);
                _sfx.Play();
            }
        }
    }
}
