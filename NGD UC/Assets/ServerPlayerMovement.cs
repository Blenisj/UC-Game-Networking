using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using App.Resource.Scripts.Obj;

namespace App.Resource.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class ServerPlayerMovement : NetworkBehaviour
    {
        [SerializeField] private Animator _myAnimator;
        [SerializeField] private NetworkAnimator _myNetAnimator;
        [SerializeField] private BulletSpawner _bulletSpawner;
        [SerializeField] private float _pSpeed;
        [SerializeField] private Transform _pTransform;
        Vector3 _moveDirection = Vector3.zero;

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

            _playerInput = new MyPlayerInputActions();
            _playerInput.Enable();
        }

        void FixedUpdate()
        {
            if (!IsOwner) return;

            Vector2 moveInput = _playerInput.Player.Movement.ReadValue<Vector2>();
            moveInput = moveInput.normalized;

            bool isJumping = _playerInput.Player.Jumping.triggered;
            bool isPunching = _playerInput.Player.Punching.triggered;
            bool isRunning = _playerInput.Player.Running.triggered;

            if (IsServer)
            {
                Move(moveInput, isRunning, isJumping, isPunching);
            }
            else if (IsClient && !IsHost)
            {
                MoveServerRpc(moveInput, isRunning, isJumping, isPunching);
            }

            if (isPunching)
            {
                _bulletSpawner.FireProjectileRpc();
            }
        }

        private void Move(Vector2 input, bool isRunning, bool isJumping, bool isPunching)
        {
            _moveDirection = new Vector3(input.x, 0f, input.y);

            _myAnimator.SetBool("IsWalking", _moveDirection.z != 0 || _moveDirection.x != 0);

            if (isJumping) { _myNetAnimator.SetTrigger("JumpTrigger"); }
            if (isPunching) { _myNetAnimator.SetTrigger("PunchTrigger"); }

            _myAnimator.SetBool("IsRunning", isRunning);
            if (isRunning)
            {
                _CC.Move(_moveDirection * (_pSpeed * 1.3f) * Time.deltaTime);
            }
            else
            {
                _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);
            }

            if (_moveDirection != Vector3.zero)
            {
                _pTransform.forward = _moveDirection;
            }
        }

        [ServerRpc]
        private void MoveServerRpc(Vector2 input, bool isRunning, bool isJumping, bool isPunching)
        {
            Move(input, isRunning, isJumping, isPunching);
        }
    }
}