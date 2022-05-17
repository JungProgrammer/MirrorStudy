using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] 
    private Health _health;
    
    
    [SerializeField] 
    private UnitMovement _unitMovement;


    [SerializeField] 
    private Targeter _targeter;
    
    
    [SerializeField] 
    private UnityEvent _onSelected = null;
    
    
    [SerializeField] 
    private UnityEvent _onDeselected = null;


    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;
    

    public UnitMovement GetUnitMovement()
        => _unitMovement;


    public Targeter GetTargeter()
        => _targeter;


    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);

        _health.ServerOnDie += ServerHandleDie;
    }


    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        
        _health.ServerOnDie -= ServerHandleDie;
    }


    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
    

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);   
    }


    public override void OnStopClient()
    {
        if (!hasAuthority)
            return;
        
        
        AuthorityOnUnitDespawned?.Invoke(this);
    }


    [Client]
    public void Select()
    {
        if (!hasAuthority)
            return;
        
        
        _onSelected?.Invoke();
    }


    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
            return;
        
        
        _onDeselected?.Invoke();
    }

    #endregion
}
