using System;
using System.Numerics;
using _game.Scripts.Definitions;
using _game.Scripts.UI;
using AYellowpaper;
using DG.Tweening;
using EditorAttributes;
using TMPro;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace _game.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class NetworkPlayerController : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> _health = new NetworkVariable<float>(100f);
        [SerializeField] private InterfaceReference<IGunOld, MonoBehaviour> _gun1;
        [SerializeField] private InterfaceReference<IGunOld, MonoBehaviour> _gun2;

        [SerializeField] private PlayerHUDController _playerHUDController;
        [SerializeField] private NetworkPlayerAnimations _playerAnimations;
        [SerializeField] private Transform _camTransform;
        [SerializeField] private NetworkPlayerMovement _networkMovement;
        [SerializeField] private NetworkObject _gunPrefab;
        [SerializeField] private Transform _hand;

        private IGunOld _equippedGun;
        private Transform _equippedGunTransform;
        private PlayerInput _playerInput;
        private float _cameraAngle;

        private ClientRpcParams _ownerRpcParams;
        private ulong _lastHitById;


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
                _playerHUDController.gameObject.SetActive(true);
                SpawnGunServerRpc();
                _playerAnimations.SetAvatar(-1);
                _playerAnimations.SetWeapon(3);
            }
            else
            {
                vCam.Priority = 0;
                _playerHUDController.gameObject.SetActive(false);
                _playerAnimations.SetAvatar(0);
                _playerAnimations.SetWeapon(3);
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

            _equippedGun = NetworkManager.SpawnManager.SpawnedObjects[id].GetComponent<IGunOld>();
            _equippedGunTransform = NetworkManager.SpawnManager.SpawnedObjects[id].GetComponent<Transform>();
            _playerHUDController.SetCurrentGun(_equippedGun);
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
            if (IsOwner)
                _equippedGunTransform.SetPositionAndRotation(_hand.position, _hand.rotation);
            HandleDeath();
        }

        private void HandleDeath()
        {
            if (!IsServer)
                return;

            if (_health.Value <= 0)
            {
                string deadPlayerId = OwnerClientId.ToString();
                string killerPlayerId = _lastHitById.ToString();

                BroadcastKillFeedRpc(deadPlayerId, killerPlayerId);

                _networkMovement.TeleportPlayer(new TransformState
                {
                    HasStartedMoving = true,
                    Position = GenerateMazeEdgePosition(),
                    Rotation = Quaternion.identity,
                    Tick = 0
                });
                _health.Value = 100;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void BroadcastKillFeedRpc(string deadPlayerId, string killerPlayerId)
        {
            UIEvents.OnPlayerKill?.Invoke(killerPlayerId, deadPlayerId, 0); // Invoke player kill event so all HUDs get updated
        }

        private void DisplayKill(string killerPlayerId, string deadPlayerId, int weaponId)
        {
            if (!IsOwner)
                return;

            _playerHUDController.DisplayKill($"Player {killerPlayerId}", $"Player {deadPlayerId}", weaponId);
        }

        private Vector3 GenerateMazeEdgePosition()
        {
            Vector2 mazeSize = new Vector2(NetworkMazeController.Instance.MazeSizeX, NetworkMazeController.Instance.MazeSizeY);
            float randomX = Random.Range(0, 2) == 0 ? (Mathf.Ceil(mazeSize.x / 2) - 1) * 4 : Mathf.Floor(mazeSize.x / 2f) * -4;
            float randomY = Random.Range(0, 2) == 0 ? (Mathf.Ceil(mazeSize.y / 2) - 1) * 4 : Mathf.Floor(mazeSize.y / 2f) * -4;
            Vector3 randomPos = new Vector3(randomX, 1, randomY);
            return randomPos;
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
                _equippedGun?.TryShoot(_playerInput.OnFoot.Shoot.inProgress, _camTransform);
            }
        }

        public void TakeDamage(float damage, ulong otherPlayerId)
        {
            if (!IsServer)
                return;

            _lastHitById = otherPlayerId;
            _health.Value -= damage;
        }

        private void UpdateHealth(float oldHealth, float newHealth) { _playerHUDController.SetHealthText(newHealth); }

        private void OnEnable()
        {
            _health.OnValueChanged += UpdateHealth;
            UIEvents.OnPlayerKill += DisplayKill;
        }

        private void OnDisable()
        {
            _health.OnValueChanged -= UpdateHealth;
            UIEvents.OnPlayerKill -= DisplayKill;
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
