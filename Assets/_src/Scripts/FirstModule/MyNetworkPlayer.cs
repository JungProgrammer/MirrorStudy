using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _displayNameText;


    [SerializeField] private Renderer _displayColorRenderer;


    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField]
    private string _displayName;


    [SyncVar(hook = nameof(HandleDisplayColorUpdated))] [SerializeField]
    private Color _displayColor;


    #region Server

    [Server]
    public void SetDisplayName(string displayName)
    {
        _displayName = displayName;
    }


    [Server]
    public void SetDisplayColor(Color displayColor)
    {
        _displayColor = displayColor;
    }


    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        SetDisplayName(newDisplayName);
    }

    #endregion


    #region Client

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        _displayNameText.text = newName;
    }


    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        _displayColorRenderer.material.SetColor("_BaseColor", newColor);
    }

    
    [ContextMenu("Set My Name")]
    public void SetMyName()
    {
        CmdSetDisplayName("Some new name");
    }
    
    #endregion
}
