using UnityEngine;

public class FloatingWord : MonoBehaviour {

	[HideInInspector] public float original_y;
	[HideInInspector] public bool shouldFloat;
	float speed;
	float amplitude;
	// Use this for initialization
	void Start () {
		original_y = transform.position.y;
		shouldFloat = true;
		float speed = Random.Range(1,4);
		float amplitude = Random.Range(1,4);
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