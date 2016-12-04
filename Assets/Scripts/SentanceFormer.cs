using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SentanceFormer : MonoBehaviour {
    private static string[] sentenceArray_s = database.SentenceType.sentanceArray;

	int sentanceIndex = 1;
	[SerializeField] GameObject wordSpot;
	[SerializeField] GameObject wordBlock;
    [SerializeField] GameObject nonWordText;
    [SerializeField] HorizontalLayoutGroup layoutGroup;
	// Use this for initialization
	void Start () {
		DisplaySentance(sentenceArray_s[sentanceIndex]);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void DisplaySentance(string sentance){
		DeleteAllChildren();
		string []sentanceSplitted = sentance.Trim().Split(database.SentenceType.delimiterChar);
		for (int i = 0; i < sentanceSplitted.Length; i++){
            GameObject newNonWord = Instantiate(this.nonWordText, this.transform);
            newNonWord.GetComponent<Text>().text = sentanceSplitted[i];
			if(i < sentanceSplitted.Length - 1){
			    Transform newSpot = Instantiate(wordSpot, transform.position, transform.rotation, this.transform).transform;
			}
		}
        // Hack to refresh content layout.
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
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
		sentanceIndex = (sentanceIndex+1) % sentenceArray_s.Length;
		DisplaySentance(sentenceArray_s[sentanceIndex]);
	}
	/// <summary>
	///	Displays the previous sentance from the list
	/// </summary>
	public void PrevSentance(){
		sentanceIndex = sentanceIndex-1;
		if(sentanceIndex < 0)
			sentanceIndex = sentenceArray_s.Length + sentanceIndex;
		DisplaySentance(sentenceArray_s[sentanceIndex]);
	}

	public void SubmitPressed(){
		string output = "";
		List<string> outWordsList = new List<string>();
		foreach(Transform child in transform){
            if (!child.name.Contains("Spot")) {
                continue;
            }

			if(child.childCount<1){
				print("fill all the spaces");
				return;
			}
			outWordsList.Add(child.GetChild(0).GetComponentInChildren<Text>().text);
			output+=child.GetChild(0).GetComponentInChildren<Text>().text+" ";
		}
		print (output);

		database.DadResponder.Instance.SubmitPlayerInput(sentenceArray_s[sentanceIndex], outWordsList, out output);

		print(output);

		GameObject.FindObjectOfType<GameManager>().DadSays(output);
	}
}
