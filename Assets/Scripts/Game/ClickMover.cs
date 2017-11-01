/* Script that enables worm movement by dragging, mainly using linecasts.
 * Includes a SmartMove function that facilitates movement around corners to reduce the chance of getting stuck. */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ClickMover : MonoBehaviour 
{	
	[Header("Scripts & Camera")]
	public GameController gameController;
	public Camera mainCamera;

	[Header("Audio")]
	public AudioClip eatAudio;
	public AudioClip crumbleAudio;

	[Header("WormBody")]
	public GameObject bodyChunk;
	public Sprite regularWorm;
	public Sprite greenWorm;
	public static List<GameObject> body = new List<GameObject>();

	[Header("Movement")]
	public float minTouchDistance;
	public float maxTouchDistance;
	public LayerMask collisionLayer;
	public bool playerSelect;
	private Vector3 facMoveLeft, facMoveRight;
	private bool firstMove = true;

	// touch movement direction
	private int horizontal = 0;
	private int vertical = 0;
	private string direction;

	// movement planning
	private Vector3 rot;
	private Vector3 newPos;
	public Vector3 oldHeadPos = Vector3.zero;
	private bool eating = false;

	void Start()
	{
		// instantiates the starting chunks (off screen)
		for (int i = 0; i < gameController.startChunks; i++) 
		{
			body.Add(Instantiate(bodyChunk, new Vector3(-13f, i, 0f), Quaternion.identity) as GameObject);

			if (SceneLoader.gameMode == "solo")
				body[i].tag = "LiveBody";
		}
	}
		
	void Update()
	{
		// runs if the player has been selected and it isn't Game Over
		if (playerSelect == true && GameController.gameOver == false)
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
				else
				{
					vertical = y > 0 ? 1 : -1;
				}
			}
		}

		// sets the variables for movement according to the direction then calls (PlanMove)
		if (Input.GetKeyDown(KeyCode.UpArrow) || vertical == 1)
		{
			direction = "up";
			rot = new Vector3(0, 0, 0);
			newPos = Vector3.up;
			PlanMove();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) || vertical == -1)
		{
			direction = "down";
			rot = new Vector3(0, 0, 180);
			newPos = Vector3.down;
			PlanMove();
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow) || horizontal == -1)
		{
			direction = "left";
			rot = new Vector3(0, 0, 90);
			newPos = Vector3.left;
			PlanMove();
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow) || horizontal == 1)
		{
			direction = "right";
			rot = new Vector3(0, 0, -90);
			newPos = Vector3.right;
			PlanMove();
		}
	}

	// once movement has been registered, scans ahead to see if movement is possible
	public void PlanMove()
	{
		// reset movement to 0 to prevent double moves
		horizontal = 0;
		vertical = 0;

		// casts a line in the desired direction to test for obstructions
		Debug.DrawLine(transform.position, transform.position + newPos, Color.green, 0.5f);
		RaycastHit2D playerRay = Physics2D.Linecast(transform.position, transform.position + newPos, collisionLayer);

		// if there is no obstruction, move normally (as long as the target isn't the same as the previous position)
		if (playerRay == false && ((oldHeadPos != transform.position + newPos) || gameController.firstMove == true))
		{
			Move();
			gameController.firstMove = false;
		}

		// if there is an obstruction, determine what is in the way and react accordingly
		if (playerRay == true)
		{
			// if the obstruction is a wall, run (PlanSmartMove)
			if (playerRay.collider.tag == "Wall" && gameController.smartMove)
			{
				SmartMoveScan();
			}
			// if it's a level change hole, change level
			else if (playerRay.collider.tag == "LevelChange")
			{
				Move();
				gameController.ChangeLevel();
			}
			// if the obstruction is a food, deactivate the food object
			else if (playerRay.collider.tag == "Food")
			{
				playerRay.collider.gameObject.SetActive(false);
				eating = true;

				// then move normally
				Move();
			}
		}
	}
			
	// normal movement (no obstructions)
	void Move()
	{
		// saves the current position and rotation of player before moving
		oldHeadPos = transform.position;
		Vector3 headRot = transform.eulerAngles;
		Quaternion headRotQua = transform.rotation;

		// moves [wormHead] according to values defined in (Update)
		transform.eulerAngles = rot;
		transform.position += newPos;

		// snaps to whole coordinate if first move
		if (firstMove == true)
		{
			transform.position = new Vector3(-9, 0, 0);
			firstMove = false;
		}

		if (body.Count > 0)
		{
			if (eating == false)
			{
				BodyShuffle(oldHeadPos, headRot);
			}
			else if (eating == true)
			{
				GrowBody(oldHeadPos, headRotQua);
			}
		}

		gameController.CheckShadow();

	}

	// facilitates easier movement around corners to stop the player getting stuck
	void SmartMoveScan()
	{
		// saves the rotation and temporarily switches it for raycasting
		Vector3 headRot = transform.eulerAngles;
		transform.eulerAngles = rot;

		// casts a line 1 space up and left of the player
		Vector3 facScanLeft = transform.TransformDirection(Vector3.up + Vector3.left);
		Debug.DrawLine(transform.position, transform.position + facScanLeft, Color.green, 0.5f);
		RaycastHit2D scanLeft = Physics2D.Linecast(transform.position, transform.position + facScanLeft, collisionLayer);

		// casts a line 1 space up and right of the player
		Vector3 facScanRight = transform.TransformDirection(Vector3.up + Vector3.right);
		Debug.DrawLine(transform.position, transform.position + facScanRight, Color.green, 0.5f);
		RaycastHit2D scanRight = Physics2D.Linecast(transform.position, transform.position + facScanRight, collisionLayer);

		// casts a line 1 space left of the player
		facMoveLeft = transform.TransformDirection(Vector3.left);
		Debug.DrawLine(transform.position, transform.position + facMoveLeft, Color.green, 0.5f);
		RaycastHit2D moveLeft = Physics2D.Linecast(transform.position, transform.position + facMoveLeft, collisionLayer);

		// casts a line 1 space right of the player
		facMoveRight = transform.TransformDirection(Vector3.right);
		Debug.DrawLine(transform.position, transform.position + facMoveRight, Color.green, 0.5f);
		RaycastHit2D moveRight = Physics2D.Linecast(transform.position, transform.position + facMoveRight, collisionLayer);

		// returns the rotation to its previous state
		transform.eulerAngles = headRot;

		// runs if space is found to the left of the wall (or if it's occupied by anything other than a wall)
		if ((scanLeft == false || scanLeft.collider.gameObject.layer !=9) && (moveLeft == false || moveLeft.collider.gameObject.layer != 9) 
			&& ((oldHeadPos != transform.position + facMoveLeft && oldHeadPos != transform.position + facScanLeft) || gameController.firstMove == true))
		{
			direction = "left";
			PlanSmartMove(scanLeft, moveLeft, facMoveLeft);
			gameController.firstMove = false;
		}
		// same to the right of the wall
		else if ((scanRight == false || scanRight.collider.gameObject.layer !=9) && (moveRight == false || moveRight.collider.gameObject.layer != 9)
			&& ((oldHeadPos != transform.position + facMoveRight && oldHeadPos != transform.position + facScanRight) || gameController.firstMove == true))
		{
			direction = "right";
			PlanSmartMove(scanRight, moveRight, facMoveRight);
			gameController.firstMove = false;
		}
	}

	// determines how to react if there is an obstruction
	void PlanSmartMove(RaycastHit2D scan, RaycastHit2D move, Vector3 facMove)
	{
		if (move == true)
		{
			// if that obstruction is food
			if (move.collider.tag == "Food")
			{
				move.collider.gameObject.SetActive(false);
				eating = true;
				SmartMove();
			}
			// if that obstruction is a level change hole
			else if (move.collider.tag == "LevelChange")
			{
				SmartMove();
				gameController.ChangeLevel();
			}
		}
		else
		{
			SmartMove();
		}
	}

	// takes the values from (PlanSmartMove) and moves accordingly
	void SmartMove()
	{
		// saves current position and rotation of [wormHead]
		oldHeadPos = transform.position;
		Vector3 headRot = transform.eulerAngles;
		Quaternion headRotQua = transform.rotation;
		
		// moves [wormHead] according to values defined in (Update)
		if (direction == "left") 
		{
			transform.position += facMoveLeft;
		}
		else if (direction == "right") 
		{
			transform.position += facMoveRight;
		}

		// keeps the worm's body in step (if it exists)
		if (body.Count > 0 && eating == false) 
		{
			BodyShuffle(oldHeadPos, headRot);
		}
		else if (eating == true)
		{
			GrowBody(oldHeadPos, headRotQua);
		}

		gameController.CheckShadow();
	}

	// keeps the worm's body in step with [wormHead]
	void BodyShuffle(Vector3 headPos, Vector3 headRot)
	{
		// moves the last body chunk in the list to where the head was
		body.Last().transform.position = headPos;
		body.Last().transform.eulerAngles = Vector3.Lerp(headRot, transform.eulerAngles, 0.5f);

		// moves this chunk from the bottom of the list to the top
		body.Insert(0, body.Last());
		body.RemoveAt(body.Count - 1);

		// shuffles body sprites normally if playing hunt / flood mode
		if (SceneLoader.gameMode != "solo")
		{
			int gameLevel = gameController.level;

			// colours the live body parts, and adjusts sorting order and tag
			for (int i = 0; i < gameLevel; i++) 
			{
				SpriteRenderer bodySprite = body[i].GetComponentInChildren<SpriteRenderer>();
				bodySprite.sortingOrder = 1;
				body[i].tag = "LiveBody";
				bodySprite.sprite = regularWorm;
			}

			// colours the (dead) body parts
			if (body.Count > gameLevel) 
			{
				for (int i = 0; i < body.Count - gameLevel; i++) 
				{
					SpriteRenderer bodySprite = body[i + gameLevel].GetComponentInChildren<SpriteRenderer>();
					bodySprite.sortingOrder = 0;
					body[i + gameLevel].tag = "Body";
					bodySprite.sprite = greenWorm;
				}
			}
		}
//		else if (SceneLoader.gameMode == "solo")
//		{
//			for (int i = 0; i < body.Count; i++)
//				body[i].tag = "LiveBody";
//		}
	}

	// interacts with food when encountered
	void GrowBody(Vector3 headPos, Quaternion headRotQua)
	{
		// updating variables and playing audio
		eating = false;

		if (gameController.foodCount > 1)
			SoundPlayer.instance.PlaySingle2(eatAudio);
		else
			SoundPlayer.instance.PlaySingle2(crumbleAudio);

		gameController.foodCount--;
		gameController.MakeHole();

		// instantiates new [bodyChunk] where the head was, and adds it to the top of the list
		GameObject newChunk = (GameObject)Instantiate(bodyChunk, headPos, headRotQua);
		body.Insert(0, newChunk);

		if (SceneLoader.gameMode == "solo")
			newChunk.tag = "LiveBody";

		newChunk.GetComponentInChildren<SpriteRenderer>().sprite = regularWorm;

		// updates the score, including the new body chunk (this MUST go here before the sprite shuffle!)
		gameController.UpdateScore();

		// shuffles the different body sprites correctly
		if (SceneLoader.gameMode != "solo")
		{
			for (int i = 0; i < body.Count - gameController.level; i++) 
			{
				body[i + gameController.level].GetComponentInChildren<SpriteRenderer>().sprite = greenWorm;
			}
		}
	}

	// resets body sprites (for use after a powerUp fades)
//	public void ResetBodySprites()
//	{
//		for (int i = 0; i < gameController.level; i++) 
//		{
//			body[i].GetComponentInChildren<SpriteRenderer>().sprite = regularWorm;
//		}
//
//		// colours the non-essential (dead) body parts
//		for (int i = 0; i < ClickMover.body.Count - gameController.level; i++)
//		{
//			body[i + gameController.level].GetComponentInChildren<SpriteRenderer>().sprite = greenWorm;
//		}
//	}
}