using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenuMover : MonoBehaviour 
{	
	[Header("Scripts & Camera")]
	public Camera mainCamera;
	public SceneLoader sceneLoader;
	public MedalController medalController;
	public GoldWorm goldWorm;

	[Header("WormBody")]
	public List<GameObject> body = new List<GameObject>();
	public GameObject bodyChunk;
	public int startChunks;
	public int headOffset;
	public GameObject arrows;

	[Header("Movement")]
	public LayerMask collisionLayer;
	public bool playerSelect;
	public float minTouchDistance;
	public float maxTouchDistance;
	private int horizontal = 0;
	private bool oneTime = false;

	void Start()
	{
		// instantiates the starting chunks (off screen)
		for (int i = 0; i < startChunks; i++)
			body.Add(Instantiate(bodyChunk, transform.position - new Vector3(headOffset, 0, 0), Quaternion.Euler(0, 0, 270)) as GameObject);

		for (int i = 0; i < body.Count; i++)
			goldWorm.menuBodies.Add(body[i]);
	}

	void Update()
	{
		// runs if the player has been selected
		if (playerSelect == true)
		{
			// converts mouse / touch position to world coordinates
			Vector3 inputPos = Input.mousePosition;
			inputPos.z = 10f;
			Vector3 worldPos = mainCamera.ScreenToWorldPoint(inputPos);

			// keeps track of the distance between the wormHead and player's touch
			float touchDistance = Vector3.Distance(transform.position, worldPos);

			// runs once the player moves their finger, and while they stay within range
			if (touchDistance > minTouchDistance && touchDistance < maxTouchDistance)
			{
				// determines the direction of movement (one of four options)
				float x = worldPos.x - transform.position.x;
				float y = worldPos.y - transform.position.y;

				if (Mathf.Abs(x) > Mathf.Abs(y))
				{
					horizontal = x > 0 ? 1 : -1;
				}
			}
		}

		// moves the worm to the right when necessary
		if (horizontal == 1)
		{
			if (oneTime == false)
			{
				arrows.SetActive(false);
				oneTime = true;
			}

			// resets "horizontal" to prevent double moves
			horizontal = 0;

			// saves head position and rotation before moving
			Vector3 headPos = transform.position;
			Vector3 headRot = transform.eulerAngles;

			// moves the worm one position to the right
			transform.position += Vector3.right;

			// keeps body in tow behind head
			BodyShuffle(headPos, headRot);

			// calls LoadGameMode to check the new player's coordinates
			sceneLoader.LoadGameMode();
		}
	}

	// keeps the worm's body in step with the wormHead
	void BodyShuffle(Vector3 headPos, Vector3 headRot)
	{
		// moves the last body chunk in the list to where the head was
		body.Last().transform.position = headPos;
		body.Last().transform.eulerAngles = Vector3.Lerp(headRot, transform.eulerAngles, 0.5f);

		// moves this chunk from the bottom of the list to the top
		body.Insert(0, body.Last());
		body.RemoveAt(body.Count - 1);
	}
}