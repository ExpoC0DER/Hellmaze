using System.Collections;
using UnityEngine;
using AYellowpaper;

public class SolidMapObject : MonoBehaviour
{
	public bool CanRespawn { get; set; } = true;
	[SerializeField] MeshRenderer[] dissolve_rendereres;
	[SerializeField] Collider[] colliders;
	[SerializeField] InterfaceReference<IDestructable, MonoBehaviour>[] _destructable;
	
	
	public void Respawn(Vector3 position, Quaternion rotation)
	{
		StartCoroutine(RespawnCor(position,rotation));
	}
	
	IEnumerator RespawnCor(Vector3 position, Quaternion rotation)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		
		float progress = 0;
		while(progress <= 1)
		{
			for (int i = 0; i < dissolve_rendereres.Length; i++)
			{
				MeshRenderer rend = dissolve_rendereres[i];
				if(rend.enabled) dissolve_rendereres[i].material.SetFloat(Shader.PropertyToID("_Dissolve"), progress);
			}
			progress += Time.deltaTime;
			yield return null;
		}
		
		if(_destructable.Length > 0)
		{
			for (int i = 0; i < _destructable.Length; i++)
			{
				_destructable[i].Value.Respawn();
			}
		}
		
		transform.position = position;
		transform.rotation = rotation;
		
		progress = 1;
		while(progress >= 0)
		{
			for (int i = 0; i < dissolve_rendereres.Length; i++)
			{
				dissolve_rendereres[i].material.SetFloat(Shader.PropertyToID("_Dissolve"), progress);
			}
			progress -= Time.deltaTime;
			yield return null;
		}
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = true;
		}
		
	}
}
