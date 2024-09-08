using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : MonoBehaviour
{
   [SerializeField] private Button _serverBtn, _clientBtn, _hostBtn;

void Start()
{
    _serverBtn.onClick.AddListener(OnServerBtnClicked);
    _clientBtn.onClick.AddListener(OnClientBtnClicked);
    _hostBtn.onClick.AddListener(OnHostBtnClicked);
}

private void OnServerBtnClicked()
{
    NetworkManager.Singleton.StartServer();
      this.gameObject.SetActive(false);
}
private void OnClientBtnClicked()
{
    NetworkManager.Singleton.StartClient();
      this.gameObject.SetActive(false);

}
private void OnHostBtnClicked()
{
    NetworkManager.Singleton.StartHost();
    this.gameObject.SetActive(false);
}


}