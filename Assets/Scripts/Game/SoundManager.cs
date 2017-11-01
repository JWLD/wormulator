using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
	[Header("Buttons")]
	public GameObject SFXCross;
	private bool SFXMuted = false;
	public GameObject musicCross;
	private bool musicMuted = false;
	private float SFXStartVolume = 0.2f;
	private float musicStartVolume = 1;

	[Header("Audio Clips")]
	public AudioClip musicLoop;
	public AudioClip deathMusic;
	private bool oneTime = true;

	void Start()
	{
		// mutes at start if player has previously muted either option
		if (PlayerPrefs.GetInt("MuteSFX") == 1)
			MuteSFX();

		if (PlayerPrefs.GetInt("MuteMusic") == 1)
			MuteMusic();
	}

	void Update()
	{
		if (oneTime == true && GameController.gameOver == true)
		{
			oneTime = false;
			PlayDeathMusic();
		}
	}

	public void MuteSFX()
	{
		// mutes SFX
		if (SFXMuted == false)
		{
			SFXCross.SetActive(true);
			SoundPlayer.instance.SFXSource1.volume = 0;
			SoundPlayer.instance.SFXSource2.volume = 0;
			SoundPlayer.instance.SFXSource3.volume = 0;
			SFXMuted = true;
			PlayerPrefs.SetInt("MuteSFX", 1);
		}
		// unmutes SFX
		else if (SFXMuted == true)
		{
			SFXCross.SetActive(false);
			SoundPlayer.instance.SFXSource1.volume = SFXStartVolume;
			SoundPlayer.instance.SFXSource2.volume = SFXStartVolume;
			SoundPlayer.instance.SFXSource3.volume = SFXStartVolume;
			SFXMuted = false;
			PlayerPrefs.SetInt("MuteSFX", 0);
		}
	}

	public void MuteMusic()
	{
		// mutes music
		if (musicMuted == false)
		{
			musicCross.SetActive(true);
			SoundPlayer.instance.musicSource.volume = 0;
			musicMuted = true;
			PlayerPrefs.SetInt("MuteMusic", 1);
		}
		// unmutes music
		else if (musicMuted == true)
		{
			musicCross.SetActive(false);
			SoundPlayer.instance.musicSource.Play();
			SoundPlayer.instance.musicSource.volume = musicStartVolume;
			musicMuted = false;
			PlayerPrefs.SetInt("MuteMusic", 0);
		}
	}

	public void PlayDeathMusic()
	{
		if (musicMuted == false)
		{
			SoundPlayer.instance.musicSource.clip = deathMusic;
			SoundPlayer.instance.musicSource.Play();
		}
	}

	public void ResumeNormalMusic()
	{
		if (musicMuted == false)
		{
			SoundPlayer.instance.musicSource.clip = musicLoop;
			SoundPlayer.instance.musicSource.Play();
		}
	}
}
