using UnityEngine;

public class PlayParticleOnEnable : MonoBehaviour
{
	private ParticleSystem _particleSystem;
	
	private void Awake()
	{
		_particleSystem = GetComponent<ParticleSystem>();
	}
	
	private void OnEnable()
	{
		_particleSystem.Play();
	}
}
