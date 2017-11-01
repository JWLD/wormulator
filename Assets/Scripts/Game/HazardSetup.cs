using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HazardSetup : MonoBehaviour 
{
	// variables for laying out hazards
	public int hazardSpacing;
	public int hazardFoodSpacing;
	public int hazardFoodAttempts;
	public GameObject[] hazards;

	public void ArrangeHazards(Vector3[] foodPositions, List<Vector3> freePositions)
	{
		// saves starting spacing values
		int hazardHazardStart = hazardSpacing;
		int hazardFoodStart = hazardFoodSpacing;

		// variable setup for checking positions against each other
		Vector3[] prevObjPos = new Vector3[3];
		Vector3 newObjPos = Vector3.zero;
		bool newPosOkay = true;

		// positions each hazard one by one
		for (int i = 0; i < hazards.Length; i++)	
		{
			// reactivates the gameObject
			hazards[i].SetActive(true);

			int loopRuns_1 = 0;
			int loopRuns_2 = 0;

			do
			{
				bool hazardFoodFail = false;
				bool hazardHazardFail = false;

				// gives the hazard a random position
				newObjPos = freePositions[Random.Range(0, freePositions.Count)];

				// checks this positions against food positions
				for (int j = 0; j < 5; j++)
				{
					// if too close to a food object, break and try a new position
					if (Vector3.Distance(newObjPos, foodPositions[j]) < hazardFoodSpacing)
					{
						newPosOkay = false;
						hazardFoodFail = true;
						loopRuns_1++;
						loopRuns_2 = 0;
						break;
					}
					// else if far enough from all food objects
					else if (j == 4)
					{
						newPosOkay = true;

						// checks this against other hazards (won't run for the first object since there's nothing to compare to)
						for (int k = 0; k < i; k++)
						{
							// if too close to another hazard, break and try new position
							if (Vector3.Distance(newObjPos, prevObjPos[k]) < hazardSpacing)
							{
								newPosOkay = false;
								hazardHazardFail = true;
								loopRuns_1 = 0;
								loopRuns_2++;
								break;
							}
							// else if all distance checks have been passed
							else if (k == i - 1)
							{
								newPosOkay = true;
							}
						}
					}
				}

				// reduces spacing requirements after 10 tries (prevents crashes)
				if (loopRuns_1 > hazardFoodAttempts && hazardFoodFail == true)
				{
					hazardFoodSpacing--;
					hazardFoodFail = false;
				}

				if (loopRuns_2 > 10 && hazardHazardFail == true)
				{
					hazardSpacing--;
					hazardHazardFail = false;
				}
			}
			while (newPosOkay == false);

			prevObjPos[i] = newObjPos;
			hazards[i].transform.position = newObjPos;
			hazards[i].GetComponent<SpriteRenderer>().enabled = true;

			// reset spacing for next hazard
			if (i < 2)
			{
				hazardSpacing = hazardHazardStart;
				hazardFoodSpacing = hazardFoodStart;
			}
		}
	}
}