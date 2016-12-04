using UnityEngine;

public class InputManager : MonoBehaviour {

	// Use this for initialization
	public LayerMask hitLayer;
	void Start () {

	}

	void Update () {
		
		// if(Input.touchCount > 0){
		// 	int touchCount = Input.touchCount;
		// 	for (int i = 0; i< touchCount; i++){
		// 		if(Input.GetTouch(i).phase == TouchPhase.Began)
		// 			InputOn(Input.GetTouch(i).position);
		// 	}
		// }
		if(Input.GetMouseButtonDown(0)){
			InputOn(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(0)){
			InputUp(Input.mousePosition);
		}
	}

	void InputOn(Vector3 position){
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero, 100, hitLayer);
		print(hit);
		print(hit.collider);
		if(hit.collider != null){
			//if the touch wasn't consumed
			WordDragNDrop wdnd = hit.collider.gameObject.GetComponent<WordDragNDrop>();
			print(wdnd.gameObject);
			if(wdnd != null)
				wdnd.isPicked = true;
		}
	}

	void InputUp(Vector3 position){
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero, 20, hitLayer);
		if(hit.collider != null){
			//if the touch wasn't consumed
			WordDragNDrop wdnd = hit.collider.gameObject.GetComponent<WordDragNDrop>();
			if(wdnd != null)
				wdnd.isPicked = false;
		}
	}
}
