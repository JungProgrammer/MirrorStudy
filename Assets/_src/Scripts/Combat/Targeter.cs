using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField]
    private Targetable _target;


    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable targetable))
            return;


        _target = targetable;
    }

    [Server]
    public void ClearTarget()
    {
        _target = null;
    }

    #endregion


    #region Client

    

    #endregion
}
