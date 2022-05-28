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
    private GameObject _unitSpawnerPrefab = null;


    [SerializeField] 
    private GameOverHandler _gameOverHandlerPrefab;


    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;


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


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        player.SetTeamColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1));
    }


    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);
            
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
