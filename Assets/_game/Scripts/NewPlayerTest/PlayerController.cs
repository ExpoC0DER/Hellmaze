using System;
using _game.Scripts.Player;
using AYellowpaper;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.NewPlayerTest
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> _health = new NetworkVariable<float>(100f);
        [SerializeField] private InterfaceReference<IGun, MonoBehaviour> _gun1;
        [SerializeField] private InterfaceReference<IGun, MonoBehaviour> _gun2;

        [SerializeField] private Vector2 _minMaxRotationX;
        [SerializeField] private Transform _camTransform;
        [SerializeField] private NetworkPlayerMovement _networkMovement;
        [SerializeField] private float _shootDistance;
        [SerializeField] private LayerMask _shootLayerMask;
        [SerializeField] private NetworkObject _hitPoint;


        private IGun _equippedGun;
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

            _equippedGun = _gun1.Value;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleMovement();
            HandleShooting();
        }

        private void HandleMovement()
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
        }

        private void HandleShooting()
        {
            if (IsClient && IsLocalPlayer)
                _equippedGun?.TryShoot(_playerInput.OnFoot.Shoot.inProgress, _camTransform);
        }

        public void TakeDamage(float damage)
        {
            if (!IsServer)
                return;

            _health.Value -= damage;
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
