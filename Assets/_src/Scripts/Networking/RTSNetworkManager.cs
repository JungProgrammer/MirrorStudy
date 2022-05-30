using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] 
    private GameObject _unitBasePrefab = null;


    [SerializeField] 
    private GameOverHandler _gameOverHandlerPrefab;


    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;


    private bool _isGameInProgress = false;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();


    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!_isGameInProgress)
            return;
        
        
        conn.Disconnect();
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Remove(player);
        
        base.OnServerDisconnect(conn);
    }


    public override void OnStopServer()
    {
        Players.Clear();

        _isGameInProgress = false;
    }


    public void StartGame()
    {
        if (Players.Count < 2)
            return;


        _isGameInProgress = true;
        
        ServerChangeScene("Scene_Map_01");
    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        
        Players.Add(player);
        
        player.SetTeamColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1));
        
        player.SetPartyOwner(Players.Count == 1);
    }


    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);
            
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);


            foreach (RTSPlayer player in Players)
            {
                GameObject baseInstance = Instantiate(_unitBasePrefab, GetStartPosition().position, Quaternion.identity);
                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }

    #endregion


    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
        ClientOnConnected?.Invoke();
    }


    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        
        ClientOnDisconnected?.Invoke();
    }


    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}
