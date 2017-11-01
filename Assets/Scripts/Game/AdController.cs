using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour
{
	public int adFrequency;

	public void ShowAd()
	{
		StartCoroutine(DisplayAdWhenReady());
	}

	IEnumerator DisplayAdWhenReady()
	{
		while (!Advertisement.IsReady())
			yield return null;

		Advertisement.Show();
	}
}