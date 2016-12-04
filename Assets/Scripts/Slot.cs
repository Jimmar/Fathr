﻿using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

	[SerializeField] bool allowMulti;
	public GameObject item {
		get {
			if(transform.childCount>0){
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if(!item || allowMulti){
			DragHandler.itemBeingDragged.transform.SetParent (transform);
			// DragHandler.itemBeingDragged.transform.position = transform.position;
			// ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject,null,(x,y) => x.HasChanged ());
		}
	}
	#endregion
}