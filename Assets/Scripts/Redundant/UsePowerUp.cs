//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//
//public class UsePowerUp : MonoBehaviour 
//{
//	[Header("Scripts")]
//	public ClickMover clickMover;
//	public GameController gameController;
//
//	[Header("PowerUp UI")]
//	public GameObject shieldImage;
//	public GameObject reverseImage;
//	public Text countdown;
//	public GameObject textSurround;
//	private string whichPowerUp = null;
//
//	[Header("Shield PowerUp")]
//	public bool shieldActive = false;
//	public float shieldDuration;
//	public Sprite ironWormReg;
//	public Sprite ironWormGreen;
//
//	[Header("Reverse PowerUp")]
//	public bool reverseActive = false;
//	public float reverseDuration;
//	public float rotateSpeed;
//
//	[Header("Audio")]
//	public AudioClip getPowerUp;
//	public AudioClip shieldSound;
//	public AudioClip reverseSound;
//	public AudioClip tick;
//	public AudioClip alert;
//
//	[Header("Other")]
//	public GameObject player;
//	public GameObject debris;
//
//	// keyboard input for computer testing
//	void Update()
//	{
//		if (gameController.gameOver == false && Input.GetKeyDown(KeyCode.Q))
//		{
//			ActivatePowerUp();
//		}
//	}
//
//	// called when the player presses the powerup button
//	public void ActivatePowerUp()
//	{
//		if (whichPowerUp == "shield" && shieldActive == false && reverseActive == false)
//		{
//			StartCoroutine(ActivateShieldCR());
//		}
//		else if (whichPowerUp == "reverse" && shieldActive == false && reverseActive == false)
//		{
//			StartCoroutine(ActivateReverseCR());
//		}
//	}
//
//	// runs if the current powerup is Shield
//	IEnumerator ActivateShieldCR()
//	{
//		shieldActive = true;
//		SoundManager.instance.PlaySingle(shieldSound);
//
//		// changes body sprites to armoured worm
//		for (int i = 0; i < gameController.level; i++)
//		{
//			ClickMover.body[i].GetComponentInChildren<SpriteRenderer>().sprite = ironWormReg;
//		}
//
//		for (int i = 0; i < ClickMover.body.Count - gameController.level; i++)
//		{
//			ClickMover.body[i + gameController.level].GetComponentInChildren<SpriteRenderer>().sprite = ironWormGreen;
//		}
//
//		// deactivate the powerUp after a set duration
//		for (int i = 0; i < shieldDuration; i++)
//		{
//			// updates the shield counter every second
//			countdown.text = "" +(shieldDuration - i);
//			yield return new WaitForSeconds(1);
//
//			// plays a tick on every second apart from the last
//			if (i + 1 < shieldDuration)
//			{
//				SoundManager.instance.PlaySingle(tick);
//			}
//			// plays an alert on the last second, then resets the UI counter
//			else
//			{
//				SoundManager.instance.PlaySingle(alert);
//				shieldImage.SetActive(false);
//				countdown.text = "";
//			}
//		}
//		shieldActive = false;
//
//		gameController.scoreTextObject.SetActive(true);
//
//		if (gameController.gameOver != true)
//		{
//			CheckForOverlap();
//		}
//
//		// changes the body sprites back
//		clickMover.ResetBodySprites();
//	}
//
//	// runs if the current powerup is Reverse
//	IEnumerator ActivateReverseCR()
//	{
//		reverseActive = true;
//		SoundManager.instance.PlaySingle(reverseSound);
//
//		// starts rotation (clockwise for even, anti-clockwise for odd)
//		for (int i = 0; i < ClickMover.body.Count; i++)
//		{
//			ClickMover.body[i].GetComponent<Animator>().SetBool("rotate", true);
//		}
//
//		// deactivate the powerUp after a set duration
//		for (int i = 0; i < reverseDuration; i++)
//		{
//			// updates the shield counter every second
//			countdown.text = "" +(reverseDuration - i);
//			yield return new WaitForSeconds(1);
//
//			// plays a tick on every second apart from the last
//			if (i + 1 < reverseDuration)
//			{
//				SoundManager.instance.PlaySingle(tick);
//			}
//			// plays an alert on the last second, then resets the UI counter
//			else
//			{
//				SoundManager.instance.PlaySingle(alert);
//				reverseImage.SetActive(false);
//				countdown.text = "";
//			}
//		}
//		reverseActive = false;
//
//		gameController.scoreTextObject.SetActive(true);
//
//		if (gameController.gameOver != true)
//		{
//			CheckForOverlap();
//		}
//
//		// stops rotation
//		for (int i = 0; i < ClickMover.body.Count; i++)
//		{
//			// sets every other piece to idle in the opposite direction
//			if (i % 2 == 0)
//			{
//				ClickMover.body[i].GetComponent<Animator>().SetBool("rotate", false);
//				ClickMover.body[i].GetComponent<Animator>().SetBool("idleMirror", true);
//			}
//			else
//			{
//				ClickMover.body[i].GetComponent<Animator>().SetBool("rotate", false);
//				ClickMover.body[i].GetComponent<Animator>().SetBool("idleMirror", false);
//			}
//		}
//	}
//
//	// checks for pieces under the wormHead when a powerup wears off
//	void CheckForOverlap()
//	{
//		Vector3 currentHeadPos = player.transform.position;
//
//		for (int i = 0; i < ClickMover.body.Count; i++)
//		{
//			if (ClickMover.body[i].transform.position == currentHeadPos)
//			{
//				ClickMover.body[i].GetComponent<EatBody>().EatBodyChunk();
//			}
//		}
//	}
//
//	// stores a powerup when it's picked up in the game
//	public void StorePowerUp(GameObject powerUp)
//	{
//		gameController.scoreTextObject.SetActive(false);
//
//		// deactivates the powerUp and plays the audio clip
//		powerUp.SetActive(false);
//		SoundManager.instance.PlaySingle(getPowerUp);
//
//		// increases the appropriate UI counter
//		if (powerUp.CompareTag("ShieldPU"))
//		{
//			reverseImage.SetActive(false);
//			shieldImage.SetActive(true);
//			whichPowerUp = "shield";
//		}
//		else if (powerUp.CompareTag("ReversePU"))
//		{
//			shieldImage.SetActive(false);
//			reverseImage.SetActive(true);
//			whichPowerUp = "reverse";
//		}
//	}
//}
