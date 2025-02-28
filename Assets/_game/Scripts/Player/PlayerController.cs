using System;
using AYellowpaper;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Player
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
        [SerializeField] private NetworkObject _gunPrefab;
        [SerializeField] private Transform _hand;

        public IGun _equippedGun;
        public string test;
        private PlayerInput _playerInput;
        private float _cameraAngle;

        private ClientRpcParams _ownerRpcParams;


        public override void OnNetworkSpawn()
        {
            CinemachineCamera vCam = _camTransform.GetComponent<CinemachineCamera>();
            transform.name = "Player: " + OwnerClientId;

            if (IsOwner)
            {
                _ownerRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { OwnerClientId }
                    }
                };
                
                vCam.Priority = 100;
                SpawnGunServerRpc();
            }
            else
            {
                vCam.Priority = 0;
            }
        }

        [ServerRpc]
        private void SpawnGunServerRpc()
        {
            // Create gun and assign it to player
            NetworkObject newHotpoint = Instantiate(_gunPrefab);
            newHotpoint.transform.SetPositionAndRotation(_hand.position, _hand.rotation);
            newHotpoint.Spawn();
            newHotpoint.TrySetParent(gameObject);
            newHotpoint.ChangeOwnership(OwnerClientId);
            
            // Tell player to equip it
            SetEquippedGunClientRpc(newHotpoint.NetworkObjectId, _ownerRpcParams);
        }

        [ClientRpc]
        private void SetEquippedGunClientRpc(ulong id, ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner)
                return;
            
            _equippedGun = NetworkManager.SpawnManager.SpawnedObjects[id].GetComponent<IGun>();
        }


        private void Start()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();

            //_equippedGun = _gun1.Value;

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
            {
                //if (_equippedGun != null)
                _equippedGun?.TryShoot(_playerInput.OnFoot.Shoot.inProgress, _camTransform);
                // else
                // {
                //     _equippedGun = GetComponentInChildren<IGun>();
                //     test = _equippedGun.Damage.ToString();
                //     _equippedGun?.TryShoot(_playerInput.OnFoot.Shoot.inProgress, _camTransform);
                // }
            }
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
