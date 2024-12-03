using Unity.VisualScripting;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
	[SerializeField] GameObject _resultScreen_object;
	[SerializeField] KeyCode key;
	
	void Start()
	{
		SwitchResults(false);
	}
	
	void Update()
	{
		if(Input.GetKeyDown(key))
		{
			SwitchResults(true);
		}
		if(Input.GetKeyUp(key))
		{
			SwitchResults(false);
		}
	}
	
	void SwitchResults(bool on)
	{
		_resultScreen_object.SetActive(on);
		if(on) RefreshResults();
	}
	
	void RefreshResults()
	{
		
	}
	
}
