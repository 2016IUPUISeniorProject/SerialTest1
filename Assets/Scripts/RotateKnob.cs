using UnityEngine;
using System.Collections;

public class RotateKnob : MonoBehaviour {

	int rotationspeed=15;
	//int friction=5;
	int lerpSpeed=5;
	private float xDeg;   //used for reading mouse to rotate knob
	private float yDeg;
	public static Vector3 euler;
	public static Vector3 velocity;
	private Quaternion fromRotation;
	private Quaternion toRotation;
	bool KNOBMOUSEDOWN=false;

	// Use this for initialization
	void Start () {
		
		//RINGARRAY = new GameObject[] {Ring0,Ring1,Ring2,Ring3,Ring4,Ring5,Ring6,Ring7,Ring8,Ring9,Ring10,Ring11,Ring12,Ring13,Ring14,Ring15};
	}
	// Update is called once per frame/// <summary>
	/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// </summary>
	void Update () {
        if (KNOBMOUSEDOWN) //if clicked or right mouse button is down
        {
            Debug.Log("pressed");
			euler.z -= Input.GetAxis ("Mouse X") * rotationspeed;
			//euler.y += Input.GetAxis ("Mouse Y") * rotationspeed;
			gameObject.transform.rotation = Quaternion.Euler(euler);
			GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);

			//xDeg -= Input.GetAxis ("Mouse X") * rotationspeed;
			//yDeg += Input.GetAxis ("Mouse Y") * rotationspeed;



			//GetComponent<Rigidbody>().transform.rotation = Quaternion.Euler(euler);
			//GetComponent<Rigidbody>().velocity = new Vector3(Input.GetAxis ("Horizontal") * 1,0,Input.GetAxis ("Vertical"));

			//fromRotation = transform.rotation;
			//toRotation = Quaternion.Euler (0, 180, xDeg);   // our cylinder has an initial rotation of 90 on x
			///these zeros will need to be updated to the relative angle of the box, if the box is moved in the virtual environment.
			//transform.rotation = Quaternion.Lerp (fromRotation, toRotation, Time.deltaTime * lerpSpeed);


		}

}// end of update

	void OnMouseDown(){
	
		KNOBMOUSEDOWN = true;
	}
	void OnMouseUp(){
		KNOBMOUSEDOWN = false;
	}



}//end of rotate knob
