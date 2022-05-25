using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] 
    private Renderer[] _colorRenderers;


    [SyncVar(hook = nameof(HandleTeamColorUpdated))] 
    private Color _teamColor = new Color();


    #region Server

    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        _teamColor = player.GetTeamColor();
    }

    #endregion


    #region Client

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in _colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    #endregion
}
