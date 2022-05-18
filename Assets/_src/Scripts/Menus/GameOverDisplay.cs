using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] 
    private GameObject _gameOverDisplayParent = null;
    
    
    [SerializeField] 
    private TMP_Text _winnerNameText;
    
    
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }


    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }


    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // stop hosting
            
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // stop client
            
            NetworkManager.singleton.StopClient();
        }
    }


    private void ClientHandleGameOver(string winnerName)
    {
        _winnerNameText.text = $"{winnerName} has won!";
        
        _gameOverDisplayParent.SetActive(true);
    }
}
