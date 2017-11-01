using UnityEngine;
using System.Collections;

public class EatBody : MonoBehaviour 
{
	[Header("Scripts")]
	public GameController gameController;
	public ClickMover clickMover;
	public GoldWorm goldWorm;

	[Header("Variables")]
	public int chunkIndex;
	private int bodyCount;
	private int deadParts;
	private bool oneTime = false;
	private GameObject player;

	[Header("DeathFX")]
	public GameObject splatReg;
	public GameObject splatGreen;
	public GameObject splatGold;
	public GameObject splatGreenGold;
	public AudioClip bloodSplat;
	public AudioClip bloodShatter;
	public Sprite regDeadHead;
	public Sprite goldDeadHead;
	private SpriteRenderer playerSprite;

	void Start()
	{
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		goldWorm = GameObject.Find("ScoreManager").GetComponent<GoldWorm>();

		player = GameObject.FindWithTag("Player");
		clickMover = player.GetComponent<ClickMover>();
		playerSprite = player.GetComponent<SpriteRenderer>();
	}

	// keeps track of how many chunks there are, and the position of each chunk
	void Update()
	{
		bodyCount = ClickMover.body.Count;
		chunkIndex = ClickMover.body.IndexOf(gameObject);

		// stops body swaying animation at gameover
		if (oneTime == false && GameController.gameOver == true)
		{
			GetComponent<Animator>().enabled = false;
			oneTime = true;
		}
	}

	// detects collision with the BodyChunk
	void OnTriggerEnter2D(Collider2D other)
	{
		// runs EatBodyChunk if the colliding object is a Mole or the Player
		if (other.CompareTag("Mole") || other.CompareTag("Player") || other.CompareTag("ScubaMole"))
		{
			EatBodyChunk();

			if (goldWorm.goldWorm == false)
				SoundPlayer.instance.PlaySingle1(bloodSplat);
			else if (goldWorm.goldWorm == true)
				SoundPlayer.instance.PlaySingle1(bloodShatter);
		}
	}

	// instantiates bloodSplatter, kills off body chunks, calls gameOver if needed
	public void EatBodyChunk()
	{
		// calculates how many body chunks to sever
		deadParts = bodyCount - chunkIndex;

		// saves where to instantiate the blood splatter
		Transform eatenChunk = ClickMover.body[chunkIndex].transform;

		// instantiates the appropriate blood splatter
		if (gameObject.tag != "LiveBody")
		{
			// instantiates contamSplatter and adds to bloodList in GameController
			if (goldWorm.goldWorm == false)
			{
				GameObject instance = Instantiate(splatGreen, eatenChunk.position, eatenChunk.rotation) as GameObject;
				gameController.instaDestroy.Add(instance);
			}
			else if (goldWorm.goldWorm == true)
			{
				GameObject instance = Instantiate(splatGreenGold, eatenChunk.position, eatenChunk.rotation) as GameObject;
				gameController.instaDestroy.Add(instance);
			}
		}
		else
		{				
			// instantiates bloodSplatter and adds to bloodList in GameController
			if (goldWorm.goldWorm == false)
			{
				GameObject instance = Instantiate(splatReg, eatenChunk.position, eatenChunk.rotation) as GameObject;
				gameController.instaDestroy.Add(instance);
			}
			else if (goldWorm.goldWorm == true)
			{
				GameObject instance = Instantiate(splatGold, eatenChunk.position, eatenChunk.rotation) as GameObject;
				gameController.instaDestroy.Add(instance);
			}
		}
			
		// deletes the chunk that was eaten
		Destroy(ClickMover.body[chunkIndex]);

		// deactivates the collider, animates and then destroys the dead chunks
		for (int i = chunkIndex + 1; i < bodyCount; i++)
		{
			ClickMover.body[i].GetComponentInChildren<Collider2D>().enabled = false;
			ClickMover.body[i].GetComponent<Animator>().SetBool("isDead", true);
			Destroy(ClickMover.body[i], 3);

			// adds them to the bloodList for insta-destroy
			gameController.instaDestroy.Add(ClickMover.body[i]);
		}

		// changes player sprite and calls gameover if it was a live body part
		if (gameObject.tag == "LiveBody")
		{
			if (PlayerPrefs.GetInt("GoldWormActive") == 1)
				playerSprite.sprite = goldDeadHead;
			else
				playerSprite.sprite = regDeadHead;

			GameController.gameOver = true;
		}
		// otherwise removes the dead parts from the body list and updates the score
		else
		{
			ClickMover.body.RemoveRange(chunkIndex, deadParts);
			gameController.UpdateScore();
		}
	}
}
