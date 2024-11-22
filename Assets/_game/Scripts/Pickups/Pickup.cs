using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
	[SerializeField] ParticleSystem particle;
	[SerializeField] GameObject visual_object;
	
	bool taken = false;
	
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
		visual_object.SetActive(false);
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
		visual_object.SetActive(true);
		taken = false;
	}
}
