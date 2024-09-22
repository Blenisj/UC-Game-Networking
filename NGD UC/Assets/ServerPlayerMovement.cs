using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;

    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;


    void Start()
    {
        _playerInput = new MyPlayerInputActions();
        _playerInput.Enable();
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>();

        if (IsServer)
        {
            Move(moveInput);
        }
        else if (IsClient)
        {
            MoveServerRPC(moveInput);
        }


    }

    private void Move(Vector2 _input)
    {
        Vector3 _moveDirection = _input.x * transform.right + _input.y * transform.forward;
        _CC.Move(motion: _moveDirection * _pSpeed * Time.deltaTime);
    }


    [ServerRpc]
    private void MoveServerRPC(Vector2 _input)
    {
        Move(_input);

    }
}