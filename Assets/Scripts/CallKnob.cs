using UnityEngine;
using System.Collections;

//the following colors the led in virtual world with a light component i.e (REDLED.enabled), sets a flag i.e. REDLEDSTATUS,
//and calls a function in communicate to serially send the updated information to the real world blinky box


public class CallKnob : MonoBehaviour {

	public static Vector3 NEWZVECTOR;
	public static Vector3 PREVIOUSZVECTOR;

	int PREVIOUS_ENCODERLEDSTATUS=0;
	public static GameObject Ring0,Ring1,Ring2,Ring3,Ring4,Ring5,Ring6,Ring7,Ring8,Ring9,Ring10,Ring11,Ring12,Ring13,Ring14,Ring15;
	public static GameObject[] RINGARRAY; 
	public static float angle;
	public static float PREVIOUSANGLE;
	public static float ANGLETHRESHOLD=25.0F;
	public static int ENCODERLEDSTATUS=0;

	// Use this for initialization
void Start () {
		 //NAME ALL THE CHILDREN (LEDS IN RING) SO THEY CAN BE REFERENCED (I USED TAGS IN UNITY)
		INITIATECHILDREN();  //INITIATES EACH INDIVIDUAL LED SO THEY CAN BE ACCESSED
		//NAME ALL THE CHILDREN (LEDS IN RING) SO THEY CAN BE REFERENCED (I USED TAGS IN UNITY)
		RINGARRAY = new GameObject[] {Ring0,Ring1,Ring2,Ring3,Ring4,Ring5,Ring6,Ring7,Ring8,Ring9,Ring10,Ring11,Ring12,Ring13,Ring14,Ring15};

		NEWZVECTOR = gameObject.transform.up;
		//angle = gameObject.transform.eulerAngles.z; //agle of the knob
		PREVIOUSZVECTOR=NEWZVECTOR;

		PREVIOUSANGLE=angle;
	}//end start
	
	// Update is called once per frame
void Update () {
		angle = gameObject.transform.eulerAngles.z;
		NEWZVECTOR = gameObject.transform.up;


		if (Mathf.Abs (angle - PREVIOUSANGLE) > ANGLETHRESHOLD) {
			if (Mathf.Sign(Vector3.Cross(PREVIOUSZVECTOR, NEWZVECTOR).z ) > 0) {
				//GOING CW, RINGLED++
				INCREMENT_RINGLED();
			} else {
				DECREMENT_RINGLED();
			}
		
			//print("previous"+PREVIOUSZVECTOR+"newvector"+NEWZVECTOR);
			PREVIOUSZVECTOR = NEWZVECTOR;
			PREVIOUSANGLE = angle;
			LIGHTRINGLED(ENCODERLEDSTATUS);
			Communicate.sendKnob(ENCODERLEDSTATUS);


		}




}//end update


	void INITIATECHILDREN(){
		Ring0= GameObject.FindGameObjectWithTag("Ring0");Ring1= GameObject.FindGameObjectWithTag("Ring1");Ring2= GameObject.FindGameObjectWithTag("Ring2");
		Ring3= GameObject.FindGameObjectWithTag("Ring3");Ring4= GameObject.FindGameObjectWithTag("Ring4");Ring5= GameObject.FindGameObjectWithTag("Ring5");
		Ring6= GameObject.FindGameObjectWithTag("Ring6");Ring7= GameObject.FindGameObjectWithTag("Ring7");Ring8= GameObject.FindGameObjectWithTag("Ring8");
		Ring9= GameObject.FindGameObjectWithTag("Ring9");Ring10= GameObject.FindGameObjectWithTag("Ring10");Ring11= GameObject.FindGameObjectWithTag("Ring11");
		Ring12= GameObject.FindGameObjectWithTag("Ring12");Ring13 = GameObject.FindGameObjectWithTag ("Ring13");Ring14= GameObject.FindGameObjectWithTag("Ring14");
		Ring15= GameObject.FindGameObjectWithTag("Ring15");
	}

	public static void LIGHTRINGLED(int CURRENTRINGLED){
		RINGARRAY[CURRENTRINGLED].GetComponent<Renderer>().material.color = Color.green;
		for (int j = 0; j <= 15; j++) {							//SET ALL ELSE TO 0??
		if (j != CURRENTRINGLED) {
				RINGARRAY [j].GetComponent<Renderer> ().material.color = Color.white;
			}
		}
		ENCODERLEDSTATUS=CURRENTRINGLED; //IF GETTING A VALUE FROM THE REAL BOX, MATCH THE UNITY VALUE WITH THE PASSED VALUE
	}

	void INCREMENT_RINGLED() {
		if(ENCODERLEDSTATUS==15)
		{ENCODERLEDSTATUS=0;}
		else {ENCODERLEDSTATUS++;}
		
	}

	void DECREMENT_RINGLED() {
		if(ENCODERLEDSTATUS==0)
		{ENCODERLEDSTATUS=15;}
		else {ENCODERLEDSTATUS--;}
	}



}//end callknob




//get objects position gameobject.transform.position; returns vector 3
//transform.up is vector green
///right is red axis
/// //forward is blue axis


//UNITY IS LEFT HANDED, CROSS(OLDVECTOR,NEWVECTOR) = -() --> CCW (RINGLED--)
					  //CROSS(OLDVECTOR,NEWVECTOR) = +() --> CW (RINGLED++)