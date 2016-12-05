using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLose : MonoBehaviour {

	// Use this for initialization
	[SerializeField] Texture2D win;
	[SerializeField] Texture2D lose;
	[SerializeField] AudioClip winClip;
	[SerializeField] AudioClip loseClip;
	AudioSource audioSource;

	void Start(){
		audioSource = GetComponent<AudioSource>();
	}

	void Update(){
		
	}
	public void Win(){
		GetComponent<RawImage>().texture = win;
		GetComponent<Animation>().Play();
		audioSource.clip = winClip;
		audioSource.Play();
	}

	public void Lose(){
		GetComponent<RawImage>().texture = lose;
		GetComponent<Animation>().Play();
		audioSource.clip = loseClip;
		audioSource.Play();
	}
}
