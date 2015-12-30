using UnityEngine;
using System.Collections;



//the following colors the led in virtual world with a light component i.e (REDLED.enabled), sets a flag i.e. REDLEDSTATUS,
//and calls a function in communicate to serially send the updated information to the real world blinky box

public class CallRed : MonoBehaviour {

	private Light REDLIGHT;
	public static bool REDLEDSTATUS=false;
	void Start () {
		REDLIGHT = GetComponent <Light>();
		REDLIGHT.enabled = false;
		gameObject.GetComponent<Renderer> ().material.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		REDLEDSTATUS=!REDLEDSTATUS;
		if (REDLEDSTATUS) {
			gameObject.GetComponent<Renderer> ().material.color = Color.red;
			REDLIGHT.enabled = true;
		} else {gameObject.GetComponent<Renderer> ().material.color = Color.white;
			REDLIGHT.enabled = false;}

		Communicate.sendRed(REDLEDSTATUS);
		//Communicate.sendRedTEST(REDLEDSTATUS);
	 }


}//end callred
