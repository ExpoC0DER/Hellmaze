using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Pickup : MonoBehaviour
{
	[SerializeField] ParticleSystem particle;
	
	MeshRenderer rend;
	bool taken = false;
	
	void Awake()
	{
		rend = GetComponent<MeshRenderer>();
	}
	
	public virtual void OnPickupPlayer(GameObject gameObject)
	{
		return;
	}
	
	public virtual void OnPickupBot(GameObject gameObject)
	{
		return;
	}
	
	void OnPickup()
	{
		rend.enabled = false;
		particle.Play();
		taken = true;
		Invoke("DisableObject", 1f);
	}
	
	void DisableObject()
	{
		gameObject.SetActive(false);		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			OnPickupPlayer(other.gameObject);
			OnPickup();
		}else if(other.CompareTag("Bot"))
		{
			OnPickupBot(other.gameObject);
			OnPickup();
		}
	}
	
	void OnEnable()
	{
		rend.enabled = true;
		taken = false;
	}
}
