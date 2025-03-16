using _game.Scripts;
using UnityEngine;

public class PlayerSpray : MonoBehaviour
{
	[SerializeField] Transform spray_obj;
	
	void OnEnable()
	{
		GameManager.main.playerControlls.Player.Spray.performed += x => UseSpray();
	}
	
	void OnDisable()
	{
		GameManager.main.playerControlls.Player.Spray.performed -= x => UseSpray();
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
