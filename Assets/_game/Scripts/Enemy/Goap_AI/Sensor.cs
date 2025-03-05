using UnityEngine;
using System;
using NUnit.Framework;

[RequireComponent(typeof(SphereCollider))]
public class Sensor : MonoBehaviour
{
	[SerializeField] float detectionRadius = 5f;
	[SerializeField] float timerInterval = 1f;
	
	SphereCollider detectionRange;
	
	public event Action OnTargetChanged = delegate { };
	
	public Vector3 TargetPosition => target ? target.transform.position : Vector3.zero;
	public bool IsTargetInRange => TargetPosition != Vector3.zero;
	
	GameObject target;
	Vector3 lastKnownPositon;
	CountdownTimer timer;
	
	void Awake()
	{
		detectionRange = GetComponent<SphereCollider>();
		detectionRange.isTrigger = true;
		detectionRange.radius = detectionRadius;
	}
	
	void Start()
	{
		timer = new CountdownTimer(timerInterval);
		timer.OnTimerStop += () =>
		{
			UpdateTargetPosition(target.OrNull());
			timer.Start();
		};
		timer.Start();
	}
	
	void Update()
	{
		timer.Tick(Time.deltaTime);
	}
	
	void UpdateTargetPosition(GameObject target = null)
	{
		this.target = target;
		if(IsTargetInRange && (lastKnownPositon != TargetPosition || lastKnownPositon != Vector3.zero))
		{
			lastKnownPositon = TargetPosition;
			OnTargetChanged.Invoke();
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Player")) return; // now this only cares about player
		UpdateTargetPosition(other.gameObject);
	}
	
	void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Player")) return; // now this only cares about player
		UpdateTargetPosition();
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = IsTargetInRange ? Color.red : Color.green;
		Gizmos.DrawWireSphere(transform.position, detectionRadius);
	}
	
}
