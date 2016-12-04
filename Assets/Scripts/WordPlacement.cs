using UnityEngine;
using UnityEngine.UI;

public class WordPlacement : MonoBehaviour {
	[SerializeField] GameObject blockPrefab;
	string []tempWordList ={"Blue", "Some", "Better", "Game"};
	Rect myRect;
	// Use this for initialization
	void Start () {
		myRect = GetComponent<RectTransform>().rect;
		GenerateWords();
	}
	
	// Update is called once per frame
	void Update () {
				
	}

	public void GenerateWords(){
		for (int i=0; i<tempWordList.Length;i++){
			Transform newWordBlock = Instantiate(blockPrefab, transform.position, transform.rotation).transform;
			newWordBlock.GetComponentInChildren<Text>().text = tempWordList[i];
			Vector3 newPos = transform.position - new Vector3(Random.Range(myRect.position.x +70, -myRect.position.x-70), 
										 					  Random.Range(myRect.position.y +50, -myRect.position.y-50),
										 					  transform.position.z);
			newWordBlock.position = newPos;
			newWordBlock.SetParent(transform);
		}
	}

	
}
