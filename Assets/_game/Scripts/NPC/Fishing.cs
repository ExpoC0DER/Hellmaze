using UnityEngine;

public class Fishing : MonoBehaviour
{
	[SerializeField] Animator anim;
	[SerializeField] Vector2 cast_cd;
	float cd =0;
	
	void Update()
	{
		if(cd >= 0)
		{
			cd -= Time.deltaTime;
			if(cd <= 0 )
			{
				anim.SetTrigger("Cast");
				cd = Random.Range(cast_cd.x, cast_cd.y);
			}
		}
	}
	
}
