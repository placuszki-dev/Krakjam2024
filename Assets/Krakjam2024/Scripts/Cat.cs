using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Cat : MonoBehaviour
{
    public event Action OnHit;   


    public NavMeshSurface surface;
    NavMeshData data;
    public NavMeshAgent agent;

    public Animator animator;

    float timer;

    Vector3 destination;


    void Start()
    {
        data = surface.navMeshData;
        agent.destination = SetRandomDest(data.sourceBounds);
        timer = 0;
    }



    //Update destination every 5 seconds to test
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 5)
        {
            agent.destination = SetRandomDest(data.sourceBounds);
            timer = 0;
        }

        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);
    }

    Vector3 SetRandomDest(Bounds bounds)
    {
        var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        var z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

        destination = new Vector3(x, 1, z);
        return destination;
    }

}
