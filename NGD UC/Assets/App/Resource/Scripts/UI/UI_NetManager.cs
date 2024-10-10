using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{

    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn, _startBttn;

    [SerializeField] private GameObject _connectionBttnGroup;

    [SerializeField] private SpawnController _mySpawnController;

    void Start()
    {
        _startBttn.gameObject.SetActive(false);
        if (_hostBttn != null) _hostBttn.onClick.AddListener(Hostclick);
        if (_clientBttn != null) _clientBttn.onClick.AddListener(ClientClick);
        if (_serverBttn != null) _serverBttn.onClick.AddListener(ServerClick);
        if (_startBttn != null) _startBttn.onClick.AddListener(StartClick);
    }

    private void StartClick()
    {
       if (IsServer)
       {
           _mySpawnController.SpawnAllPlayers();
           _startBttn.gameObject.SetActive(false);
       }
    }


    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();    
        _connectionBttnGroup.SetActive(false);
        _startBttn.gameObject.SetActive(true);
    }
    
    private void ClientClick()
    {
       NetworkManager.Singleton.StartClient();     
       _connectionBttnGroup.SetActive(false);
    }

    private void Hostclick()
    {
        NetworkManager.Singleton.StartHost();        
        _connectionBttnGroup.SetActive(false);
        _startBttn.gameObject.SetActive(true);
    }
}