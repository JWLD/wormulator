using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundPlayer : MonoBehaviour
{
	public static SoundPlayer instance;

	[Header("Audio Sources")]
	public AudioSource musicSource;
	public AudioSource SFXSource1;
	public AudioSource SFXSource2;
	public AudioSource SFXSource3;

	[Header("Pitch Variance")]
	public float lowPitchRange = 0.95f;
	public float highPitchRange = 1.05f;

	void Start()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void PlaySingle1(AudioClip clip)
	{
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		SFXSource1.pitch = randomPitch;

		SFXSource1.clip = clip;
		SFXSource1.Play();
	}

	public void PlaySingle2(AudioClip clip)
	{
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		SFXSource2.pitch = randomPitch;

		SFXSource2.clip = clip;
		SFXSource2.Play();
	}

	public void PlaySingle3(AudioClip clip)
	{
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		SFXSource3.pitch = randomPitch;

		SFXSource3.clip = clip;
		SFXSource3.Play();
	}

	public void PlayNoPitchVariation(AudioClip clip)
	{
		SFXSource3.pitch = 1;
		SFXSource3.clip = clip;
		SFXSource3.Play();
	}
}