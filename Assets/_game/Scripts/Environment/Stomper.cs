using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class Stomper : MonoBehaviour
{
	[SerializeField] Transform _stompBlock;
	[SerializeField] GameObject _damagingFloor;
	[SerializeField] int _weaponIndex = 0;
	[SerializeField] float _damage = 300;
	[SerializeField] float _moveDownSpeed = 10;
	[SerializeField] float _moveUpSpeed = 5;
	[SerializeField] float _minHeight = 1;
	[SerializeField] float _maxHeight = 4;
	[SerializeField] float _waitDown = 2;
	[SerializeField] float _waitUp = 5;
	float _cooldown;
	bool _isStomping = false;
	bool soundPlaying = false;
	bool hitSfx_played = false;
	[SerializeField] EventReference move_sfx;
	[SerializeField] EventReference hit_sfx;
	private EventInstance moveSound;
	
	void Start()
	{
		_cooldown = _waitUp;
	}
	
	void Update()
	{
		if(!_isStomping)
		{
			if(_stompBlock.localPosition.y >= _maxHeight)
			{
				_cooldown -= Time.deltaTime;
				if(_cooldown <= 0)
				{
					_isStomping = true;
					_damagingFloor.SetActive(true);
					_cooldown = _waitDown;
					hitSfx_played = false;
				}
				StopSound();
				
			}else
			{
				_stompBlock.Translate(Vector3.up * _moveUpSpeed * Time.deltaTime);
				PlaySound();
			}
		}else
		{
			if(_stompBlock.localPosition.y <= _minHeight)
			{
				if(!hitSfx_played)
				{
					FMODHelper.PlayNewInstance(hit_sfx, transform);
					hitSfx_played = true;
				}
				
				_cooldown -= Time.deltaTime;
				if(_cooldown <= 0)
				{
					_isStomping = false;
					_damagingFloor.SetActive(false);
					_cooldown = _waitUp;
				}
				
				StopSound();
			}else
			{
				_stompBlock.Translate(Vector3.down * _moveDownSpeed * Time.deltaTime);
				PlaySound();
			}
		}
	}
	
	void PlaySound()
	{
		if(soundPlaying) return;
		
		moveSound = FMODHelper.CreateNewInstance(move_sfx, transform);
		moveSound.start();
		soundPlaying = true;
		
	}
	void StopSound()
	{
		if(!soundPlaying) return;
		
		if (FMODHelper.InstanceIsPlaying(moveSound))
		{
			moveSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			moveSound.release();
		}
		soundPlaying = false;
	}
	
}
