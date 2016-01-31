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
    //allows us to access the character controller for movement (such as SimpleMove()
    public CharacterController controller;

    //used to access the button properties
    GameObject blueButton;
    GameObject greenButton;
    GameObject redButton;
    GameObject yellowButton;

    //used to access the panel base
    GameObject panelBase;

    //knob and ring gameobjects
    public static GameObject ring0,ring1,ring2,ring3,ring4,ring5,ring6,ring7,ring8,ring9,ring10,ring11,ring12,ring13,ring14,ring15;
    public static GameObject[] ringArray = new GameObject[16];
    public static GameObject knob;
    //knob and ring variables
    public int currentRingState = 0;        //number between 0-15 corresponding to the state of the led rings
    private static int previousRingState = 0;   //GLOBAL ONLY TO THIS CLASS!

    //movement variables
    public float sensitivity = 1f;          //controls the sensitivity of looking around
    public float speed = 1f;                //controls the speed of movement

    public float minY = -60f;               //used to restrict the movement in the Y direction
    public float maxY =  60f;               //how far we can look up and down

    //overridden by Occulus headset
    public float rotationY =  0f;           //stores X and Y rotation information for looking around
    public float rotationX =  0f;

    //used to calculate distance from panel
    float x1,x2,y1,y2,z1,z2;
    //used to store the distance from player to panel
    float distance = 0f;

    //class wide counter
    uint counter = 0;
    public static bool secretModeActive = false;
    bool tempBlueStatus;
    bool tempRedStatus;
    bool tempGreenStatus;
    bool tempYellowStatus;
    int tempEncoderStatus;

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

        //start at the same state as the panel
        sendAll();


	}//end start
	
	void Update ()
    {
        //method responsible for rotationg the camera (right joystick or Occulus headset)
        rotateCamera(); //DISABLE FOR CAMERA TESTING
        //move forward and backward directions
        moveForward();
        //move left and right directions
        strafe();
        //are we in range of the panel to interact?
        //only allow interaction with the panel if we are in range
        if (inRange() && !secretModeActive )
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
        //allow looking to be done with either the right joystick or the arrow keys
        //when occulus is not enabled

        //horizontal movement (left and right)
        if(Input.GetKey(KeyCode.LeftArrow) )
            rotationX = transform.localEulerAngles.y + -1 * sensitivity;
        else if(Input.GetKey(KeyCode.RightArrow) )
            rotationX = transform.localEulerAngles.y + 1 * sensitivity;
        else 
            rotationX = transform.localEulerAngles.y + Input.GetAxis("RightHorizontal") * sensitivity;

        //vertical movement (up and down)
        if (Input.GetKey(KeyCode.DownArrow))
            rotationY = rotationY + -1 * sensitivity;
        else if (Input.GetKey(KeyCode.UpArrow))
            rotationY = rotationY + 1 * sensitivity;
        else
            rotationY = rotationY + Input.GetAxis("RightVertical") * sensitivity;


        //take Y rotation value and fit it into an interval [what you are fitting, min , max]
        //we do not want the player to be able to look down or up farther than 60 degrees
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        //now rotate the camera based on X and Y rotations
        //assign the gameobject's transform (position) a 3-d vector containing (x,y,z)
        transform.localEulerAngles = new Vector3(-1 * rotationY, rotationX, 0);

    }//end rotate camera

    //move forward and backward by checking positive and negative values of left joystick
    void moveForward()
    {
        //FORWARD
        if (Input.GetAxis("LeftVertical") > 0 || Input.GetKey(KeyCode.W) )
            controller.SimpleMove(transform.forward * speed);
        //BACKWARD
        else if (Input.GetAxis("LeftVertical") < 0 || Input.GetKey(KeyCode.S))
            controller.SimpleMove(-1 * transform.forward * speed);
        
    }//end moveForward

    //strafing back and forth by checking positive and negactive values of left joystick
    void strafe()
    {
        //an extra factor of 1.2 was added to compensate for slower strafing and foward movement
        //LEFT
        if (Input.GetAxis("LeftHorizontal") < 0 || Input.GetKey(KeyCode.A))
            controller.SimpleMove(-1 * transform.right * speed * 1.2f);
        else if (Input.GetAxis("LeftHorizontal") > 0 || Input.GetKey(KeyCode.D))
            controller.SimpleMove(transform.right * speed * 1.2f);
        
    }//end strafe
        
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
        //RED
        //light up the blue box if the blue key on the gamepad is pressed ( X )
        if (Input.GetKeyDown(KeyCode.Joystick1Button2))                                                // red button down
        {
            CallRed.REDLEDSTATUS = !CallRed.REDLEDSTATUS;

            if (CallRed.REDLEDSTATUS)
            {
                redButton.GetComponent<Renderer>().material.color = Color.red;
                CallRed.REDLIGHT.enabled = true;
                SoundEffects.buttonClick.Play();
            }
            else
            {
                redButton.GetComponent<Renderer>().material.color = Color.white;
                CallRed.REDLIGHT.enabled = false;
                SoundEffects.buttonClick.Play();
            }
            if (Communicate.sp.IsOpen) //make sure the port is open before we send
            {
                Communicate.sendRed(CallRed.REDLEDSTATUS);
            }
            //Communicate.sendBlueTEST(BLUELEDSTATUS);
        }
            
        //BLUE
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))                                                // blue button down
        {
            CallBlue.BLUELEDSTATUS = !CallBlue.BLUELEDSTATUS;

            if (CallBlue.BLUELEDSTATUS)
            {
                blueButton.GetComponent<Renderer>().material.color = Color.blue;
                CallBlue.BLUELIGHT.enabled = true;
                SoundEffects.buttonClick.Play();
                //start a counter to see how long we have pressed the blue button down for
                InvokeRepeating("checkMode", 0, 1);
            }
            else
            {
                blueButton.GetComponent<Renderer>().material.color = Color.white;
                CallBlue.BLUELIGHT.enabled = false;
                SoundEffects.buttonClick.Play();
                CancelInvoke();
                counter = 0;
            }
            if (Communicate.sp.IsOpen) //make sure the port is open before we send
            {
                Communicate.sendBlue(CallBlue.BLUELEDSTATUS);
            }
            //Communicate.sendBlueTEST(BLUELEDSTATUS);
        }
            
        //GREEN
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))                                                // green button down
        {
            CallGreen.GREENLEDSTATUS = !CallGreen.GREENLEDSTATUS;

            if (CallGreen.GREENLEDSTATUS)
            {
                greenButton.GetComponent<Renderer>().material.color = Color.green;
                CallGreen.GREENLIGHT.enabled = true;
                SoundEffects.buttonClick.Play();
            }
            else
            {
                greenButton.GetComponent<Renderer>().material.color = Color.white;
                CallGreen.GREENLIGHT.enabled = false;
                SoundEffects.buttonClick.Play();
            }
            if (Communicate.sp.IsOpen) //make sure the port is open before we send
            {
                Communicate.sendGreen(CallGreen.GREENLEDSTATUS);
            }
            //Communicate.sendBlueTEST(BLUELEDSTATUS);
        }

        //YELLOW
        if (Input.GetKeyDown(KeyCode.Joystick1Button3))                                                // yellow button down
        {
            CallYellow.YELLOWLEDSTATUS = !CallYellow.YELLOWLEDSTATUS;

            if (CallYellow.YELLOWLEDSTATUS)
            {
                yellowButton.GetComponent<Renderer>().material.color = Color.yellow;
                CallYellow.YELLOWLIGHT.enabled = true;
                SoundEffects.buttonClick.Play();
            }
            else
            {
                yellowButton.GetComponent<Renderer>().material.color = Color.white;
                CallYellow.YELLOWLIGHT.enabled = false;
                SoundEffects.buttonClick.Play();
            }
            if (Communicate.sp.IsOpen) //make sure the port is open before we send
            {
                Communicate.sendYellow(CallYellow.YELLOWLEDSTATUS);
            }
        }
    }//end checkButtons

    //code that will light up and turn off the ring of LED's
    void checkRings()
    {
        //update date ring state based off of global variable encoderledstatus
        //both Unity and the panel use this varible to determine their states
        currentRingState = CallKnob.ENCODERLEDSTATUS;


        //move around the rings counterclockwise using L key or left bumper
        if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            //if we rotate to the left turn the next light on the and previous one off
            //also rotate the knob
            //turn the current ring off, the next ring on, update the current ring state, rotate the knob, and play the sound
            ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.black;

            //if we are at the end of the list wrap back around to ring0
            if (currentRingState == 0)
                currentRingState = 15;
            else
                currentRingState -= 1;

            ringArray[currentRingState].GetComponent<Renderer>().material.color = Color.green;
            //SoundEffects.knobClick.Play();

            CallKnob.ENCODERLEDSTATUS = currentRingState;
            if (Communicate.sp.IsOpen) //make sure the port is open before we send
            {
                Communicate.sendKnob(CallKnob.ENCODERLEDSTATUS);
            }
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

            //SoundEffects.knobClick.Play();

            CallKnob.ENCODERLEDSTATUS = currentRingState;
            if (Communicate.sp.IsOpen) //make sure the port is open before we send
            {
                Communicate.sendKnob(CallKnob.ENCODERLEDSTATUS);
            }
        }

        //this section was added due to the panel's inability to know which way to turn the knob
        //now check the previous state against the current state so we can rotate accordingly

        //clockwise rotation
        if ( (currentRingState -previousRingState) == 1 || (currentRingState -previousRingState) == -15 )
        {
            knob.transform.Rotate(new Vector3(0, -1 * 22.5f, 0), Space.Self);
            SoundEffects.knobClick.Play();
        }
        //counterclockwise rotation
        else if( (currentRingState -previousRingState) == -1 || (currentRingState -previousRingState) == 15)
        {
            knob.transform.Rotate(new Vector3(0, 22.5f, 0), Space.Self);
            SoundEffects.knobClick.Play();
            //also make green and black from here
        }
        //by default nothing should happen if they are equal because the state
        //of the leds has not changed
   
        previousRingState = currentRingState; //save the state before we leave

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
        {
            //Debug.Log("in range");
            return true;
        }
        else
        {
            //Debug.Log("not in range");
            return false;
        }
    }// end inRange()


    //secret mode that can be entered by holding the blue button down for a specified amount of time
    void checkMode()
    {
        if (Input.GetKey(KeyCode.JoystickButton0) )
        {
            counter++;
            Debug.Log("counter is " + counter);
        }
        else
        {
            counter = 0;
        }

        if (counter == 5)
        {
            Debug.Log("secret mode entered!!");
            CancelInvoke();
            InvokeRepeating("secretMode1", 0, 0.25f);
            counter = 0;
            secretModeActive = true;
            //save current status
            tempBlueStatus = CallBlue.BLUELEDSTATUS;
            tempRedStatus = CallRed.REDLEDSTATUS;
            tempGreenStatus = CallGreen.GREENLEDSTATUS;
            tempYellowStatus = CallYellow.YELLOWLEDSTATUS;
            tempEncoderStatus = CallKnob.ENCODERLEDSTATUS;
            //SoundEffects.knobClick.mute = true;
        }
            
    }//end check secret mode

    void secretMode1()
    {
        
        if (counter % 4 == 0)
        {
            CallRed.REDLEDSTATUS = true;
            CallBlue.BLUELEDSTATUS = false;
            CallGreen.GREENLEDSTATUS = false;
            CallYellow.YELLOWLEDSTATUS = false;

            if (CallKnob.ENCODERLEDSTATUS != 15)
                CallKnob.ENCODERLEDSTATUS += 1;
            else
                CallKnob.ENCODERLEDSTATUS = 0;
            
            sendAll();
        }
        else if (counter % 4 == 1)
        {
            CallRed.REDLEDSTATUS = false;
            CallBlue.BLUELEDSTATUS = true;
            CallGreen.GREENLEDSTATUS = false;
            CallYellow.YELLOWLEDSTATUS = false;

            if (CallKnob.ENCODERLEDSTATUS != 15)
                CallKnob.ENCODERLEDSTATUS += 1;
            else
                CallKnob.ENCODERLEDSTATUS = 0;
            
            sendAll();
        }
        else if (counter % 4 == 2)
        {
            CallRed.REDLEDSTATUS = false;
            CallBlue.BLUELEDSTATUS = false;
            CallGreen.GREENLEDSTATUS = true;
            CallYellow.YELLOWLEDSTATUS = false;

            if (CallKnob.ENCODERLEDSTATUS != 15)
                CallKnob.ENCODERLEDSTATUS += 1;
            else
                CallKnob.ENCODERLEDSTATUS = 0;

            sendAll();
        }
        else if (counter % 4 == 3)
        {
            CallRed.REDLEDSTATUS = false;
            CallBlue.BLUELEDSTATUS = false;
            CallGreen.GREENLEDSTATUS = false;
            CallYellow.YELLOWLEDSTATUS = true;

            if (CallKnob.ENCODERLEDSTATUS != 15)
                CallKnob.ENCODERLEDSTATUS += 1;
            else
                CallKnob.ENCODERLEDSTATUS = 0;

            sendAll();
        }

        counter++;

        if (counter == 20)
        {
            counter = 0;
            CancelInvoke();
            CallRed.REDLEDSTATUS = tempRedStatus;
            CallBlue.BLUELEDSTATUS = tempBlueStatus;
            CallGreen.GREENLEDSTATUS = tempGreenStatus;
            CallYellow.YELLOWLEDSTATUS = tempYellowStatus;
            CallKnob.ENCODERLEDSTATUS = tempEncoderStatus;
            sendAll();
            //SoundEffects.knobClick.mute = false;
            secretModeActive = false;
        }

    }//end secretMode1

    //used to set all data about lights to comm port
    void sendAll()
    {
        if (Communicate.sp.IsOpen) //make sure the port is open before we send
        {
            Communicate.sendRed(CallRed.REDLEDSTATUS);
            Communicate.sendBlue(CallBlue.BLUELEDSTATUS);
            Communicate.sendGreen(CallGreen.GREENLEDSTATUS);
            Communicate.sendYellow(CallYellow.YELLOWLEDSTATUS);
            Communicate.sendKnob(CallKnob.ENCODERLEDSTATUS);
        }
    }
        
}//end script
