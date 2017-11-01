/* Controls the maze UI - adds a 'gate' barrier once the player moves from the starting position.
 * Adds a level change hole when all food is eaten and sets up the board for each new level. */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour 
{
	[Header("Maze Setup")]
	public GameObject[] mazeLayouts;

	[Header("Floors")]
	public SpriteRenderer mazeFloor;
	public Sprite[] floorImages;

	[Header("Level Change")]
	public GameObject levelChangeHole;

	[Header("Starting Gate")]
	public Transform playerTransform;
	public GameObject gateCollider;
	private bool gateOpen = true;

	[Header("Flood Objects")]
	public GameObject startHole;
	public GameObject scoreTextObject;
	public GameObject airPuff;

	public List<Vector3> holePositions;
	public List<Vector3> noGoodCorners;

	void Update()
	{
		// runs while the gatePiece is still open
		if (gateOpen == true)
		{
			// closes the gate once the player has moved out
			if (playerTransform.position == new Vector3(-8f, 1f, 0f) || playerTransform.position == new Vector3(-8f, -1f, 0f))
			{
				CloseStartingGate();
				gateOpen = false;
			}
		}
	}

	// blocks off the starting hole once the worm has moved out, and activates a UI element
	public void CloseStartingGate()
	{
		// closes gate
		gateCollider.SetActive(true);

		// if playing flood, activates the air puff and hides the starting hole
		if (SceneLoader.gameMode == "flood")
		{
			startHole.SetActive(false);
			airPuff.SetActive(true);
		}
		// if not playing flood, activates the score text instead
		else
		{
			scoreTextObject.SetActive(true);
		}
	}

	// called when all the food has been cleared
	public void LevelComplete()
	{
		// randomly chooses a corner to move the level-change hole to, then activates it
		holePositions = new List<Vector3>() {new Vector3(-8f, 7f, 0f), new Vector3(10f, 7f, 0f), new Vector3(10f, -7f, 0f), new Vector3(-8f, -7f, 0f)};

		for (int i = 0; i < ClickMover.body.Count; i++)
		{
			for (int j = 0; j < 4; j++)
				if (ClickMover.body[i].transform.position == holePositions[j])
					noGoodCorners.Add(holePositions[j]);
		}
			
		if (noGoodCorners.Count > 0 && noGoodCorners.Count < 4)
			for (int i = 0; i < noGoodCorners.Count; i++)
				holePositions.Remove(noGoodCorners[i]);

		levelChangeHole.transform.position = holePositions[Random.Range(0, holePositions.Count)];
		levelChangeHole.SetActive(true);
	}

	// called when the player changes level
	public void ChangeToNextMaze(int nextLevel)
	{
		// saved for easier calculations
		int prevLevel = nextLevel - 1;

		// deactivates level-change hole now that it's been used
		levelChangeHole.SetActive(false);
		noGoodCorners.Clear();

		// deactivates the previous maze and activates the next
		mazeLayouts[(prevLevel - 1) % mazeLayouts.Length].SetActive(false);
		mazeLayouts[(nextLevel - 1) % mazeLayouts.Length].SetActive(true);

		// vertically flip floor sprite every level
		mazeFloor.gameObject.transform.eulerAngles += new Vector3(180f, 0f, 0f);

		// change floor sprite every 3 levels
		if (prevLevel % 3 == 0)
		{
			if ((prevLevel / 3) % 3 == 0)
				mazeFloor.sprite = floorImages[0];
			else if ((prevLevel / 3) % 3 == 1)
				mazeFloor.sprite = floorImages[1];
			else if ((prevLevel / 3) % 3 == 2)
				mazeFloor.sprite = floorImages[2];
		}
	}
}