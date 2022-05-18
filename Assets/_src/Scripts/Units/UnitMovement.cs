using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] 
    private NavMeshAgent _meshAgent;


    [SerializeField] 
    private Targeter _targeter;


    [SerializeField] 
    private float _chaseRange = 10f;


    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }


    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }


    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.GetTarget();
        
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > _chaseRange * _chaseRange)
            {
                _meshAgent.SetDestination(target.transform.position);
            }
            else if (_meshAgent.hasPath)
            {
                _meshAgent.ResetPath();
            }
        }
        
        
        if (!_meshAgent.hasPath)
            return;
        if (_meshAgent.remainingDistance > _meshAgent.stoppingDistance)
            return;
        
        
        _meshAgent.ResetPath();
    }
    

    [Command]
    public void CmdMove(Vector3 position)
    {
        _targeter.ClearTarget();
        
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return;


        _meshAgent.SetDestination(position);
    }


    [Server]
    private void ServerHandleGameOver()
    {
        _meshAgent.ResetPath();
    }

    #endregion
}
