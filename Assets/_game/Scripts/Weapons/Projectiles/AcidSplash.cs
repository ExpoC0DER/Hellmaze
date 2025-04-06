using UnityEngine;
using System.Collections;
using FMODUnity;
using _game.Scripts;

public class AcidSplash : MonoBehaviour, IProjectile
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public PlayerStats Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }
	public int WeaponIndex { get; set; }

	[SerializeField] Rigidbody rb;
	[SerializeField] float init_force = 10;
	
	[SerializeField] GameObject splashObj;
	[SerializeField] GameObject visualObj;
	
	[SerializeField] EventReference setup_sound;
	
	bool setup = false;
	bool onTrigger = false;
	
	
	public void Initialize(PlayerStats source, float damage, int weaponIndex, string poolName)
	{
		this.Source = source;
		this.Damage = damage;
		this.PoolName = poolName;
		this.WeaponIndex = weaponIndex;
		onTrigger = false;
		setup = false;
		rb.isKinematic = false;
		visualObj.SetActive(true);
		splashObj.SetActive(false);
	}
	
	IEnumerator ReturnRoutine()
	{
		yield return new WaitForSeconds(0.1f);
		onTrigger = true;
		yield return new WaitForSeconds(20);
		ReturnToPool();
		
	}
	
	/* void OnTriggerEnter(Collider other)
	{
		if(!setup)
		{
			if(other.CompareTag("Player") || other.CompareTag("Bot") && onTrigger)
			{
				Spawn(other.GetComponent<PlayerStats>(), Vector3.zero);
			}else
			{
				Spawn(null, other.ClosestPoint(transform.position));
			}
			Debug.Log("by trigger " + other.gameObject.name);
			setup = true;
		}
	} */
	
	void OnCollisionEnter(Collision other)
	{
		if(!setup && onTrigger)
		{
			if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot") && onTrigger)
			{
				Setup(other.gameObject.GetComponent<PlayerStats>(),  Vector3.zero);
			}else
			{
				Setup(null, other.GetContact(0).point);
			}
			Debug.Log("by collision " + other.transform.name);
			setup = true;
		}
	}
	
	void Setup(PlayerStats player, Vector3 colPoint)
	{
		LG_tools.DrawRay(transform.position, transform.forward * 2, Color.green);
		FMODHelper.PlayNewInstance(setup_sound, transform);
		rb.isKinematic = true;
		Vector3 pos = transform.position;
		Vector3 rot = Vector3.zero;
		Vector3 dir = (colPoint - transform.position).normalized;
		
		if(player)
		{
			player.TakeDamage(Damage, Source, WeaponIndex);
			dir = Vector3.down;
		}
		if(Physics.Raycast(pos, dir, out RaycastHit hit))
		{
			pos = hit.point;
			rot = hit.normal;
		}
		
		//LG_tools.DrawPoint(transform.position, 60, Color.blue);
		//LG_tools.DrawPoint(pos, 60, Color.green);
		LG_tools.DrawLine(transform.position, pos, Color.red, 60);
		//Debug.Log("end " + pos +" start " + transform.position);
		
		splashObj.transform.position = pos;
		splashObj.transform.rotation = Quaternion.LookRotation(rot);
		splashObj.SetActive(true);
		
		visualObj.SetActive(false);
		
	}
	
	void OnEnable()
	{
		rb.AddForce(init_force * transform.forward, ForceMode.Impulse);
		StartCoroutine(ReturnRoutine());
	}
	public void ReturnToPool()
	{
		ObjectPooler.main.ReturnObject(transform, PoolName);
	}
}
