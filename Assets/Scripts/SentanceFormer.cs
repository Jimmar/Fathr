using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class SentanceFormer : MonoBehaviour {

	string []sentencesList = {"It's like _ but _", "Do you remember _ ?", "It's the same thing as _"};
	int sentanceIndex = 0;
	[SerializeField] GameObject wordSpot;
	[SerializeField] GameObject wordBlock;
	// Use this for initialization
	void Start () {
		DisplaySentance(sentencesList[sentanceIndex]);
		sentencesList = database.SentenceType.sentanceArray;
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
		sentanceIndex = (sentanceIndex+1) % sentencesList.Length;
		DisplaySentance(sentencesList[sentanceIndex]);
	}
	/// <summary>
	///	Displays the previous sentance from the list
	/// </summary>
	public void PrevSentance(){
		sentanceIndex = sentanceIndex-1;
		if(sentanceIndex < 0)
			sentanceIndex = sentencesList.Length + sentanceIndex;
		DisplaySentance(sentencesList[sentanceIndex]);
	}

	public void SubmitPressed(){
		string output = "";
		List<string> outWordsList = new List<string>();
		List<string> submitWordsList = new List<string>();
		foreach(Transform child in transform){
			if(child.childCount<1){
				print("fill all the spaces");
				return;
			}
			outWordsList.Add(child.GetChild(0).GetComponentInChildren<Text>().text);
			output+=child.GetChild(0).GetComponentInChildren<Text>().text+" ";
		}
		print (output);
		string [] originalList = sentencesList[sentanceIndex].Split();
		for(int i=0; i<originalList.Length; i++){
			if(originalList[i].Equals("-")){
				submitWordsList.Add(originalList[i]);
			}
		}

		database.DadResponder.Instance.SubmitPlayerInput(sentencesList[sentanceIndex],submitWordsList,out output);

		print(output);

		GameObject.Find("GameManager").GetComponent<GameManager>().DadSays(output);
	}
}
