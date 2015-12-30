
using UnityEngine;
using System.Collections;

public class CallYellow : MonoBehaviour {

	public static bool YELLOWLEDSTATUS=false;
	private Light YELLOWLIGHT;
	void Start () {
		YELLOWLIGHT = GetComponent <Light>();
		YELLOWLIGHT.enabled = false;
		gameObject.GetComponent<Renderer> ().material.color = Color.white;
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown() {
		//print("Clicked");
		YELLOWLEDSTATUS=!YELLOWLEDSTATUS;
		if (YELLOWLEDSTATUS) {
			gameObject.GetComponent<Renderer> ().material.color = Color.yellow;
			YELLOWLIGHT.enabled = true;
		} else {gameObject.GetComponent<Renderer> ().material.color = Color.white;
			YELLOWLIGHT.enabled = false;}
		Communicate.sendYellow (YELLOWLEDSTATUS);
		}
		
}

