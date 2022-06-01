using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject _lobbyUI;


    [SerializeField] 
    private Button _startGameButton;


    [SerializeField] 
    private Text[] _playerNameTexts;


    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }


    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }


    private void HandleClientConnected()
    {
        _lobbyUI.SetActive(true);
    }


    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager) NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            _playerNameTexts[i].text = players[i].GetDisplayName();
        }

        for (int i = players.Count; i < _playerNameTexts.Length; i++)
        {
            _playerNameTexts[i].text = "Waiting For Player ...";
        }

        _startGameButton.interactable = players.Count >= 2;
    }


    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        _startGameButton.gameObject.SetActive(state);
    }


    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }


    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
