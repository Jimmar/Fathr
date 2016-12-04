using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {

	[SerializeField] AudioMixer musicMixer;
	[SerializeField] AudioMixerSnapshot confused;
	[SerializeField] AudioMixerSnapshot natural;
	[SerializeField] AudioMixerSnapshot understanding;
    

	public void Confused(){
		confused.TransitionTo(1);
	}

	public void Natural(){
		natural.TransitionTo(1);
	}

	public void Understanding(){
		understanding.TransitionTo(1);
	}
}
