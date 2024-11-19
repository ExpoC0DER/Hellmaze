using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	[SerializeField] float currentHealth = 100;
	[SerializeField] float maxHealth = 100;
	bool dead = false;
	
	[SerializeField] ParticleSystem blood_part;
	
	public void ChangeHealth(float amount)
	{
		currentHealth += amount;
		
		if(amount < 0)
		{
			blood_part.Play();
		}
		
		if(currentHealth <= 0)
		{
			Die();
		}else if(currentHealth >= maxHealth)
		{
			currentHealth = maxHealth;
		}
		
	}
	
	void Die()
	{
		dead = true;
	}
}
