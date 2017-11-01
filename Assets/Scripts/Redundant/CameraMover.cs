using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float cameraShift;
	public float smoothTime;
	private float targetPosX;
	private float velocity;

	[HideInInspector]
	public bool changeLevel = false;

	public void UpdateTargetPos()
	{
		targetPosX = transform.position.x + cameraShift;
	}

	void Update()
	{
		if (changeLevel == true && targetPosX != transform.position.x) 
		{
			float cameraPosX = Mathf.SmoothDamp(transform.position.x, targetPosX, ref velocity, smoothTime);

			transform.position = new Vector3(cameraPosX, transform.position.y, transform.position.z);
		}
	}
}