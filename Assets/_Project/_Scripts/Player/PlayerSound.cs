using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSound : MonoBehaviour 
{
	public AudioClip walkSound;
	public float walkVolume;
	public AudioClip talkSound;
	public float talkVolume;
	public AudioSource audioSource;
	CharacterController characterController;
	bool isWalk;
	// Start is called before the first frame update
    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
		if (characterController.velocity.magnitude > 0.1f && !isWalk)
		{
			isWalk = true;
			ChooseSound(walkSound, true, walkVolume);
		}
		if (characterController.velocity.magnitude < 0.1f && isWalk)
		{
			isWalk = false;
			audioSource.Stop();
		}
    }

	private void ChooseSound(AudioClip clip, bool loop, float volume)
	{
		audioSource.clip = clip;
		audioSource.loop = loop;
		audioSource.volume = volume;
		audioSource.Play();
	}

	public void PlayTalkSound()
	{
		Debug.Log("PlayTalkSound");
		ChooseSound(talkSound, false, talkVolume);
	}
}
