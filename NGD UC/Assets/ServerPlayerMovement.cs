using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using App.Resource.Scripts.Obj;


[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovement : NetworkBehaviour
{

    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetAnimator;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private float _pSpeed;
    [SerializeField] private Transform _pTransform;
    Vector2 _moveDirection = new Vector3(0, 0f, 0);

    public CharacterController _CC;
    private MyPlayerInputActions _playerInput;
    
    void Start()
    {

        if (_myAnimator == null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }

        if (_myNetAnimator == null)
        {
            _myNetAnimator = gameObject.GetComponent<NetworkAnimator>();
        }
        
        _playerInput = new();
        _playerInput.Enable();
    }

    void FixedUpdate()
    {
        
        if(!IsOwner) return;
        Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>();

        bool IsJumping = _playerInput.Player.Jumping.triggered;
        bool IsPunching = _playerInput.Player.Punching.triggered;
        bool IsRunning = _playerInput.Player.Running.triggered;

        
        if (IsServer)
        {
            Move(moveInput, IsJumping, IsPunching, IsRunning);
        }
        else if (IsClient && !IsHost)
        {
            MoveServerRPC(moveInput, IsJumping, IsPunching, IsRunning);
        }

        if (IsPunching)
        {
            _bulletSpawner.FireProjectileRpc();
        }
        
    }
    
    private void Move(Vector2 _input, bool isRunning, bool isJumping, bool isPunching)
    {
        _moveDirection = new Vector3(_input.x, 0f, _input.y);

        _myAnimator.SetBool("IsWalking", _input.x != 0 || _input.y != 0);

        if (isJumping){ _myNetAnimator.SetTrigger("JumpTrigger");}
        if (isPunching){ _myNetAnimator.SetTrigger("PunchTrigger");}

        _myAnimator.SetBool("IsRunning", isRunning);
        if (isRunning){
            _CC.Move(_moveDirection * (_pSpeed * 1.3f) * Time.deltaTime);
        }
        else{
            _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);
        }

        _pTransform.forward = _moveDirection;
        

    }

    [Rpc(SendTo.Server)]
   private void MoveServerRPC(Vector2 _input, bool isRunning, bool isJumping, bool isPunching)
   {
       Move(_input, isJumping, isPunching, isRunning);
   }
    
    
}