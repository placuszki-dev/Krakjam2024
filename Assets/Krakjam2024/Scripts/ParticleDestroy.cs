using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
 
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }

}
