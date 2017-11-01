using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeadCollision : MonoBehaviour 
{
	[Header("Scripts")]
	public GameController gameController;
	public GoldWorm goldWorm;

	[Header("DeathFX")]
	public GameObject splatReg;
	public GameObject splatGold;

//	[Header("BodySprites")]
//	public Sprite greenWorm;
//	public Sprite pinkWorm;

	[Header("Audio")]
	public AudioClip beepAudio;
	public AudioClip bloodSplat;
	public AudioClip bloodShatter;

	[Header("Bomb Timer")]
	public Text lengthCounter;
	public int bombTimer;
	public bool bombActive;
	public AudioClip mineAudio;
	public AudioClip tickAudio;
	public AudioClip bombAudio;
	public GameObject dustCloud;
	public Animator scoreAnimator;
	private bool oneTime = false;

	void Update()
	{
		if (oneTime == false && GameController.gameOver == true)
		{
			StopCountdown();
			oneTime = true;
		}
	}

	// called when the wormHead collides with aomething
	void OnTriggerEnter2D(Collider2D other)
	{
		// if the object is an enemey or bomb hazard
		if (other.CompareTag("Mole") || other.CompareTag("ScubaMole"))
		{
			// instantiates a blood splatter effect and destroys the head
			if (goldWorm.goldWorm == false)
				Instantiate(splatReg, transform.position, transform.rotation);
			else if (goldWorm.goldWorm == true)
				Instantiate(splatGold, transform.position, transform.rotation);
			
			Destroy(gameObject);

			// kills off body chunks (deactivating collider, starting death animation)
			for (int i = 0; i < ClickMover.body.Count; i++)
			{
				ClickMover.body[i].GetComponentInChildren<Collider2D>().enabled = false;
				ClickMover.body[i].GetComponent<Animator>().SetBool("isDead", true);
				Destroy(ClickMover.body[i], 3);
			}
				
			if (goldWorm.goldWorm == false)
				SoundPlayer.instance.PlaySingle2(bloodSplat);
			else if (goldWorm.goldWorm == true)
				SoundPlayer.instance.PlaySingle2(bloodShatter);

			// calls gameover
			GameController.gameOver = true;
		}

		// if the object hit is a bomb
		if (other.CompareTag("BombHZ"))
		{
			other.gameObject.GetComponent<Collider2D>().enabled = false;
			SoundPlayer.instance.PlayNoPitchVariation(mineAudio);
			GameObject instance = Instantiate(dustCloud, gameObject.transform.position, Quaternion.identity) as GameObject;
			instance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			gameController.instaDestroy.Add(instance);

			StartCoroutine("BombCountdown");
		}


		// if the object is a half hazard - cuts the worm in half
//		if (other.tag == "HalfHZ" && ClickMover.body.Count > 1)
//		{
//			// saves the original body length before halving
//			int bodyLength = ClickMover.body.Count / 2;
//
//			// kills off half the body chunks (deactivating collider, starting death animation, removing from list)
//			for (int i = ClickMover.body.Count - 1; i > bodyLength - 1; i--)
//			{
//				ClickMover.body[i].GetComponentInChildren<Collider2D>().enabled = false;
//				ClickMover.body[i].GetComponent<Animator>().SetBool("isDead", true);
//				Destroy(ClickMover.body[i], 3);
//				ClickMover.body.RemoveAt(i);
//			}
//
//			// plays audio
//			soundManager.PlaySingle2(sliceAudio);
//
//			// updates score counter and moves hazard offscreen
//			gameController.UpdateScore();
//			other.gameObject.transform.position = new Vector3(-16f, -2f, 0f);
//		}

		// if the object is a potion hazard - flips body parts to other colour
//		if (other.tag == "PotionHZ")
//		{
//			// turns all body parts pink
//			if (gameController.pinkWorm == false)
//			{
//				for (int i = 0; i < ClickMover.body.Count; i++)
//				{
//					SpriteRenderer bodySprite = ClickMover.body[i].GetComponentInChildren<SpriteRenderer>();
//					bodySprite.sortingOrder = 1;
//					ClickMover.body[i].tag = "LiveBody";
//					bodySprite.sprite = pinkWorm;
//				}
//
//				gameController.pinkWorm = true;
//
//				// plays audio and moves hazard offscreen
//				soundManagerScript.PlaySingle(beepAudio);
//				other.gameObject.transform.position = new Vector3(-14f, -2f, 0f);
//			}
//			// turns all body parts green
//			else if (gameController.pinkWorm == true)
//			{
//				for (int i = 0; i < ClickMover.body.Count; i++)
//				{
//					SpriteRenderer bodySprite = ClickMover.body[i].GetComponentInChildren<SpriteRenderer>();
//					bodySprite.sortingOrder = 0;
//					ClickMover.body[i].tag = "Body";
//					bodySprite.sprite = greenWorm;
//				}
//
//				gameController.pinkWorm = false;
//
//				// plays audio and moves hazard offscreen
//				soundManagerScript.PlaySingle(beepAudio);
//				other.gameObject.transform.position = new Vector3(-14f, -2f, 0f);
//			}
//		}
	}

	public IEnumerator BombCountdown()
	{
		bombActive = true;

		for (int i = bombTimer; i > 0; i--)
		{
			lengthCounter.text = "" +i;
			scoreAnimator.SetTrigger("Pulse");
			yield return new WaitForSeconds(1);
			SoundPlayer.instance.PlaySingle1(tickAudio);
		}

		SoundPlayer.instance.PlaySingle1(bombAudio);
		bombActive = false;
		gameController.UpdateScore();

		if (goldWorm.goldWorm == false)
			Instantiate(splatReg, transform.position, transform.rotation);
		else if (goldWorm.goldWorm == true)
			Instantiate(splatGold, transform.position, transform.rotation);

		Destroy(gameObject);
		GameController.gameOver = true;
	}

	public void StopCountdown()
	{
		StopCoroutine("BombCountdown");
		bombActive = false;
		gameController.UpdateScore();
	}
}