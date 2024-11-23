using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
	[SerializeField] ParticleSystem particle;
	[SerializeField] GameObject visual_object;
	[SerializeField] float respawnTime = 30;
	
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
		StartCoroutine(Respawn());
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
	
	IEnumerator Respawn()
	{
		yield return new WaitForSeconds(1);
		DisableObject();
		yield return new WaitForSeconds(respawnTime);
		visual_object.SetActive(false);
		particle.Play();
		taken = false;
	}
	
	void OnEnable()
	{
		visual_object.SetActive(true);
		taken = false;
	}
}
