using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _game.Scripts
{
    public class BasicGun : MonoBehaviour, IGun
    {
        [SerializeField] private GunSettings _gunSettings;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Transform _hitPoint;

        private bool _fired;
        private float _fireDelay;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private void Start()
        {
            // Store the initial position and rotation
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;
        }

        private void Update()
        {
            if (_fireDelay > 0)
                _fireDelay -= Time.deltaTime;
        }

        private void ApplyRecoil()
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

        public void Shoot(bool keyDown)
        {
            if (keyDown)
            {
                if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Manual && _fired == false)
                {
                    _fired = true;
                    FireRaycast();
                }
                else if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Automatic && _fireDelay <= 0)
                {
                    FireRaycast();
                    _fireDelay = _gunSettings.FiringSpeed;
                }
            }
            else
            {
                _fired = false;
            }
        }

        private void FireRaycast()
        {
            if (Physics.Raycast(_shootPoint.position, _shootPoint.forward, out RaycastHit hit))
            {
                Instantiate(_hitPoint.gameObject, hit.point, Quaternion.identity);
            }
            ApplyRecoil();
        }
    }
}
