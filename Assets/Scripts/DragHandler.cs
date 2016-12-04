using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	Transform startParent;

	Transform originParent;
	Vector3 originPosition;

	[SerializeField] AudioClip pickup;
	[SerializeField] AudioClip putDown;

	AudioSource audioSource;

	void Start(){
		originParent = transform.parent;
		originPosition = transform.position;
		audioSource = GetComponent<AudioSource>();
	}

	public void resetPosition(){
		transform.SetParent(originParent);
		transform.position = originPosition;
		FloatingWord fw = GetComponent<FloatingWord>();
		if(fw != null){
			fw.shouldFloat = true;
		}
	}

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		audioSource.clip = pickup;
		audioSource.Play();
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		transform.SetParent(transform.root);
		FloatingWord fw = itemBeingDragged.GetComponent<FloatingWord>();
		if(fw != null)
			fw.shouldFloat = false;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		// FloatingWord fw = itemBeingDragged.GetComponent<FloatingWord>();
		// if(fw != null){
		// 	fw.original_y = transform.position.y;
		// 	fw.shouldFloat = true;
		// }
		
		itemBeingDragged = null;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		if(transform.parent == transform.root){
			transform.position = startPosition;
			// if(fw != null){
			// 	fw.original_y = transform.position.y;
			// 	fw.shouldFloat = true;
			// }
		}
		audioSource.clip = putDown;
		audioSource.Play();
	}

	#endregion

}