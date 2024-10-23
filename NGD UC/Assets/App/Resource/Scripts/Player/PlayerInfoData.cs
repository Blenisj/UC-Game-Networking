using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.Player
{
    public struct PlayerInfoData : INetworkSerializable, IEquatable<PlayerInfoData>
    {
        public ulong _clientId;
        public FixedString64Bytes _name;
        public bool _isPlayerReady;
        public Color _ColorId;


        public PlayerInfoData(ulong id)
        {
            _clientId = id;
            _name = "";
            _isPlayerReady = false;
            _ColorId = Color.magenta;

        }


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {

            serializer.SerializeValue(ref _clientId);
            serializer.SerializeValue(ref _name);
            serializer.SerializeValue(ref _isPlayerReady);
            serializer.SerializeValue(ref _ColorId);

            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out _clientId);
                reader.ReadValueSafe(out _name);
                reader.ReadValueSafe(out _isPlayerReady);
                reader.ReadValueSafe(out _ColorId);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(_clientId);
                writer.WriteValueSafe(_name);
                writer.WriteValueSafe(_isPlayerReady);
                writer.WriteValueSafe(_ColorId);
            }


        }

        public bool Equals(PlayerInfoData other)
        {
            return _clientId == other._clientId;
        }

        public override string ToString() => _name.Value.ToString();

        public static implicit operator string(PlayerInfoData name) => name.ToString();

        public static implicit operator PlayerInfoData(string s) =>
            new PlayerInfoData { _name = new FixedString64Bytes(s) };
    }
}