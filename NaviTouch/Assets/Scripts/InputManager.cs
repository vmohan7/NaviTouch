/*
 * This file is part of NaviTouch.
 * Copyright 2015 Vasanth Mohan. All Rights Reserved.
 * 
 * NaviTouch is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * NaviTouch is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with NaviTouch.  If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	//set in the inspector
	public GameObject[] LeftRows;
	public GameObject[] RightRows;

	public Text output;

	public Sprite lockedIcon;
	public Sprite unlockedIcon;

	private Dictionary<int, GameObject> SelectedKeys;
	private bool isLocked = true;

	// Use this for initialization
	void Start () {
		TouchManager.OnTouchUp += HandleOnTouchUp;
		TouchManager.OnTouchMove += HandleOnTouchMove;
		TouchManager.OnTouchDown += HandleOnTouchDown;

		SelectedKeys = new Dictionary<int, GameObject> ();
	}

	void Update() {
		if (!isLocked) {
			transform.parent.rotation = NaviDeviceLocation.DeviceLocation.transform.rotation;
		}
	}

	void OnDestroy(){
		TouchManager.OnTouchUp -= HandleOnTouchUp;
		TouchManager.OnTouchMove -= HandleOnTouchMove;
		TouchManager.OnTouchDown -= HandleOnTouchDown;
	}

	private void HandleOnTouchDown (int fingerID, Vector2 pos)
	{
		SelectKey (fingerID, pos);

	}

	private void HandleOnTouchMove (int fingerID, Vector2 pos)
	{
		DeSelectKey (fingerID);
		SelectKey (fingerID, pos);
	}

	private void HandleOnTouchUp (int fingerID, Vector2 pos)
	{
		GameObject quad = DeSelectKey (fingerID);
		string text = "";
		if (quad.name == "Backspace") {
			output.text = output.text.Remove (output.text.Length - 1); //removes last character
			return;
		} else if (quad.name == "Lock") {
			isLocked = !isLocked;
			Image img = quad.GetComponentInChildren<Image>();
			if (isLocked)
				img.sprite = unlockedIcon;
			else
				img.sprite = lockedIcon;
			return;
		} else if (quad.name == "Space") {
			text = " ";
		} else if (quad.name == "Enter") {
			text = "\n";
		}  
		else {
			text = quad.name;
		}
		output.text += text;
	}

	private void SelectKey(int fingerID, Vector2 pos){
		GameObject quad;
		if (pos.x < TouchManager.Instance.DeviceWidth / 2f) {
			quad = GetQuadFromRows(LeftRows, pos );
		} else {
			quad = GetQuadFromRows(RightRows, new Vector2(pos.x - TouchManager.Instance.DeviceWidth / 2f, pos.y));
		}

		SelectedKeys.Remove (fingerID);
		SelectedKeys.Add (fingerID, quad);
		quad.GetComponent<Renderer> ().material.color = Color.red;
	}

	private GameObject DeSelectKey(int fingerID){
		GameObject quad;
		SelectedKeys.TryGetValue (fingerID, out quad);
		if (quad != null) {
			quad.GetComponent<Renderer> ().material.color = Color.white;
		}

		return quad;
	}

	private GameObject GetQuadFromRows(GameObject[] rows, Vector2 pos) {
		GameObject row = rows [(rows.Length-1) - (int)(pos.y * rows.Length / TouchManager.Instance.DeviceHeight)];
		return row.transform.GetChild ((int)(pos.x * 2f* (float) row.transform.childCount / TouchManager.Instance.DeviceWidth)).gameObject;
	}
}
