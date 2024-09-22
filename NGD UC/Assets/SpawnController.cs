using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class SpawnController : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject _playerPrefab;
    [SerializeField]
    private Transform[] _spawnPoints;

    [SerializeField]
    private NetworkVariable<int> _playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private TMP_Text _countTxt;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        }
        _playerCount.OnValueChanged += PlayerCountChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
        }
        _playerCount.OnValueChanged -= PlayerCountChanged;
    }

    private void PlayerCountChanged(int previousValue, int newValue)
    {
        UpdateCountTextClientRpc(newValue);
    }

    [ClientRpc]
    private void UpdateCountTextClientRpc(int newValue)
    {
        UpdateCountText(newValue);
    }

    private void UpdateCountText(int newValue)
    {        
        Debug.Log("Message From Client RPC");
        _countTxt.text = $"Players : {newValue}"; 
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        _playerCount.Value++;
    }

    private void OnClientDisconnectedCallback(ulong clientId)
    {
        _playerCount.Value--;
    }
    
     public void SpawnAllPlayers()
    {

        if (!IsServer) return;

        int spawnNum = 0;
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            NetworkObject spawnedPlayerNO = NetworkManager.Instantiate(_playerPrefab, _spawnPoints[spawnNum].position,
                _spawnPoints[spawnNum].rotation);
            
            spawnedPlayerNO.SpawnAsPlayerObject(clientId);
            spawnNum++;
        }

    }
}