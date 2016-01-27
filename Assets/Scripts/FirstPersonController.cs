using UnityEngine;
using System.Collections;

/* RIGHT HORIZONTAL AND RIGHT VERTICAL ARE THE X AND Y AXIS OF THE RIGHT ANALOG STICK
 * THE SAME GOES FOR THE LEFT STICK
 * THE AXIS ARE EQUIVALENT TO THE UNIT CIRCLE AND WILL RETURN A FLOAT ACCORDING TO THE ANGLE THAT THEY ARE TILTED
 * FOR EXAMPLE IF THE LEFT STICK WAS PRESSED STRAIGHT UP THEN (0,1) WOULD BE RETURNED*/

//Script that will be used to move around in the VR environment. A 3-D "capsule" object was created and the main camera was made the child.
//A character controller was also added to the capsule. This script will define all of the movement from the gamepad.

/*JOYSTICK BUTTONS
 * Input.GetKeyDown( KeyCode.Joystick1Button0 )
 * X = 0
 * A = 1
 * B = 2
 * Y = 3
 * LB = 4
 * RB = 5
 * LT = 6
 * RT = 7
 * BACK = 8
 * START = 9
 * LA = 10
 * RA = 11 */

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;  //allows us to access the character controller for movement (such as SimpleMove()

    //used to access the button properties
    GameObject blueButton;
    GameObject greenButton;
    GameObject redButton;
    GameObject yellowButton;
    //used to access the panel base
    GameObject panelBase;
    //used to access our sounds
    public GameObject sounds;
    //knob and ring gameobjects
    public static GameObject ring0,ring1,ring2,ring3,ring4,ring5,ring6,ring7,ring8,ring9,ring10,ring11,ring12,ring13,ring14,ring15;
    public static GameObject[] ringArray = new GameObject[16];
    GameObject knob;

    //keeps track of whether a button has been pressed or not
    bool blueHasBeenPressed = false;
    bool greenHasBeenPressed = false;
    bool yellowHasBeenPressed = false;
    bool redHasBeenPressed = false;

    public float sensitivity = 1f;          //controls the sensitivity of looking around
    public float speed = 1f;                //controls the speed of movement

    public float minY = -60f;               //used to restrict the movement in the Y direction
    public float maxY =  60f;               //how far we can look up and down

    //overridden by Occulus headset
    public float rotationY =  0f;           //stores X and Y rotation information for looking around
    public float rotationX =  0f;

    //knob and ring variables
    public int currentRingState = 0;        //number between 0-15 corresponding to the state of the led rings

    bool inRangeOfPanel = false;            //used to check whether we are close enough to the panel to interact
    //used to calculate distance from panel
    float x1;
    float x2;
    float y1;
    float y2; 
    float z1;
    float z2;
    //used to store the distance
    float distance = 0f;
  
	void Start () 
    {
        //CAN I FORCE THE CURSOR TO SHOW UP WHILE IN THE SCENE WITH THE OCCULUS HEADSET ?
        Cursor.visible = true;

        //refer to the instance of the gameobject we want to access found in the hierarchy
        //##CAN ALSO REFER TO THE OBJECT ITSELF RATHER THAN THE TAG##//
        // ---->theCube = GameObject.Find("cube");
        redButton = GameObject.Find("REDLED");
        blueButton = GameObject.Find("BLUELED");
        greenButton = GameObject.Find("GREENLED");
        yellowButton = GameObject.Find("YELLOWLED");
        sounds = GameObject.Find("Sounds");
        knob = GameObject.Find("Cylinder");
        panelBase = GameObject.Find("Base");
        //set up the array of rings
        initializeRingArray();
        //now set the initial state of the ring
        ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.green;
        //assign the panel's position which is assigned once assuming that it will not move
        x2 = panelBase.transform.position.x;
        y2 = panelBase.transform.position.y;
        z2 = panelBase.transform.position.z;

	}//start
	
	void Update ()
    {
        //method responsible for rotationg the camera (right joystick or Occulus headset)
        rotateCamera(); //DISABLE FOR CAMERA TESTING
        //move forward and backward directions
        moveForward();
        //move left and right directions
        strafe();
        //are we in range of the panel to interact?
        inRange();

        //only allow interaction with the panel if we are in range
        if (inRange())
        {
            //check to see if we need to light any buttons up
            checkButtons();
            //check which rings we should light up and rotate the knob
            checkRings();
        }

	}//end update

    //ROTATION IS OVERRIDEN BY THE OCCULUS HEADSET!
    //allows the player to look around using either the right joystick or Occulus headset
    void rotateCamera()
    {
        //horizontal movement (left and right)
        rotationX = transform.localEulerAngles.y + Input.GetAxis("RightHorizontal") * sensitivity;
        //vertical movement (up and down)
        rotationY = rotationY + Input.GetAxis("RightVertical") * sensitivity;
        //take that value and fit it into an interval [what you are fitting, min , max]
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        //now rotate the camera based on X and Y rotations
        //assign the gameobject's transform (position) a 3-d vector containing (x,y,z)
        transform.localEulerAngles = new Vector3(-1 * rotationY, rotationX, 0);

    }//end rotate camera

    //move forward and backward by checking positive and negative values of left joystick
    void moveForward()
    {
        //FORWARD
        if (Input.GetAxis("LeftVertical") > 0)
            controller.SimpleMove(transform.forward * speed);
        //BACKWARD
        else if (Input.GetAxis("LeftVertical") < 0)
            controller.SimpleMove(-1 * transform.forward * speed);
    }//end moveForward

    //strafing back and forth by checking positive and negactive values of left joystick
    void strafe()
    {
        //an extra factor of 1.2 was added to compensate for slower strafing and foward movement
        //LEFT
        if (Input.GetAxis("LeftHorizontal") < 0)
            controller.SimpleMove(-1 * transform.right * speed * 1.2f);
        else if (Input.GetAxis("LeftHorizontal") > 0)
            controller.SimpleMove(transform.right * speed * 1.2f);
    }
        
    //return true if we are currently walking
    bool isPlayerMoving()
    {
        if (Input.GetAxis("LeftVertical") != 0 || Input.GetAxis("LeftHorizontal") != 0)
            return true;
        else
            return false;
        
    }//end isPlayerMoving


    //checks to see if any of the buttons need to be lit based on input from gamepad
    //make sure to set flags appropriately so that they serial port sends the correct data
    /*WILL WANT TO CHECK THIS PORTION OF THE CODE WITH THE ACTUAL PANEL CONNECTED*/
    void checkButtons()
    {
        //BLUE
        //light up the blue box if the blue key on the gamepad is pressed ( X )

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) && !blueHasBeenPressed)
        {
            blueButton.GetComponent<Renderer>().material.color = Color.blue;
            CallBlue.BLUELIGHT.enabled = true;
            CallBlue.BLUELEDSTATUS = true;
            blueHasBeenPressed = true;
            sounds.GetComponent<AudioSource>().Play();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) && blueHasBeenPressed)
            {
                blueButton.GetComponent<Renderer>().material.color = Color.white;
                CallBlue.BLUELIGHT.enabled = false;
                CallBlue.BLUELEDSTATUS = false;
                blueHasBeenPressed = false;
                sounds.GetComponent<AudioSource>().Play();
            }
        }

        //GREEN
        //light up the green box if the green key on the gamepad is pressed ( A )
        if (Input.GetKeyDown(KeyCode.Joystick1Button1) && !greenHasBeenPressed)
        {
            greenButton.GetComponent<Renderer>().material.color = Color.green;
            CallGreen.GREENLIGHT.enabled = true;
            CallGreen.GREENLEDSTATUS = true;
            greenHasBeenPressed = true;
            sounds.GetComponent<AudioSource>().Play();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) && greenHasBeenPressed)
            {
                greenButton.GetComponent<Renderer>().material.color = Color.white;
                CallGreen.GREENLIGHT.enabled = false;
                CallGreen.GREENLEDSTATUS = false;
                greenHasBeenPressed = false;
                sounds.GetComponent<AudioSource>().Play();
            }
        }

        //RED
        //light up the blue box if the red key on the gamepad is pressed ( B )
        if (Input.GetKeyDown(KeyCode.Joystick1Button2)  && !redHasBeenPressed)
        {
            redButton.GetComponent<Renderer>().material.color = Color.red;
            CallRed.REDLIGHT.enabled = true;
            CallRed.REDLEDSTATUS = true;
            redHasBeenPressed = true;
            sounds.GetComponent<AudioSource>().Play();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button2) && redHasBeenPressed)
            {
                redButton.GetComponent<Renderer>().material.color = Color.white;
                CallRed.REDLIGHT.enabled = false;
                CallRed.REDLEDSTATUS = false;
                redHasBeenPressed = false;
                sounds.GetComponent<AudioSource>().Play();
            }
        }

        //YELLOW
        //light up the blue box if the yellow key on the gamepad is pressed ( Y )
        if (Input.GetKeyDown(KeyCode.Joystick1Button3) && !yellowHasBeenPressed)
        {
            yellowButton.GetComponent<Renderer>().material.color = Color.yellow;
            CallYellow.YELLOWLIGHT.enabled = true;
            CallYellow.YELLOWLEDSTATUS = true;
            yellowHasBeenPressed = true;
            sounds.GetComponent<AudioSource>().Play();
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Joystick1Button3) && yellowHasBeenPressed)
            {
                yellowButton.GetComponent<Renderer>().material.color = Color.white;
                CallYellow.YELLOWLIGHT.enabled = false;
                CallYellow.YELLOWLEDSTATUS = false;
                yellowHasBeenPressed = false;
                sounds.GetComponent<AudioSource>().Play();
            }
        }
    }//end checkButtons

    //code that will light up and turn off the ring of LED's
    //-----> STILL NEED TO ADD CODE TO SEND OVER SERIAL PORT
    void checkRings()
    {
        //move around the rings counterclockwise using L key or left bumper
        if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            //if we rotate to the left turn the next light on the and previous one off
            //also rotate the knob
            //turn the current ring off, the next ring on, update the current ring state, and rotate the knob
            ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.black;

            //if we are at the end of the list wrap back around to ring0
            if (currentRingState == 0)
                currentRingState = 15;
            else
                currentRingState -= 1;

            ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.green;
            knob.transform.Rotate(new Vector3(0, 22.5f, 0), Space.Self); 
        }
        //move clockwise instead
        else if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            //turn the current ring off, the next ring on, update the current ring state, and rotate the knob
            ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.black;

            //if we are at the end of the list wrap back around to ring0
            if (currentRingState == 15)
                currentRingState = 0;
            else
                currentRingState += 1;

            ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.green;
            knob.transform.Rotate(new Vector3(0, -1 * 22.5f, 0), Space.Self); 
        }
  
