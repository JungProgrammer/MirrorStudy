using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] 
    private Targeter _targeter;


    [SerializeField] 
    private GameObject _pojectilePrefab;


    [SerializeField] 
    private Transform _projectileSpwnPoint;


    [SerializeField] 
    private float _fireRange = 5f;


    [SerializeField] 
    private float _fireRate = 1f;


    [SerializeField] 
    private float _rotatingSpeed = 20f;


    private float _lastFireTime;


    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.GetTarget();
        
        if (target == null)
            return;
        if (!CanFireAtTarget())
            return;


        Quaternion targetRotation =
            Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetRotation, _rotatingSpeed * Time.deltaTime);

        if (Time.time > (1 / _fireRate) + _lastFireTime)
        {
            Quaternion projectileRotation =
                Quaternion.LookRotation(target.GetAimAtPoint().position -
                                        _projectileSpwnPoint.position);
            
            GameObject projectileInstance = Instantiate(_pojectilePrefab, _projectileSpwnPoint.position, projectileRotation);
            
            NetworkServer.Spawn(projectileInstance, connectionToClient);
            
            _lastFireTime = Time.time;
        }
    }


    [Server]
    private bool CanFireAtTarget()
    {
        return (_targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= _fireRange * _fireRange;
    }
}
