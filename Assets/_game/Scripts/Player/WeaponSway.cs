using _game.Scripts;
using _game.Scripts.System;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
	[Header("Sway Settings")]
	[SerializeField] private float smooth;
	[SerializeField] private float swayMultiplier;

	Vector2 inputLook;
	
	public bool isOn = true;

	void Update()
	{
		float mouseX = 0;
		float mouseY = 0;
		 
		if(isOn)
		{
			inputLook = GameManager.Instance.playerControlls.Player.Look.ReadValue<Vector2>();
			mouseX = inputLook.x * swayMultiplier;
			mouseY = inputLook.y * swayMultiplier;
		}
		
		Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
		Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
		Quaternion targetRotation = rotationX * rotationY;

		transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
	}
}
