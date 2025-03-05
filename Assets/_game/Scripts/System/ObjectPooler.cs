using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
public class ObjectPooler: MonoBehaviour
{
	/* [Header("Particles")]
	[SerializeField] GameObject example_part; */
	[Header("Decals")]
	[SerializeField] GameObject gunShot_dec;
	[SerializeField] GameObject blood_part;
	[SerializeField] GameObject blood_dec;
	[SerializeField] GameObject blood_gib_dec;
	[SerializeField] GameObject explosion_dec;
	[SerializeField] GameObject explosion_small_dec;
	[Header("Projectiles")]
	[SerializeField] GameObject bomb;
	[SerializeField] GameObject rocket;
	[SerializeField] GameObject stickyBomb;
	[SerializeField] GameObject bullet;
	[SerializeField] GameObject plasmaBall;
	[SerializeField] GameObject acidSplash;
	[SerializeField] GameObject drone;
	//could also do audio files
	
	ObjectPool objectPooler;
	
	public static ObjectPooler main {get; private set;}
	
	void Awake()
	{
		if(main != null)
		{
			Destroy(this);
		}else
		{
			main = this;
		}
	}
	
	void Start()
	{
		objectPooler = new ObjectPool();
		//objectPooler.SetupPool(example_part.GetComponent<ParticleSystem>(), 20, "example_part");
		objectPooler.SetupPool(gunShot_dec.GetComponent<Transform>(), 40, "gunShot_dec");
		objectPooler.SetupPool(blood_part.GetComponent<Transform>(), 40, "blood_part");
		objectPooler.SetupPool(blood_dec.GetComponent<Transform>(), 40, "blood_dec");
		objectPooler.SetupPool(blood_gib_dec.GetComponent<Transform>(), 40, "blood_gib_dec");
		objectPooler.SetupPool(explosion_dec.GetComponent<Transform>(), 40, "explosion_dec");
		objectPooler.SetupPool(explosion_small_dec.GetComponent<Transform>(), 40, "explosion_small_dec");
		
		objectPooler.SetupPool(bomb.GetComponent<MonoBehaviour>(), 60, "bomb");
		objectPooler.SetupPool(rocket.GetComponent<MonoBehaviour>(), 60, "rocket");
		objectPooler.SetupPool(stickyBomb.GetComponent<MonoBehaviour>(), 60, "stickyBomb");
		objectPooler.SetupPool(bullet.GetComponent<MonoBehaviour>(), 100, "bullet");
		objectPooler.SetupPool(plasmaBall.GetComponent<MonoBehaviour>(), 100, "plasmaBall");
		objectPooler.SetupPool(acidSplash.GetComponent<MonoBehaviour>(), 100, "acidSplash");
		objectPooler.SetupPool(drone.GetComponent<MonoBehaviour>(), 30, "drone");
	}
	
	public void SpawnPooledObject(string poolName, Vector3 position, Quaternion rotation, Transform toParent)
	{
		if(toParent.CompareTag("Player") && toParent.CompareTag("Bot")) return;
		
		Transform instance = objectPooler.DequeueObject<Transform>(poolName);
		
		
		instance.transform.position = position;
		instance.transform.rotation = rotation;
		//component specific initialization
		//instance.Play();
		instance.transform.SetParent(toParent, true);
		instance.gameObject.SetActive(true);
		//instance.transform.localScale = Vector3.one;
	}
	
	public void SpawnProjectile(string poolName, Vector3 position, Quaternion rotation, PlayerStats source, float damage, int weaponIndex)
	{
		MonoBehaviour instance = objectPooler.DequeueObject<MonoBehaviour>(poolName);
		IProjectile proj = instance as IProjectile;
		proj.Initialize(source, damage, weaponIndex, poolName);
		instance.transform.position = position;
		instance.transform.rotation = rotation;
		instance.gameObject.SetActive(true);
	}
	
	public void SpawnDecalParticleObject(string poolName, Vector3 position, Quaternion rotation, Transform toParent)
	{
		ParticleSystem instance = objectPooler.DequeueObject<ParticleSystem>(poolName);
		
		instance.gameObject.SetActive(true);
		instance.transform.position = position;
		instance.transform.rotation = rotation;
		//component specific initialization
		instance.Play();
		instance.transform.SetParent(toParent, true);
	}
	
	public void SpawnParticleObject(string poolName, Vector3 position, Quaternion rotation)
	{
		ParticleSystem instance = objectPooler.DequeueObject<ParticleSystem>(poolName);
		instance.gameObject.SetActive(true);
		instance.transform.position = position;
		instance.transform.rotation = rotation;
		//component specific initialization
		instance.Play();
	}
	
	public void ReturnParticleObject(ParticleSystem part, string name) => objectPooler.EnqueueObject(part, name);
	public void ReturnObject(Transform transform, string name) => objectPooler.EnqueueObject(transform, name);
}

public class ObjectPool
{
	public Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();
	public Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();

	public void SetupPool<T>(T pooledItemPrefab, int poolsize, string dictionaryEntry) where T: Component
	{
		poolDictionary.Add(dictionaryEntry, new Queue<Component>());
		poolLookup.Add(dictionaryEntry, pooledItemPrefab);
		for (int i = 0; i < poolsize; i++)
		{
			T pooledInstance = Object.Instantiate(pooledItemPrefab);
			pooledInstance.gameObject.SetActive(false);
			poolDictionary[dictionaryEntry].Enqueue((T)pooledInstance);
		}
	}
	
	public void EnqueueObject<T>(T item, string name) where T: Component
	{
		if(!item.gameObject.activeSelf) return;
		
		item.transform.position = Vector3.zero;
		poolDictionary[name].Enqueue(item);
		item.gameObject.SetActive(false);
	}
	
	public T DequeueObject<T>(string key) where T: Component
	{
		if(poolDictionary[key].TryDequeue(out var item))
		{
			return (T)item;
		}
		return (T)EnqueueNewInstance(poolLookup[key], key);
	}
	
	public T EnqueueNewInstance<T>(T item, string key) where T: Component
	{
		T newInstance = Object.Instantiate(item);
		newInstance.gameObject.SetActive(false);
		newInstance.transform.position = Vector3.zero;
		poolDictionary[key].Enqueue(newInstance);
		return newInstance;
	}
}
