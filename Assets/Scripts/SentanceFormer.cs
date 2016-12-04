using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class SentanceFormer : MonoBehaviour {

	string []tempSentances = {"It's like _ but _", "Do you remember _ ?", "It's the same thing as _"};
	int sentanceIndex = 0;
	[SerializeField] GameObject wordSpot;
	[SerializeField] GameObject wordBlock;
	// Use this for initialization
	void Start () {
		DisplaySentance(tempSentances[sentanceIndex]);
		tempSentances = database.SentenceType.sentanceArray;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void DisplaySentance(string sentance){
		DeleteAllChildren();
		string []sentanceSplitted = sentance.Trim().Split();
		for (int i = 0; i < sentanceSplitted.Length; i++){
			Transform newSpot = Instantiate(wordSpot, transform.position, transform.rotation).transform;
			newSpot.SetParent(transform);
			if(sentanceSplitted[i] != "_"){
				Transform newWordBlock = Instantiate(wordBlock, transform.position, transform.rotation).transform;
				Destroy(newWordBlock.GetComponent<DragHandler>());
				newWordBlock.SetParent(newSpot);
				newWordBlock.GetComponentInChildren<Text>().text = sentanceSplitted[i];
			}
		}
	}

	private void DeleteAllChildren(){
		var children = new List<GameObject>();
		foreach (Transform child in transform) {
			if(child.GetComponentInChildren<DragHandler>() != null)
				child.GetComponentInChildren<DragHandler>().resetPosition();
			children.Add(child.gameObject);
		};
		children.ForEach(child => Destroy(child));
	}

	/// <summary>
	///	Displays the next sentance from the list
	/// </summary>
	public void NextSentance(){
		sentanceIndex = (sentanceIndex+1) % tempSentances.Length;
		DisplaySentance(tempSentances[sentanceIndex]);
	}
	/// <summary>
	///	Displays the previous sentance from the list
	/// </summary>
	public void PrevSentance(){
		sentanceIndex = sentanceIndex-1;
		if(sentanceIndex < 0)
			sentanceIndex = tempSentances.Length + sentanceIndex;
		DisplaySentance(tempSentances[sentanceIndex]);
	}

	public void SubmitPressed(){
		string output = "";
		foreach(Transform child in transform){
			if(child.childCount<1){
				print("fill all the spaces");
				return;
			}
			output+=child.GetChild(0).GetComponentInChildren<Text>().text+" ";
		}
		print (output);
	}
}
