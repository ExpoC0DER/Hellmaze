using UnityEngine;

public class Explosive : MonoBehaviour
{
	[SerializeField] float expl_radius = 5;
	[SerializeField] float expl_force = 5;
	
	protected float _damage;
	protected Transform _source;
	
	[SerializeField] ParticleSystem explosion_part;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public virtual void Explode()
	{
		explosion_part.Play();
		
		Collider[] cols = Physics.OverlapSphere(transform.position, expl_radius);
		
		for (int i = 0; i < cols.Length; i++)
		{
			Transform obj = cols[i].transform;
			if(obj.TryGetComponent(out Rigidbody rb))
			{
				rb.AddExplosionForce(expl_force, transform.position, expl_radius);
				float distanceRatio = Mathf.InverseLerp(0, expl_radius, Vector3.Distance(transform.position, obj.transform.position));
				float damage = Mathf.Lerp(_damage, 0, distanceRatio);
				if(obj.TryGetComponent(out PlayerStats playerStats))
				{
					playerStats.ChangeHealth(-damage);
				}else if(obj.TryGetComponent(out EnemyAI enemyAI))
				{
					enemyAI.TakeDamage(damage, _source.position);
				}
			}else
			{
				continue;
			}
		}
	}
}
