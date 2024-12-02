using UnityEngine;

public class Explosive : MonoBehaviour
{
	[SerializeField] float expl_radius = 5;
	[SerializeField] float expl_force = 5;
	
	[SerializeField] protected float _damage;
	[SerializeField] protected int _weaponIndex;
	[SerializeField] protected GameObject visual_model;
	protected PlayerStats _source;
	
	[SerializeField] ParticleSystem explosion_part;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public virtual void Explode()
	{
		explosion_part.Play();
		visual_model.SetActive(false);
		
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
					playerStats.TakeDamage(damage, _source, _weaponIndex);
					Debug.Log("explosive damage " + -damage);
				}/* else if(obj.TryGetComponent(out EnemyAI enemyAI))
				{
					enemyAI.TakeDamage(damage, _source.position);
					Debug.Log("explosive damage bot " + damage);
				} */
			}else
			{
				continue;
			}
		}
	}
}
