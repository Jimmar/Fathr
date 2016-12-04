using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadSays : MonoBehaviour {

	[SerializeField] AudioClip []understands;
	[SerializeField] AudioClip []kindaUnderstands;
	[SerializeField] AudioClip []confused;
	AudioSource audioSource;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	public void IUnderstand(){
		PlayRandomFrom(understands);
	}

	public void IAmConfused(){
		PlayRandomFrom(confused);
	}

	public void IKindaUnderstand(){
		PlayRandomFrom(kindaUnderstands);
	}

	void PlayRandomFrom(AudioClip []clips){
		audioSource.clip = clips[Random.Range(0,clips.Length)];
		audioSource.Play();
	}
}
