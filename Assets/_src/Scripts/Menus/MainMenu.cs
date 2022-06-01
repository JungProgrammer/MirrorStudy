using System;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject _landingPagePanel;


    [SerializeField] 
    private bool _isUseSteam = false;


    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> _lobbyEnter_t;


    private void Start()
    {
        if (!_isUseSteam)
            return;


        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        _lobbyEnter_t = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }


    public void HostLobby()
    {
        _landingPagePanel.SetActive(false);

        if (_isUseSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }
        
        NetworkManager.singleton.StartHost();
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            _landingPagePanel.SetActive(true);
            return;   
        }


        NetworkManager.singleton.StartHost();
        
        Debug.Log($"SteamIDLobby: {callback.m_ulSteamIDLobby}");
        
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString());
        
        
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");
        
        Debug.Log($"hostAddress: {hostAddress}");
    }
    
    
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    
    
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
            return;


        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");
        
        Debug.Log($"hostAddress: {hostAddress}");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
        
        _landingPagePanel.SetActive(false);
    }
}
