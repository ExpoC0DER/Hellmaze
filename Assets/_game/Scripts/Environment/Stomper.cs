using System.Collections;
using UnityEngine;

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
				}
				
			}else
			{
				_stompBlock.Translate(Vector3.up * _moveUpSpeed * Time.deltaTime);
			}
		}else
		{
			if(_stompBlock.localPosition.y <= _minHeight)
			{
				_cooldown -= Time.deltaTime;
				if(_cooldown <= 0)
				{
					_isStomping = false;
					_damagingFloor.SetActive(false);
					_cooldown = _waitUp;
				}
			}else
			{
				_stompBlock.Translate(Vector3.down * _moveDownSpeed * Time.deltaTime);
			}
		}
	}
	
}
