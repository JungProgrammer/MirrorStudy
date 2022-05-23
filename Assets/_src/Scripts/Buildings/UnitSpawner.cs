using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] 
    private Health _health;
    
    
    [SerializeField] 
    private Unit _unitPrefab = null;


    [SerializeField] 
    private Transform _unitSpawnPoint;


    [SerializeField] 
    private TMP_Text _remainingUnitsText;


    [SerializeField] 
    private Image _unitProgressImage;


    [SerializeField] 
    private int _maxUnitQueue = 5;


    [SerializeField] 
    private float _spawnMoveRange;


    [SerializeField] 
    private float _unitSpawnDuration = 5f;


    [SyncVar(hook = nameof(ClientHandleQueueUnitsUpdated))]
    private int _queueUnits;

    [SyncVar] 
    private float _unitTimer;


    private float _progressImageVelocity;

    
    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }


    #region Server

    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;
    }


    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
    }


    [Server]
    private void ProduceUnits()
    {
        if (_queueUnits == 0)
            return;


        _unitTimer += Time.deltaTime;
        
        if (_unitTimer < _unitSpawnDuration)
            return;
        
        
        Unit unitInstance = Instantiate(_unitPrefab, _unitSpawnPoint.position, _unitSpawnPoint.rotation);
        
        NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);


        Vector3 spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = _unitSpawnPoint.position.y;
        
        Debug.DrawRay(_unitSpawnPoint.position + spawnOffset, Vector3.up * 5, Color.green);
        Debug.Log(_unitSpawnPoint.position + spawnOffset);

        UnitMovement unitMovement = unitInstance.GetUnitMovement();
        unitMovement.ServerMove(_unitSpawnPoint.position + spawnOffset);


        _queueUnits--;
        _unitTimer = 0;
    }


    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }


    [Command]
    private void CmdSpawnUnit()
    {
        if (_queueUnits == _maxUnitQueue)
            return;


        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < _unitPrefab.GetResourcesCost())
            return;

        _queueUnits++;
        
        player.SetResources(player.GetResources() - _unitPrefab.GetResourcesCost());
    }

    #endregion


    #region Client
    
    private void UpdateTimerDisplay()
    {
        float newProgress = _unitTimer / _unitSpawnDuration;

        if (newProgress < _unitProgressImage.fillAmount)
        {
            _unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            _unitProgressImage.fillAmount = Mathf.SmoothDamp(
                _unitProgressImage.fillAmount,
                newProgress,
                ref _progressImageVelocity,
                0.1f
            );
        }
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!hasAuthority)
            return;
        
        
        CmdSpawnUnit();
    }


    private void ClientHandleQueueUnitsUpdated(int oldUnits, int newUnits)
    {
        _remainingUnitsText.text = newUnits.ToString();
    }

    #endregion
}
