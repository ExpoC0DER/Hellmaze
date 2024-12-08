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
			StartCoroutine(DelaySound());
		}
		
		IEnumerator DelaySound()
		{
			yield return new WaitForSeconds(0.04f);
			FMODHelper.PlayNewInstance(sfx[Random.Range(0, sfx.Length)], transform.position);
		}
	}
}
