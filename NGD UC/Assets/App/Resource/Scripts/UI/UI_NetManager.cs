using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : MonoBehaviour
{
   [SerializeField] private Button _serverBtn, _clientBtn, _hostBtn, _startBtn;

   [SerializeField] private GameObject _connectionBttnGroup;

   [SerializeField] private SpawnController _mySpawnController;

void Start()
{
    _startBtn.gameObject.SetActive(false);
    _serverBtn.onClick.AddListener(OnServerBtnClicked);
    _clientBtn.onClick.AddListener(OnClientBtnClicked);
    _hostBtn.onClick.AddListener(OnHostBtnClicked);
    _startBtn.onClick.AddListener(OnStartBtnClicked);
}

private void OnServerBtnClicked()
{
    NetworkManager.Singleton.StartServer();
      _connectionBttnGroup.SetActive(false);
      _startBtn.gameObject.SetActive(true);
}
private void OnClientBtnClicked()
{
    NetworkManager.Singleton.StartClient();
      _connectionBttnGroup.SetActive(false);

}
private void OnHostBtnClicked()
{
    NetworkManager.Singleton.StartHost();
    _connectionBttnGroup.SetActive(false);
    _startBtn.gameObject.SetActive(true);
}
private void OnStartBtnClicked()
{
    _mySpawnController.SpawnAllPlayers();
    _startBtn.gameObject.SetActive(false);
}
}