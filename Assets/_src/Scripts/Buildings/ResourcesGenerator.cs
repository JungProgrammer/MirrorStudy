using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResourcesGenerator : NetworkBehaviour
{
    [SerializeField] 
    private Health _health;


    [SerializeField] 
    private int _resourcesPerInterval;


    [SerializeField] 
    private float _interval;


    private float _timer;
    private RTSPlayer _rtsPlayer;


    public override void OnStartServer()
    {
        _timer = _interval;
        _rtsPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();

        _health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }


    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }


    [ServerCallback]
    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _rtsPlayer.SetResources(_rtsPlayer.GetResources() + _resourcesPerInterval);

            _timer += _interval;
        }
    }


    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }


    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}
