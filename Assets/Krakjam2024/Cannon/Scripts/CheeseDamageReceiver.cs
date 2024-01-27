using Placuszki.Krakjam2024;
using UnityEngine;

public class CheeseDamageReceiver : MonoBehaviour
{
   [SerializeField] private Cat _cat;

   private void OnCollisionEnter(Collision other)
   {
      var cheese = other.gameObject.GetComponent<Cheese>();
      if (cheese)
      {
         var playerId = cheese.GetPlayerId();
         _cat.Hit(playerId);
      }
   }
}
