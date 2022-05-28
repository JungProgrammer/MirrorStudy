using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject _landingPagePanel;


    [SerializeField] 
    private TMP_InputField _addressInput;


    [SerializeField] 
    private Button _joinButton;


    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }


    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }


    public void Join()
    {
        string address = _addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        _joinButton.interactable = false;
    }


    private void HandleClientConnected()
    {
        _joinButton.interactable = true;
        
        gameObject.SetActive(false);
        _landingPagePanel.SetActive(false);
    }


    private void HandleClientDisconnected()
    {
        _joinButton.interactable = true;
    }
}
