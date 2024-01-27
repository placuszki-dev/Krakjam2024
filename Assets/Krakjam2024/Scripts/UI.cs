using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{

    public Animation _uiAnim;
   
    IEnumerator Start()
    {
        _uiAnim.Play("IdleBack");
        yield return new WaitForSeconds(1);

        _uiAnim.Play("ser");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
