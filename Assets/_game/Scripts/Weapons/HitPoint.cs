using UnityEngine;

namespace _game.Scripts
{
	public class HitPoint : MonoBehaviour
	{
		[SerializeField] string pool_name;
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
	}
}
