using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ExitHole : MonoBehaviour 
{
	public Collider2D player;
	public Collider2D finalChunk;

	// turns the hole off once the final chunk leaves the collider
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other == player) 
		{
			finalChunk = ClickMover.body.Last().GetComponent<Collider2D>();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other == finalChunk) 
		{
			gameObject.SetActive(false);
		}
	}
}
