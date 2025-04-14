using System;
using System.Collections;
using System.Collections.Generic;
using _game.Scripts.Definitions;
using _game.Scripts.Player;
using _game.Scripts.Utils;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts
{
    public class BasicGunOld : NetworkBehaviour, IGunOld
    {
        [SerializeField] private GunSettings _gunSettings;
        [SerializeField] private Transform _hitPoint;
        [field: SerializeField] public int Ammo { get; set; }
        [field: SerializeField] public int MaxAmmo { get; set; } = 30;
        [field: SerializeField] public float Damage { get; set; } = 5f;

        private bool _fired;
        private float _fireDelay;

        private EventInstance _automaticSound;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Transform _camera;
        private IGun _gunImplementation;

        private void Start()
        {
            _camera = Camera.main!.transform;
            // Store the initial position and rotation
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;

            Ammo = MaxAmmo;
        }

        private void Update()
        {
            if (_fireDelay > 0)
                _fireDelay -= Time.deltaTime;

            ShowGun(IsOwner);
        }

        private void ShowGun(bool value)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(value);
            }
        }

        public void ApplyRecoil()
        {
            transform.DOKill(true);
            // Position recoil (kickback)
            transform.DOLocalMove(_originalPosition + _gunSettings.RecoilKickback, _gunSettings.RecoilDuration)
                .SetEase(_gunSettings.RecoilEase)
                .OnComplete(() =>
                {
                    // Return to original position
                    transform.DOLocalMove(_originalPosition, _gunSettings.RecoilReturnDuration)
                        .SetEase(_gunSettings.ReturnEase);
                });

            // Rotation recoil
            transform.DOLocalRotate(_originalRotation.eulerAngles + _gunSettings.RecoilRotation, _gunSettings.RotationDuration)
                .SetEase(_gunSettings.RecoilEase)
                .OnComplete(() =>
                {
                    // Return to original rotation
                    transform.DOLocalRotate(_originalRotation.eulerAngles, _gunSettings.RotationReturnDuration)
                        .SetEase(_gunSettings.ReturnEase);
                });
        }

        public void TryShoot(bool keyDown, Transform point)
        {
            if (keyDown)
            {
                if (Ammo <= 0 && Ammo != -1) // Do nothing if out of ammo and ammo isn't set to infinite
                    return;

                if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Manual && _fired == false)
                {
                    _fired = true;
                    FMODHelper.PlayNewInstance(_gunSettings.ManualSound);

                    if (Ammo != -1)
                        Ammo--;
                    PlayerEvents.OnPlayerShot?.Invoke(Ammo);

                    if (IsLocalPlayer)
                        ApplyRecoil();

                    PlaceHitMarkServerRpc(Damage, point.position, point.forward);
                }
                if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Automatic && _fireDelay <= 0)
                {
                    if (!_fired)
                    {
                        _fired = true;
                        _automaticSound = FMODHelper.CreateNewInstance(_gunSettings.AutomaticSound);
                        _automaticSound.setParameterByName("Parameter 1", 1);
                        _automaticSound.start();
                    }

                    _fireDelay = _gunSettings.FiringSpeed;
                    
                    if (Ammo != -1)
                        Ammo--;
                    PlayerEvents.OnPlayerShot?.Invoke(Ammo);

                    // If local player play recoil animation instantly
                    if (IsLocalPlayer)
                        ApplyRecoil();

                    PlaceHitMarkServerRpc(Damage, point.position, point.forward);
                }
            }
            else
            {
                if (FMODHelper.InstanceIsPlaying(_automaticSound))
                {
                    _automaticSound.setParameterByName("Parameter 1", 0);
                    _automaticSound.release();
                }
                _fired = false;
            }
        }

        [ServerRpc]
        private void PlaceHitMarkServerRpc(float damage, Vector3 position, Vector3 direction)
        {
            if (Physics.Raycast(position, direction, out RaycastHit raycastHit))
            {
                //NetworkObject newHotpoint = Instantiate(_hitPoint, raycastHit.point, Quaternion.identity);
                //newHotpoint.transform.position = position;
                //newHotpoint.Spawn();

                // if (raycastHit.transform.TryGetComponent(out NetworkObject networkObject))
                // {
                //     print(networkObject.name);
                //     newHotpoint.TrySetParent(networkObject);
                // }

                if (raycastHit.transform.TryGetComponent(out NetworkPlayerController playerController))
                {
                    playerController.TakeDamage(damage, OwnerClientId);
                }
            }
            ApplyRecoilClientRpc();
        }

        [ClientRpc]
        private void ApplyRecoilClientRpc()
        {
            // Play the recoil animation for other other players too
            if (!IsLocalPlayer)
                ApplyRecoil();
        }
    }
}
