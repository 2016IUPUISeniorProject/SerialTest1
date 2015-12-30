using UnityEngine;
using System.Collections;

//the following colors the led in virtual world with a light component i.e (REDLED.enabled), sets a flag i.e. REDLEDSTATUS,
//and calls a function in communicate to serially send the updated information to the real world blinky box


public class CallGreen : MonoBehaviour {

	private Light GREENLIGHT;

	public static bool GREENLEDSTATUS=false;
	void Start () {
		
		GREENLIGHT = GetComponent <Light>();
		GREENLIGHT.enabled = false;
		gameObject.GetComponent<Renderer> ().material.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	 void OnMouseDown() {
	 	//print("Clicked");
		GREENLEDSTATUS=!GREENLEDSTATUS;

		if (GREENLEDSTATUS) {
			gameObject.GetComponent<Renderer> ().material.color = Color.green;
			GREENLIGHT.enabled = true;
		} else {gameObject.GetComponent<Renderer> ().material.color = Color.white;
		  		GREENLIGHT.enabled = false;}

		Communicate.sendGreen(GREENLEDSTATUS);
	 }
}
