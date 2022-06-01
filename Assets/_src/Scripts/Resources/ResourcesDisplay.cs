using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text _resourcesText;


    private RTSPlayer _rtsPlayer;


    private void Start()
    {
        _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        
        ClientHandleResourcesUpdated(_rtsPlayer.GetResources());
                
        _rtsPlayer.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }


    private void OnDestroy()
    {
        _rtsPlayer.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }


    private void ClientHandleResourcesUpdated(int resources)
    {
        _resourcesText.text = $"Resources: {resources}";
    }
}
