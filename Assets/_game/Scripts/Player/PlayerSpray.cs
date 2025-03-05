using UnityEngine;

public class PlayerSpray : MonoBehaviour
{
	[SerializeField] Transform spray_obj;
	[SerializeField] KeyCode key = KeyCode.T;
	
	void Update()
	{
		if(Input.GetKeyDown(key))
		{
			UseSpray();
		}
	}
	
	void UseSpray()
	{
		if(Physics.Raycast(Camera.main!.transform.position, Camera.main!.transform.forward, out RaycastHit hit, 2, Physics.AllLayers, QueryTriggerInteraction.Ignore))
		{
			if(!spray_obj.gameObject.activeSelf) spray_obj.gameObject.SetActive(true);
			
			spray_obj.transform.position = hit.point;
			spray_obj.transform.rotation = Quaternion.LookRotation(hit.normal);
		}
	}
}
