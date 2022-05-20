using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] 
    private Building[] _buildings;


    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int _resources = 500;


    public event Action<int> ClientOnResourcesUpdated;
    
    
    [SerializeField]
    private List<Unit> _playerUnits = new List<Unit>();


    private List<Building> _myBuildings = new List<Building>();


    public List<Unit> GetUnits()
        => _playerUnits;


    public List<Building> GetMyBuildings()
        => _myBuildings;


    public int GetResources()
        => _resources;


    [Server]
    public void SetResources(int newResources)
    {
        _resources = newResources;
    }


    #region Server
    
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }
    
    
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }


    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in _buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }
        
        if (buildingToPlace == null)
            return;


        Building buildingInstance = Instantiate(buildingToPlace, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance.gameObject, connectionToClient);
    }
    
    
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;
        
        
        _playerUnits.Add(unit);
    }
    
    
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
            return;
        
        
        _playerUnits.Remove(unit);
    }
    
    
    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;
        
        
        _myBuildings.Add(building);
    }
    
    
    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;
        
        
        _myBuildings.Remove(building);
    }
    
    #endregion


    #region Client
    
    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
            return;
        
        
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleOnBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleOnBuildingDespawned;
    }
    
    
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
            return;
        
        
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleOnBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleOnBuildingDespawned;
    }


    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }
    
    
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        _playerUnits.Add(unit);
    }
    
    
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        _playerUnits.Remove(unit);
    }
    
    
    private void AuthorityHandleOnBuildingSpawned(Building building)
    {
        _myBuildings.Add(building);
    }
    
    
    private void AuthorityHandleOnBuildingDespawned(Building building)
    {
        _myBuildings.Remove(building);
    }
    
    #endregion
}
