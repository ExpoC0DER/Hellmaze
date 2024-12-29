using System.Collections;
using FMODUnity;
using UnityEngine;

namespace _game.Scripts
{
	public class HitPoint : MonoBehaviour
	{
		[SerializeField] string pool_name;
		[SerializeField] EventReference[] sfx;
		void Start()
		{
			Invoke("ReturnToPool", 10);
		}
		
		void OnDestroy()
		{
			ReturnToPool();
		}
		
		void ReturnToPool()
		{
			ObjectPooler.main.ReturnObject(transform, pool_name);
		}
		
		void OnEnable()
		{
			FMODHelper.PlayNewInstance(sfx[Random.Range(0, sfx.Length)], transform.position);
			Debug.Log(transform.position);
		}
	}
}
