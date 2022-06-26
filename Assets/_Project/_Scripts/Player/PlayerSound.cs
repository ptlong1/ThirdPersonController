using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSound : MonoBehaviour 
{
	public AudioClip walkSound;
	public AudioClip talkSound;
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
			ChooseSound(walkSound, true);
		}
		if (characterController.velocity.magnitude < 0.1f && isWalk)
		{
			isWalk = false;
			audioSource.Stop();
		}
    }

	private void ChooseSound(AudioClip clip, bool loop)
	{
		audioSource.clip = clip;
		audioSource.loop = loop;
		audioSource.Play();
	}

	public void PlayTalkSound()
	{
		ChooseSound(talkSound, false);
	}
}