/*
 * //EVENTUALLY I WILL WANT TO SEND THIS DATA OVER THE SERIAL PORT, I BELIEVE IT SHOULD BE THE INDEX OF THE LED
        THAT SHOULD BE LIT
        ALSO I SHOULD SET GLOBAL FLAG TO LET ALL OTHER SCRIPTS KNOW IF THE COM PORT IS OPEN OR NOT INSTEAD OF CHECKING EVERY FRAME
        if (Communicate.sp.IsOpen) //make sure the port is open before we send
        {
            Communicate.sendKnob(currentRingState);
        }

*/


    }//end checkRings

    //refer the objects in the array to the actual gameobjects
    void initializeRingArray()
    {
        ring0 = GameObject.Find("Ring0");
        ring1 = GameObject.Find("Ring1");
        ring2 = GameObject.Find("Ring2");
        ring3 = GameObject.Find("Ring3");
        ring4 = GameObject.Find("Ring4");
        ring5 = GameObject.Find("Ring5");
        ring6 = GameObject.Find("Ring6");
        ring7 = GameObject.Find("Ring7");
        ring8 = GameObject.Find("Ring8");
        ring9 = GameObject.Find("Ring9");
        ring10 = GameObject.Find("Ring10");
        ring11 = GameObject.Find("Ring11");
        ring12 = GameObject.Find("Ring12");
        ring13 = GameObject.Find("Ring13");
        ring14 = GameObject.Find("Ring14");
        ring15 = GameObject.Find("Ring15");

        for (int i = 0; i < 16; i++)
        {
            ringArray[i] = GameObject.Find("Ring" + i);
        }

    }//end initializeArray

    //check to see if we are in range of the panel so that interaction can occur
    //return a boolean to check
    bool inRange()
    {
        //update the players position for the distance calculation
        x1 = transform.position.x;
        y1 = transform.position.y;
        z1 = transform.position.z;

        //calculate the distance from our current position to the panel's position
        distance = Mathf.Sqrt( Mathf.Pow((x2-x1),2)  + Mathf.Pow((y2-y1),2) + Mathf.Pow((z2-z1),2) );

        if (distance < 7.3)
            return true;
        else
            return false;
    }// end inRange()
        
}//end script
