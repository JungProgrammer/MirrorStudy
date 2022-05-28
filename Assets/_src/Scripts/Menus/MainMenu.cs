using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject _landingPagePanel;


    public void HostLobby()
    {
        _landingPagePanel.gameObject.SetActive(false);
        
        NetworkManager.singleton.StartHost();
    }
}
