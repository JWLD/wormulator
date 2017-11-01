using UnityEngine;
using System.Collections;

public class HazardBehaviour : MonoBehaviour 
{
	// invisibility timings
	private float fadeTimer;
	public GameObject dustSFX;
	private SpriteRenderer hazardSprite;
	private Animator hazardAnimator;
	private Collider2D hazardCollider;
	public GameController gameController;
	private bool levelComplete;

	void Start()
	{
		hazardSprite = GetComponent<SpriteRenderer>();
		hazardAnimator = GetComponent<Animator>();
		hazardCollider = GetComponent<Collider2D>();
	}

	void Update()
	{
		// deactivates sprite after a set time (making the hazard invisible)
		if (hazardSprite.enabled == true && levelComplete == false)
		{
			fadeTimer += Time.deltaTime;

			if (fadeTimer >= gameController.invisTimer)
			{
				hazardSprite.enabled = false;
				fadeTimer = 0;

				// instantiates dust cloud when the hazard disappears
				GameObject instance = Instantiate(dustSFX, gameObject.transform.position, Quaternion.identity) as GameObject;
				Destroy(instance, 2f);
			}
		}
	}

	// called once all food has been cleared
	public void LevelComplete()
	{
		levelComplete = true;
		hazardSprite.enabled = true;
		hazardCollider.enabled = false;
		hazardAnimator.SetBool("isSafe", true);
	}

	// called when the player enters a new level
	public void NewLevel()
	{
		levelComplete = false;
		hazardCollider.enabled = true;
		fadeTimer = 0;
		hazardAnimator.SetBool("isSafe", false);
	}
}