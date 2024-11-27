using System;
using _game.Scripts.Player;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.NewPlayerTest
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private Vector2 _minMaxRotationX;
        [SerializeField] private Transform _camTransform;
        [SerializeField] private NetworkPlayerMovement _networkMovement;
        [SerializeField] private float _shootDistance;
        [SerializeField] private LayerMask _shootLayerMask;
        [SerializeField] private NetworkObject _hitPoint;


        private PlayerInput _playerInput;
        private float _cameraAngle;

        public override void OnNetworkSpawn()
        {
            CinemachineCamera vCam = _camTransform.GetComponent<CinemachineCamera>();
            transform.name = "Player: " + OwnerClientId;

            if (IsOwner)
            {
                vCam.Priority = 100;
            }
            else
            {
                vCam.Priority = 0;
            }
        }

        private void Start()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Vector2 movementInput = _playerInput.OnFoot.Movement.ReadValue<Vector2>();
            Vector2 lookInput = _playerInput.OnFoot.Look.ReadValue<Vector2>();
            bool isJumping = _playerInput.OnFoot.Jump.IsPressed();
            bool isSprinting = _playerInput.OnFoot.Sprint.IsPressed();

            if (IsClient && IsLocalPlayer)
            {
                _networkMovement.ProcessLocalPlayerMovement(movementInput, lookInput, isJumping, isSprinting);
            }
            else
            {
                _networkMovement.ProcessSimulatedPlayerMovement();
            }

            if (IsLocalPlayer && _playerInput.OnFoot.Shoot.inProgress)
            {
                if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit raycastHit, _shootDistance, _shootLayerMask))
                {
                    PlaceHitMarkServerRpc();
                }
            }
        }

        [ServerRpc]
        private void PlaceHitMarkServerRpc()
        {
            if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit raycastHit, _shootDistance, _shootLayerMask))
            {
                NetworkObject newHotpoint = Instantiate(_hitPoint, raycastHit.point, Quaternion.identity);
                //newHotpoint.transform.position = position;
                newHotpoint.Spawn();

                if (raycastHit.transform.TryGetComponent(out NetworkObject networkObject))
                {
                    print(networkObject.name);
                    newHotpoint.TrySetParent(networkObject);
                }
            }

        }

        [ServerRpc]
        private void PlaceHitMarkServerRpc(Vector3 position)
        {
            NetworkObject newHotpoint = Instantiate(_hitPoint);
            newHotpoint.transform.position = position;
            newHotpoint.Spawn();
        }

        // private void RotateCamera(Vector2 lookInput)
        // {
        //     _cameraAngle = Vector3.SignedAngle(transform.forward, _camTransform.forward, _camTransform.right);
        //     float cameraRotationAmount = lookInput.y * _turnSpeed * Time.deltaTime;
        //     float newCameraAngle = _cameraAngle - cameraRotationAmount;
        //     if (newCameraAngle <= _minMaxRotationX.x && newCameraAngle >= _minMaxRotationX.y)
        //     {
        //         _camTransform.Rotate(_camTransform.right, -lookInput.y * _turnSpeed * Time.deltaTime, Space.World);
        //     }
        // }
    }
}
