using UnityEngine;

public class RotateContinuously : MonoBehaviour
{
	[SerializeField] Vector3 Direction;
	[SerializeField] float Speed = 1;
	
	void Update()
	{
		transform.Rotate(Direction, Speed * Time.deltaTime);
	}
}
