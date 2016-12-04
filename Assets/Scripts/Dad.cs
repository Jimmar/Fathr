using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dad : MonoBehaviour {

	[SerializeField] Texture2D []dadTextures;
	// Use this for initialization
	void Start () {
		GetComponent<RawImage>().texture = dadTextures[Random.Range(0,dadTextures.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
