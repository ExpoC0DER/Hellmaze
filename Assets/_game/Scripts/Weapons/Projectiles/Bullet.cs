using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour, IProjectile
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public PlayerStats Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }
	public int WeaponIndex { get; set; }

	[SerializeField] GameObject visual_model;
	[SerializeField] Rigidbody rb;
	[SerializeField] float init_force = 10;
	[SerializeField] float fly_time = 3;
	
	bool onTrigger = false;
	bool flying = false;
	
	public void Initialize(PlayerStats source, float damage, int weaponIndex, string poolName)
	{
		this.Source = source;
		this.Damage = damage;
		this.PoolName = poolName;
		this.WeaponIndex = weaponIndex;
		onTrigger = false;
		flying = true;
		visual_model.SetActive(true);
	}
	
	
	IEnumerator ReturnRoutine()
	{
		yield return new WaitForSeconds(0.1f);
		onTrigger = true;
		yield return new WaitForSeconds(fly_time);
		ReturnToPool();
		
	}
	
	void FixedUpdate()
	{
		if(!gameObject.activeSelf || !flying) return;
		rb.MovePosition(rb.position + init_force * transform.forward * Time.fixedDeltaTime);
	}
	
	void OnCollisionEnter(Collision other)
	{
		if(other.transform == Source.transform && onTrigger == false || !flying) return;
		
		//ContactPoint cp = other.GetContact(0);
		Physics.Raycast(transform.position, transform.forward , out RaycastHit hit, 1, LayerMask.GetMask("Default"));
		ShotEffect(other.transform, hit.point, hit.normal);
	}
	
	/* void OnTriggerEnter(Collider other)
	{
		if(other.transform == Source.transform || onTrigger == false) return;
		
		Physics.Raycast(transform.position, other.ClosestPoint(transform.position), out RaycastHit hit, 1);
		
		ShotEffect(other.transform, hit.point, hit.normal);
	} */
	
	void ShotEffect(Transform other, Vector3 hitPos, Vector3 hitRot)
	{
		
		if(other.gameObject.TryGetComponent(out IDestructable destructable))
		{
			destructable.TakeDamage(Damage, Source, WeaponIndex);
		}
		if(other.transform.CompareTag("Player") || other.transform.CompareTag("Bot"))
		{
			ObjectPooler.main.SpawnPooledObject("blood_part", hitPos, Quaternion.LookRotation(hitRot), other.gameObject.transform);
			if(Physics.Raycast(hitPos, transform.forward, out RaycastHit bloodHit, 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
			{
				ObjectPooler.main.SpawnPooledObject("blood_dec", bloodHit.point, Quaternion.LookRotation(bloodHit.normal), bloodHit.transform);
			}
			
		}else
		{
			ObjectPooler.main.SpawnPooledObject("gunShot_dec", hitPos, Quaternion.LookRotation(hitRot), other.gameObject.transform);
		}
		LG_tools.DrawPoint(transform.position, 15, Color.red);
		LG_tools.DrawPoint(hitPos, 15, Color.yellow);
		LG_tools.DrawLine(hitPos, transform.position, Color.green, 15);
		onTrigger = false;
		visual_model.SetActive(false);
		Invoke("ReturnToPool", 1);
	}
	
	public void ReturnToPool()
	{
		if(!flying) return;
		ObjectPooler.main.ReturnObject(transform, PoolName);
		flying = false;
	}
	
	void OnEnable()
	{
		StartCoroutine(ReturnRoutine());
	}
}
