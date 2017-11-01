using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
	public int rotateSpeed;

	void Start()
	{
		// starts the food off at a random rotation
		transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
	}

	void Update() 
	{
		// constantly rotates the food clockwise
		transform.Rotate (0, 0, -rotateSpeed * Time.deltaTime);
	}
}
