using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{

    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn, _startBttn;

    [SerializeField] private GameObject _connectionBttnGroup, _socialPanel;

    [SerializeField] private SpawnController _mySpawnController;
    
    void Start()
    {
        _startBttn.gameObject.SetActive(false);
        if (_hostBttn != null) _hostBttn.onClick.AddListener(Hostclick);
        if (_clientBttn != null) _clientBttn.onClick.AddListener(ClientClick);
        if (_serverBttn != null) _serverBttn.onClick.AddListener(ServerClick);
    }

    private void StartClick()
    {
       if (IsServer)
       {
           _mySpawnController.SpawnAllPlayers();
       }
    }


    private void ServerClick()
    {
      var isSuccesfull =  NetworkManager.Singleton.StartServer();     

      if (isSuccesfull)
      {
          _connectionBttnGroup.SetActive(false);
          _socialPanel.SetActive(true);
      }
       // _startBttn.gameObject.SetActive(true);
    }
    
    private void ClientClick()
    {
        var isSuccesfull = NetworkManager.Singleton.StartClient();    
        if (isSuccesfull)
        {
            _connectionBttnGroup.SetActive(false);
            _socialPanel.SetActive(true);
        }
    }

    private void Hostclick()
    {
        var isSuccesfull = NetworkManager.Singleton.StartHost();     
        if (isSuccesfull)
        {
            _connectionBttnGroup.SetActive(false);
            _socialPanel.SetActive(true);
        }

    }
}