using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] 
    private NavMeshAgent _meshAgent;


    [ServerCallback]
    private void Update()
    {
        if (!_meshAgent.hasPath)
            return;
        if (_meshAgent.remainingDistance > _meshAgent.stoppingDistance)
            return;
        
        
        _meshAgent.ResetPath();
    }


    #region Server

    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return;


        _meshAgent.SetDestination(position);
    }

    #endregion
}
