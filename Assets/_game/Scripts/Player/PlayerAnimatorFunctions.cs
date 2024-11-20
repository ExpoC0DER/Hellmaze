using _game.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimatorFunctions : MonoBehaviour
{
	[SerializeField] GameObject[] weaponsModels;
	[SerializeField] ParticleSystem[] muzzleParticles;
	[SerializeField] Transform cameraTransform;
	int weaponSet;
	bool isPlayer =false;
	float camRot;
	Animator animator;
	[SerializeField] NavMeshAgent bot_agent;
	
	private void Awake()
	{
		animator = GetComponent<Animator>();
		isPlayer = transform.parent.CompareTag("Player");
	}
	
	public void SetWeapon(int index)
	{
		for (int i = 0; i < weaponsModels.Length; i++)
		{
			weaponsModels[i].SetActive(i == index);
			if(i == index) weaponSet = i;
		}
	}
	
	public void MuzzleParticle()
	{
		muzzleParticles[weaponSet].Play();
	}
	
	private void Update()
	{
		if(isPlayer)
		{
			float cameraAngle = cameraTransform.eulerAngles.x;
			if(cameraAngle > 90 && cameraAngle <= 360) cameraAngle -= 360;
			camRot = Mathf.InverseLerp(90, -90f, cameraAngle);
			animator.SetFloat("SpineRotation", camRot);
			//Debug.Log("camRot " +camRot);
		}else
		{
			BotMovemenetAnimation();
		}
		
	}
	
	void BotMovemenetAnimation()
	{
		Vector3 agentVel = bot_agent.velocity.normalized;
		float speedX = agentVel.z;
		float speedY = agentVel.x;
		animator.SetFloat("SpeedX", speedX);
		animator.SetFloat("SpeedY", speedY);
		//Debug.Log("velocity " +agentVel);
	}
}
