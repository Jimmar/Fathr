using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour {


	[SerializeField] RawImage tumblrImage;
	[SerializeField] SentanceFormer sentanceFormer;
	[SerializeField] WordPlacement wordPlacement;
	[SerializeField] Text dadText;
	// Use this for initialization
	void Start () {
		UpdateState();
        this.GenerateWords();
    }

	void ChangeImage(string newImagePath){
		Texture2D texture = Resources.Load(newImagePath) as Texture2D;
		tumblrImage.texture = texture;
	}

	public void UpdateState(){
		database.Database.Image image = Game.Instance.currentImage;
		ChangeImage(image.resourcePath);
	}
    private void GenerateWords()
    {
		HashSet<string> wordsList = new HashSet<string>();
        if (!Game.Instance.myScorer.isInAdjectiveState) {
		    foreach(database.Database.LinkedWord linkedword in Game.Instance.currentImage.linkedWords){
			    try{
				    foreach(string word in database.Database.Instance.GetWordsLinkedTo(linkedword.word, 3)){
					    wordsList.Add(word);
				    }
			    }
			    catch{
				    continue;
			    }
		    }
        }
        else
        {
            foreach (string descriptor in database.Database.Instance.GetWordsLinkedTo("_Adjectives", 0))
            {
                wordsList.Add(descriptor);
            }
            foreach (database.Database.LinkedWord imgDescriptor in Game.Instance.currentImage.linkedDescriptors) {
                wordsList.Add(imgDescriptor.word);
            }
        }

		wordPlacement.GenerateWords(wordsList.ToArray());
    }

	public void DadSays(string say, bool shouldGetWords = true){
		dadText.text = say;
        UpdateState();
        if (shouldGetWords) {
            this.GenerateWords();
        }
	}
}
