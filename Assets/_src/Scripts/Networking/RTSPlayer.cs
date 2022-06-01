using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] 
    private Transform _cameraTransform;
    
    
    [SerializeField] 
    private LayerMask _buildingBlockLayer;
    
    
    [SerializeField] 
    private Building[] _buildings;


    [SerializeField] 
    private float _buildingRangeLimit;
    

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int _resources = 500;

    
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool _isPartyOwner;


    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] 
    private string _displayName;


    public event Action<int> ClientOnResourcesUpdated;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;


    private Color _teamColor;
    
    [SerializeField]
    private List<Unit> _playerUnits = new List<Unit>();


    private List<Building> _myBuildings = new List<Building>();


    public string GetDisplayName()
        => _displayName;

    public bool GetIsPartyOwner()
        => _isPartyOwner;
    
    
    public Transform GetCameraTransform()
        => _cameraTransform;


    public Color GetTeamColor()
        => _teamColor;


    public List<Unit> GetUnits()
        => _playerUnits;


    public List<Building> GetMyBuildings()
        => _myBuildings;


    public int GetResources()
        => _resources;


    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            _buildingBlockLayer))
        {
            return false;
        }

        bool inRange = false;

        foreach (Building building in _myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= _buildingRangeLimit * _buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }


    #region Server
    
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        
        DontDestroyOnLoad(gameObject);
    }
    
    
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }


    [Server]
    public void SetDisplayName(string displayName)
    {
        _displayName = displayName;
    }


    [Server]
    public void SetPartyOwner(bool state)
    {
        _isPartyOwner = state;
    }
    
    
    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        _teamColor = newTeamColor;
    }


    [Server]
    public void SetResources(int newResources)
    {
        _resources = newResources;
    }


    [Command]
    public void CmdStartGame()
    {
        if (!_isPartyOwner)
            return;
        
        
        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
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
        
        if (_resources < buildingToPlace.GetPrice())
            return;


        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        
        
        if (!CanPlaceBuilding(buildingCollider, point))
            return;


        Building buildingInstance = Instantiate(buildingToPlace, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance.gameObject, connectionToClient);
        
        SetResources(_resources - buildingToPlace.GetPrice());
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


    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;
        
        DontDestroyOnLoad(gameObject);
        
        ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
    }


    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        
        if (!isClientOnly)
            return;
        
        ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);
        
        if (!hasAuthority)
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


    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }


    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
            return;


        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
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
