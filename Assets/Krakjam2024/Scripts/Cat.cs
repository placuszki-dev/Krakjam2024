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

    public NavMeshSurface _surface;
    public NavMeshAgent _agent;
    public GameObject _deadParticle;
    [Space]
    public Animator _animator;
    public float _endRunSpeed = 20f;


    private float _timer;
    private NavMeshData _data;
    //cheese cheese cheese!
    bool _cheese;

    Vector3 destination;


    public void Hit(int playerID)
    {
        if (_cheese)
            return;

        var part = Instantiate(_deadParticle);
        part.GetComponent<ParticleSystem>().Play();

        Destroy(this.gameObject);
    }

    void Start()
    {
        _data = _surface.navMeshData;
        _agent.destination = SetRandomDest(_data.sourceBounds);
        _timer = 0;
    }


    void Update()
    {
        _animator.SetFloat("speed", _agent.velocity.magnitude / _agent.speed);

        if (_cheese)
            return;

        _timer += Time.deltaTime;
        if (_timer > 5)
        {
            _agent.destination = SetRandomDest(_data.sourceBounds);
            _timer = 0;
        }
    }

    Vector3 SetRandomDest(Bounds bounds)
    {
        var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        var z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

        destination = new Vector3(x, 1, z);
        return destination;
    }
}
