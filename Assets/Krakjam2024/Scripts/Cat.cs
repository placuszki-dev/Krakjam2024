using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class Cat : MonoBehaviour
{
    public NavMeshSurface _surface;
    public NavMeshAgent _agent;
    public GameObject _deadParticle;
    public GameObject _catBubble;
    public Image _catFaceImage;
    public Sprite[] _catSprites;
    [Space]
    public Animator _animator;
    public float _deadDelay = 2f;


    private float _timer;
    private NavMeshData _data;
    //cheese cheese cheese!
    bool _alreadyHit;
    bool _activated;

    Vector3 destination;


    [ContextMenu("hit")]
    void TestHit()
    {
        Hit("1");
    }

    public void Hit(string playerID)
    {
        if (_alreadyHit)
            return;
        
        _alreadyHit = true;

        GameManager.Instance.CatHit(playerID);

        _agent.speed = 0;
        _catBubble.gameObject.SetActive(true);
        _animator.SetFloat("speed", 0);
        _animator.SetTrigger("stop");
        _catFaceImage.sprite = _catSprites[UnityEngine.Random.Range(0, _catSprites.Length)];

        StartCoroutine(DeadDelay());
    }

    [ContextMenu("Init")]
    public void Init()
    {
        _data = _surface.navMeshData;
        _agent.destination = SetRandomDest(_data.sourceBounds);
        _timer = 0;
        _activated = true;
    }


    void Update()
    {
        if (_alreadyHit || !_activated)
            return;

        _animator.SetFloat("speed", _agent.velocity.magnitude / _agent.speed);

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

    IEnumerator DeadDelay()
    {
        yield return new WaitForSeconds(_deadDelay);

        var part = Instantiate(_deadParticle);
        part.transform.position = transform.position;

        DestroyCat();
    }

    public void DestroyCat()
    {
        Destroy(this.gameObject);
    }
}
