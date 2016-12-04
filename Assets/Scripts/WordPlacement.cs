using UnityEngine;
using UnityEngine.UI;

public class WordPlacement : MonoBehaviour {
	[SerializeField] GameObject blockPrefab;
	Rect myRect;
	// Use this for initialization
	void Start () {
		myRect = GetComponent<RectTransform>().rect;
	}
	
	// Update is called once per frame
	void Update () {
				
	}

	public void GenerateWords(string []words){
		for (int i=0; i<words.Length;i++){
			Transform newWordBlock = Instantiate(blockPrefab, transform.position, transform.rotation).transform;
			newWordBlock.GetComponentInChildren<Text>().text = words[i];
			Vector3 newPos = transform.position - new Vector3(Random.Range(myRect.position.x +70, -myRect.position.x-70), 
										 					  Random.Range(myRect.position.y +50, -myRect.position.y-50),
										 					  transform.position.z);
			newWordBlock.position = newPos;
			newWordBlock.SetParent(transform);
		}
	}

	
}
