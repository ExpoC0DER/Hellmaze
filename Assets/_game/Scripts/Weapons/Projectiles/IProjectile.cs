using UnityEngine;

public interface IProjectile
{
	public float Damage { get; set; }
	public Transform Source { get; set; }
	public string PoolName { get; set; }
	public void Initialize(Transform source, float damage, string poolName);
}
