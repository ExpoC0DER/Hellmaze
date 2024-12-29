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
			mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
			mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;
		}
		
		Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
		Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
		Quaternion targetRotation = rotationX * rotationY;

		transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
	}
}
