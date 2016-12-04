using UnityEngine;

public class WordDragNDrop : MonoBehaviour {

	public bool isPicked {set;get;}
	// Use this for initialization
	void Start () {
		isPicked = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Dragging(){
		transform.position = Input.mousePosition;
	}
		
}
