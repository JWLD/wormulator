using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodSetup : MonoBehaviour 
{
	public int spacing;
	public List<GameObject> foodObjects = new List<GameObject>();
	public GameObject foodPrefab;
	public GameObject foodContainer;

	private List<Vector3> freePositions = new List<Vector3>();
	private int count = 0;

	public HazardSetup hazardScript;
	public GameController gameControllerScript;

	void Start() 
	{
		// creates Vector array of spaces
		count = 0;

		for (int i = -8; i < 11; i += 2)
		{
			for (int j = -7; j < 8; j += 2)
			{				
				freePositions.Add(new Vector3(i, j, 0));
				count++;
			}		
		}

		// deletes corners to prevent double moves into the wall when changing level
		freePositions.Remove(new Vector3(-8, 7, 0));
		freePositions.Remove(new Vector3(-8, -7, 0));
		freePositions.Remove(new Vector3(10, 7, 0));
		freePositions.Remove(new Vector3(10, -7, 0));

		// deletes first three moves out of the starting gate
		freePositions.Remove(new Vector3(-8, 0, 0));
		freePositions.Remove(new Vector3(-8, 1, 0));
		freePositions.Remove(new Vector3(-8, -1, 0));

		ArrangeFood(5);
	}

	// randomly positions food objects
	public void ArrangeFood(int foodCount)
	{
		// if playing flood mode, adds a new piece of food to the game each level (but not the first)
		if (SceneLoader.gameMode == "flood" && foodCount > 5)
		{
			// instantiates a new food object, sets its parent, then adds it to the list of food objects
			GameObject instance = Instantiate(foodPrefab, new Vector3(-14f, 0f, 0f), Quaternion.identity) as GameObject;
			instance.transform.parent = foodContainer.transform;
			foodObjects.Add(instance);
		}

		// variable setup for checking positions against each other
		Vector3[] prevFoodPos = new Vector3[foodCount];
		Vector3 newFoodPos = Vector3.zero;
		bool newPosOkay = true;

		// positions each piece of food one by one
		for (int i = 0; i < foodObjects.Count; i++)	
		{
			// reactivates the food gameObject
			foodObjects[i].SetActive(true);

			int loopRuns = 0;

			do
			{
				// gives them a random position
				newFoodPos = freePositions[Random.Range(0, freePositions.Count)];

				// checks this new position against other positions
				for (int j = 0; j < i; j++)
				{
					// if any of the checks show distance is less than spacing allows
					if (Vector3.Distance(newFoodPos, prevFoodPos[j]) < spacing)
					{
						newPosOkay = false;
						break;
					}
					// else if we've reached the last check and all is still okay
					else if (j == i - 1)
					{
						newPosOkay = true;
					}
				}

				// reduces spacing requirements if a valid position isn't found after 10 tries
				loopRuns++;

				if (loopRuns > 10)
				{
					spacing--;
				}
			}
			while (newPosOkay == false);

			prevFoodPos[i] = newFoodPos;
			foodObjects[i].transform.position = newFoodPos;
		}

		// arranges hazards after food if playing solo mode
		if (SceneLoader.gameMode == "solo" || gameControllerScript.hazardsActive == true)
			hazardScript.ArrangeHazards(prevFoodPos, freePositions);
	}
}