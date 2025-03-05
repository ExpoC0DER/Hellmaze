using _game.Scripts;
using UnityEngine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;

namespace _game.Scripts
{
	public class ProjectileGun : MonoBehaviour, IGun
	{
		[SerializeField] private GunSettings _gunSettings;
		[SerializeField] private string projectilePool_name;
		[SerializeField] private Transform spawnPoint;
		[SerializeField] ParticleSystem muzzleFlash_part;
		[SerializeField] ParticleSystem casing_part;
		[SerializeField] ParticleSystem particleProjectile_part;
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
		bool noAmmoClicked = false;

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

		public float GetDamage() => _gunSettings.Damage;
		
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
					if(!noAmmoClicked)
					{
						FMODHelper.PlayNewInstance(_gunSettings.NoAmmoSound, transform);
						noAmmoClicked = true;
					}
					return;
				}
				if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Manual && _fired == false && _fireDelay <= 0)
				{
					_fired = true;
					FMODHelper.PlayNewInstance(_gunSettings.ManualSound, transform);
					Fire(target);
					_fireDelay = _gunSettings.FiringSpeed;
				}
				if (_gunSettings.FiringMode == GunSettings.FiringModeSetting.Automatic && _fireDelay <= 0)
				{
					Fire(target);

					if (!_fired)
					{
						_fired = true;
						_automaticSound = FMODHelper.CreateNewInstance(_gunSettings.AutomaticSound, transform);
						_automaticSound.setParameterByName("Parameter 1", 1);
						_automaticSound.start();
					}

					_fireDelay = _gunSettings.FiringSpeed;
				}
			}
			else
			{
				StopShooting();
				noAmmoClicked = false;
			}
		}
		
		public void StopShooting()
		{
			if (FMODHelper.InstanceIsPlaying(_automaticSound))
				{
					_automaticSound.setParameterByName("Parameter 1", 2);
					_automaticSound.release();
				}
				_fired = false;
		}

		private void Fire(Transform target)
		{
			Vector3 dir;
			Vector3 orig;
			if (Ammo > 0)
				Ammo--;
			if(target == null)
			{
				dir = _camera.forward;
				orig = _camera.position;
			}else
			{
				orig = new Vector3(transform.position.x, transform.position.y +0.75f, transform.position.z);
				dir = (new Vector3(target.position.x, target.position.y +0.4f, target.position.z) - orig).normalized;
			}
			
			for (int i = 0; i < _gunSettings.ShotsPerFire; i++)
			{
				
				if(particleProjectile_part)
				{
					particleProjectile_part.transform.rotation = Quaternion.LookRotation(dir);
					particleProjectile_part.Play();
					break;
				}else
				{
					dir = GetShootingSpread_Direction(orig, dir);
					ObjectPooler.main.SpawnProjectile(projectilePool_name, spawnPoint.position, Quaternion.LookRotation(dir), Source, _gunSettings.Damage * DamageMultiplier, WeaponIndex); 
				}
			}
			//Debug.DrawRay(orig, dir * _gunSettings.MaxRange, Color.red, 3);
			
			
			
			if(muzzleFlash_part) muzzleFlash_part.Play();
			if(casing_part) casing_part.Play();
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
	}
}
