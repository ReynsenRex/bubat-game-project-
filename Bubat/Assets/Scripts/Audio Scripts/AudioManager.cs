using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource SFXSource;

	public AudioClip bgm;
	public AudioClip shieldHit;
	public AudioClip slash;
	public AudioClip slashHit;
	public AudioClip dodge;
	public AudioClip walk;
	public AudioClip hurt;
	public AudioClip enemyHurt;
	public AudioClip shout;

    public void Awake()
	{
		musicSource.clip = bgm;
		musicSource.Play();
	}

	public void PlaySFX(AudioClip clip)
	{
		SFXSource.PlayOneShot(clip);
	}
	
	public void StopSFX()
	{
		if (SFXSource.isPlaying)
		{
			SFXSource.Stop();
		}
	}
}
