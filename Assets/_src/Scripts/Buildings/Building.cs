using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] 
    private GameObject _buildingPreview;
    
    
    [SerializeField] 
    private Sprite _icon;


    [SerializeField] 
    private int _id = -1;


    [SerializeField] 
    private int _price = 100;
    
    
    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;
    
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;


    public GameObject GetBuildingPreview()
    {
        return _buildingPreview;
    }


    public Sprite GetIcon()
    {
        return _icon;
    }


    public int GetId()
    {
        return _id;
    }


    public int GetPrice()
    {
        return _price;
    }


    #region Server

    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }


    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    #endregion


    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);   
    }


    public override void OnStopClient()
    {
        if (!hasAuthority)
            return;
        
        
        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    #endregion
}
