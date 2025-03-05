using UnityEngine;

public class StickybombLaser : MonoBehaviour
{
	[SerializeField] LineRenderer lr;
	[SerializeField] BoxCollider col;
	[SerializeField] StickyBomb stickyBomb;
	bool _triggered = false;
	
	void OnEnable()
	{
		float distance = 25;
		if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 25))
		{
			distance = Vector3.Distance(transform.position, hit.point);
		}
		Vector3[] points = {transform.position, transform.position + transform.forward * distance};
		lr.SetPositions(points);
		col.size = new Vector3(0.1f, 0.1f, distance);
		col.center = new Vector3(0, 0, distance * 0.5f - 0.05f);
		_triggered = false;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if((other.CompareTag("Player") || other.CompareTag("Bot")) && !_triggered)
		{
			other.TryGetComponent(out PlayerStats player);
			stickyBomb.LaserTrigger(player);
			_triggered = true;
		}
	}
}
