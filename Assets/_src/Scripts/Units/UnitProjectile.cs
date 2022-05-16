using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] 
    private Rigidbody _rigidbody;


    [SerializeField] 
    private float _destroyAfterSeconds;


    [SerializeField] 
    private float _launchForce;


    private void Start()
    {
        _rigidbody.velocity = transform.forward * _launchForce;
    }


    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), _destroyAfterSeconds);
    }


    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
