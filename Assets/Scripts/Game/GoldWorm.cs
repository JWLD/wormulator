using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GoldWorm : MonoBehaviour
{
	[Header("Scripts")]
	public LocalScores localScores;
	public ClickMover clickMover;
	public FloodControl floodControl;
	public GameController gameController;
	public MainMenuMover mainMenuMover;

	[Header("Objects")]
	public SpriteRenderer playerSprite;
	public SpriteRenderer[] menuHeads;
	public List<GameObject> menuBodies = new List<GameObject>();
	public bool goldWorm = false;

	[Header("Sprites")]
	public Sprite regHead;
	public Sprite goldHead;

	public Sprite regBody;
	public Sprite goldBody;
	public Sprite regGreenBody;
	public Sprite goldGreenBody;

	public Sprite drownHeadReg;
	public Sprite drownBodyReg;
	public Sprite drownBodyGreenReg;

	public Sprite drownHeadGold;
	public Sprite drownBodyGold;
	public Sprite drownBodyGreenGold;

	[Header("UI Components")]
	public GameObject goldWormButton;
	public GameObject goldCross;

	void Start()
	{
		if (localScores.huntScores[0].score >= MedalController.huntGold && 
			localScores.soloScores[0].score >= MedalController.soloGold && 
			localScores.floodScores[0].score >= MedalController.floodGold)
		{
			goldWormButton.SetActive(true);

			if (PlayerPrefs.GetInt("GoldWormActive") == 1)
				SwitchWormSprites();
		}
	}

	public void SwitchWormSprites()
	{
		if (goldWorm == false)
		{
			if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				for (int i = 0; i < 3; i++)
					menuHeads[i].GetComponentInChildren<SpriteRenderer>().sprite = goldHead;

				for (int i = 0; i < menuBodies.Count; i++)
					menuBodies[i].GetComponentInChildren<SpriteRenderer>().sprite = goldBody;
			}
			else if (SceneManager.GetActiveScene().buildIndex == 1)
			{
				// change sprites on scripts
				playerSprite.sprite = goldHead;
				clickMover.regularWorm = goldBody;
				clickMover.greenWorm = goldGreenBody;

				floodControl.drownedWorm = drownHeadGold;
				floodControl.drownedBodyReg = drownBodyGold;
				floodControl.drownedBodyGreen = drownBodyGreenGold;

				// change sprites already in-game
				for (int i = 0; i < gameController.level; i++)
					ClickMover.body[i].GetComponentInChildren<SpriteRenderer>().sprite = goldBody;

				for (int i = 0; i < ClickMover.body.Count - gameController.level; i++)
					ClickMover.body[i + gameController.level].GetComponentInChildren<SpriteRenderer>().sprite = goldGreenBody;
			}

			goldWorm = true;
			goldCross.SetActive(false);
			PlayerPrefs.SetInt("GoldWormActive", 1);
		}
		else if (goldWorm == true)
		{
			if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				for (int i = 0; i < 3; i++)
					menuHeads[i].GetComponentInChildren<SpriteRenderer>().sprite = regHead;

				for (int i = 0; i < menuBodies.Count; i++)
					menuBodies[i].GetComponentInChildren<SpriteRenderer>().sprite = regBody;
			}
			else if (SceneManager.GetActiveScene().buildIndex == 1)
			{
				// change sprites on scripts
				playerSprite.sprite = regHead;
				clickMover.regularWorm = regBody;
				clickMover.greenWorm = regGreenBody;

				floodControl.drownedWorm = drownHeadReg;
				floodControl.drownedBodyReg = drownBodyReg;
				floodControl.drownedBodyGreen = drownBodyGreenReg;

				// change sprites already in-game
				for (int i = 0; i < gameController.level; i++)
					ClickMover.body[i].GetComponentInChildren<SpriteRenderer>().sprite = regBody;

				for (int i = 0; i < ClickMover.body.Count - gameController.level; i++)
					ClickMover.body[i + gameController.level].GetComponentInChildren<SpriteRenderer>().sprite = regGreenBody;
			}
			
			goldWorm = false;
			goldCross.SetActive(true);
			PlayerPrefs.SetInt("GoldWormActive", 0);
		}
	}
}
