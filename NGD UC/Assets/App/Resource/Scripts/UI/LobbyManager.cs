using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
    [SerializeField] private GameObject _panelPrefab;
    [SerializeField] private GameObject _ContentGo;
    [SerializeField] private TMP_Text rdyTxt;
    [SerializeField] private NetworkedPlayerData _networkPlayers;

    private List<GameObject> _PlayerPanels = new List<GameObject>();
    private ulong _myServerID;
    private bool isReady = false;

    private void Start()
    {
        _myServerID = NetworkManager.ServerClientId;

        if (IsServer)
        {
            rdyTxt.text = "Waiting for Players";
            _readyBttn.gameObject.SetActive(false);
        }
        else
        {
            rdyTxt.text = "Not Ready";
            _readyBttn.gameObject.SetActive(true);
        }

        _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
        _leaveBttn.onClick.AddListener(LeaveBttnClick);
        _readyBttn.onClick.AddListener(ClientRdyBttnToggle);
    }

    private void ClientRdyBttnToggle()
    {
        if(IsServer){return;}

        isReady = !isReady;
        if(isReady)
        {
            rdyTxt.text = "Ready";
        }
        else
        {
            rdyTxt.text = "Not Ready";
        }

        RdyBttnToggleServerRPC(isReady);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RdyBttnToggleServerRPC(bool readyStatus, RpcParams rpcParams = default)
    {
        Debug.Log("From REady button RPC");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }

    private void LeaveBttnClick()
    {
        if (IsServer)
        {
            QuitLobbyServerRPC();
        }
        else
        {
            foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
            {
                if (playerData._clientId == _myServerID)
                {
                    KickUserBttn(playerData._clientId);
                }

            }
            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void QuitLobbyServerRPC(RpcParams rpcParams = default)
    {
        KickUserBttn(rpcParams.Receive.SenderClientId);
    }
    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeEvent)
    {
        Debug.Log("Net players has changed, event Fired!");
        PopulateLabels();
    }

    [ContextMenu("PopulateLabels")]
    private void PopulateLabels()
    {
        ClearPlayerPanel();

        bool allReady = true;

        foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            GameObject newPlayerPanel = Instantiate(_panelPrefab, _ContentGo.transform);
            PlayerLabel _playerLabel = newPlayerPanel.GetComponent<PlayerLabel>();

            _playerLabel.onKickClicked += KickUserBttn;

            if (IsServer && playerData._clientId != _myServerID)
            {
                _playerLabel.SetKickActive(true);
                _readyBttn.gameObject.SetActive(false);
            }
            else
            {
                _playerLabel.SetKickActive(false);
                _readyBttn.gameObject.SetActive(true);
            }

            _playerLabel.SetPlayerLabelName(playerData._clientId);
            _playerLabel.SetReady(playerData._isPlayerReady);
            _playerLabel.SetPlayerColor(playerData._colorId);
            _PlayerPanels.Add(newPlayerPanel);

            if (playerData._isPlayerReady == false)
            {
                allReady = false;
            }

        }

        if (IsServer)
        {
            if (allReady)
            {
                if(_networkPlayers._allConnectedPlayers.Count > 1)
                {
                    rdyTxt.text = "All Players Ready";
                    _startBttn.gameObject.SetActive(true);
                }
                else
                {
                    rdyTxt.text = "Empty Lobby";
                }
            }
            else
            {
                rdyTxt.text = "Waiting for all players to be ready";
                _startBttn.gameObject.SetActive(false);
            }
        }
    }

    private void ClearPlayerPanel()
    {
        foreach (GameObject panel in _PlayerPanels)
        {
            Destroy(panel);
        }

        _PlayerPanels.Clear();
    }

    private void KickUserBttn(ulong kickTarget)
    {
        if (!IsServer || !IsHost){return;}

        foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            if (playerData._clientId == kickTarget)
            {
                _networkPlayers.RemovePlayerData(playerData);

                KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]

    private void KickedClientRpc(RpcParams rpcParams = default)
    {
        SceneManager.LoadScene(0);
    }

}
