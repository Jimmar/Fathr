using UnityEngine;

public class FloatingWord : MonoBehaviour {

	[HideInInspector] public float original_y;
	float speed = 2;
	float amplitude = 2;
	[HideInInspector] public bool shouldFloat;
	// Use this for initialization
	void Start () {
		original_y = transform.position.y;
		shouldFloat = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldFloat){
			Vector2 newPos = transform.position;
			newPos.y = original_y+amplitude*Mathf.Sin(speed*Time.time);
			transform.position = newPos;
		}
	}
}