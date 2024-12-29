using System.Collections.Generic;
using _game.Scripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimatorFunctions : MonoBehaviour
{
	[SerializeField] int character_selected = 0;
	[SerializeField] GameObject[] characters;
	[SerializeField] Avatar[] character_avatars;
	[SerializeField] GameObject[] weaponsModels;
	[SerializeField] List<TransformArray> weaponPositions = new List<TransformArray>();
	[SerializeField] Transform weaponBaseParent;
	[SerializeField] CinemachineCamera cinemachineCamera;
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
		if(!isPlayer) character_selected = Random.Range(0, characters.Length);
		SetAvatar(character_selected);
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
	
	public void SetDead()
	{
		animator.SetFloat("DieAnimIndex", Random.Range(0, 2));
		animator.SetBool("IsDead", true);
		animator.SetLayerWeight(3, 1);
	}
	
	public void SetAlive()
	{
		animator.SetBool("IsDead", false);
		animator.SetLayerWeight(3, 0);
	}
	
	public void SetAvatar(int index)
	{
		animator.enabled = false;
		/* if(isPlayer)
		{
			cinemachineCamera.Follow = characters[index].transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head");
		} */
		for (int i = 0; i < characters.Length; i++)
		{
			characters[i].SetActive(i == index);
		}
		
		for (int i = 0; i < weaponsModels.Length; i++)
		{
			weaponsModels[i].transform.SetParent(weaponPositions[index].Transforms[i] ,false);
		}
		
		animator.avatar = character_avatars[index];
		animator.enabled = true;
	}
	
}

[System.Serializable]
public class TransformArray
{
	public Transform[] Transforms;
}


