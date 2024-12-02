using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;

namespace _game.Scripts
{
	public class BasicGun : MonoBehaviour, IGun
	{
		[SerializeField] private GunSettings _gunSettings;
		[SerializeField] private Transform _hitPoint;
		[SerializeField] private Transform _fleshHitPoint;
		[SerializeField] ParticleSystem muzzleFlash_part;
		[SerializeField] ParticleSystem bullet_fire_part;
		[SerializeField] ParticleSystem casing_part;
		[field: SerializeField] public int Ammo { get; set; } = 30;
		[field: SerializeField] public int MaxAmmo { get; set; } = 120;
		[field: SerializeField] public float BotInaccuracy { get; set; } = 1;
		[field: SerializeField] public float DamageMultiplier { get; set; } = 0.5f;
		public PlayerStats Source { get; set; }
        public int WeaponIndex { get; set; }

        private bool _fired;
		private float _fireDelay;

		private EventInstance _automaticSound;

		private Vector3 _originalPosition;
		private Quaternion _originalRotation;
		private Transform _camera;
		

		private void Start()
		{
			_camera = Camera.main!.transform;
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

		public void PickAmmo(int amount)
		{
			if(Ammo < MaxAmmo)
			{
				Ammo += amount;
				if(Ammo > MaxAmmo) Ammo = MaxAmmo;
			}
			
		}
		
		public void Shoot(bool keyDown, Transform target, out bool succesShot)
		{
			succesShot = Ammo != 0;
			if (keyDown)
			{
				if(!succesShot)
				{
					//play no ammo click sound
					return;
				}
				if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Manual && _fired == false)
				{
					_fired = true;
					FMODHelper.PlayNewInstance(_gunSettings.ManualSound);
					FireRaycast(target);
				}
				if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Automatic && _fireDelay <= 0)
				{
					FireRaycast(target);

					if (!_fired)
					{
						_fired = true;
						_automaticSound = FMODHelper.CreateNewInstance(_gunSettings.AutomaticSound);
						_automaticSound.setParameterByName("Parameter 1", 1);
						_automaticSound.start();
					}

					_fireDelay = _gunSettings.FiringSpeed;
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
		
		public void StopShooting()
		{
			if (FMODHelper.InstanceIsPlaying(_automaticSound))
				{
					_automaticSound.setParameterByName("Parameter 1", 0);
					_automaticSound.release();
				}
				_fired = false;
		}

		private void FireRaycast(Transform target)
		{
			Vector3 dir;
			Vector3 orig;
			if (Ammo > 0)
				Ammo--;
			if(target == null)
			{
				dir = _camera.forward;
				orig = _camera.position + _camera.forward * 0.13f; //PLAYER SHOT ITSELF (QUICKFIX)
			}else
			{
				orig = transform.position;
				dir = (new Vector3(target.position.x, target.position.y +0.75f, target.position.z) - transform.position).normalized;
			}
			
			BulletParticle(Quaternion.LookRotation(dir));
			
			for (int i = 0; i < _gunSettings.ShotsPerFire; i++)
			{
				dir = GetShootingSpread_Direction(orig, dir);
			
				//Debug.DrawRay(orig, dir * _gunSettings.MaxRange, Color.red, 3);
				
				if (Physics.Raycast(orig, dir, out RaycastHit hit, _gunSettings.MaxRange))
				{
					//GameObject t;
					if(hit.transform.CompareTag("Player") || hit.transform.CompareTag("Bot"))
					{
						ObjectPooler.main.SpawnPooledObject("blood_part", hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
						//t = Instantiate(_fleshHitPoint.gameObject, hit.point, Quaternion.LookRotation(hit.normal));
					}else
					{
						ObjectPooler.main.SpawnPooledObject("gunShot_dec", hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
						//t = Instantiate(_hitPoint.gameObject, hit.point, Quaternion.LookRotation(hit.normal));
					}
					
					//t.transform.SetParent(hit.transform, true);
					
					//print(hit.transform.name);
					/* if (hit.transform.TryGetComponent(out EnemyAI ai))
					{
						ai.TakeDamage(_gunSettings.Damage * DamageMultiplier, transform.position);
					}else  */
					if (hit.transform.TryGetComponent(out PlayerStats player))
					{
						//Debug.Log("bot shot player " + _gunSettings.Damage);
						player.TakeDamage(_gunSettings.Damage * DamageMultiplier, Source, WeaponIndex);
					}
				}
			}
			
			muzzleFlash_part.Play();
			casing_part.Play();
			ApplyRecoil();
		}
		
		Vector3 GetShootingSpread_Direction(Vector3 origin, Vector3 direction)
		{
			Vector3 targetPos = origin + direction * _gunSettings.MaxRange;
			targetPos = new Vector3(
				targetPos.x - UnityEngine.Random.Range(-_gunSettings.Spread - BotInaccuracy, _gunSettings.Spread + BotInaccuracy),
				targetPos.y - UnityEngine.Random.Range(-_gunSettings.Spread - BotInaccuracy, _gunSettings.Spread + BotInaccuracy),
				targetPos.z - UnityEngine.Random.Range(-_gunSettings.Spread - BotInaccuracy, _gunSettings.Spread + BotInaccuracy)
				);
			Vector3 dir = targetPos - origin;
			return dir.normalized;
		}
		
		void BulletParticle(Quaternion rotation)
		{
			bullet_fire_part.transform.rotation = rotation;
			bullet_fire_part.Play();
		}
	}
}
