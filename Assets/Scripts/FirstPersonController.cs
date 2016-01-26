using UnityEngine;
using System.Collections;

/* RIGHT HORIZONTAL AND RIGHT VERTICAL ARE THE X AND Y AXIS OF THE RIGHT ANALOG STICK
 * THE SAME GOES FOR THE LEFT STICK
 * THE AXIS ARE EQUIVALENT TO THE UNIT CIRCLE AND WILL RETURN A FLOAT ACCORDING TO THE ANGLE THAT THEY ARE TILTED
 * FOR EXAMPLE IF THE LEFT STICK WAS PRESSED STRAIGHT UP THEN (0,1) WOULD BE RETURNED*/

//Script that will be used to move around in the VR environment. A 3-D "capsule" object was created and the main camera was made the child.
//A character controller was also added to the capsule. This script will define all of the movement from the gamepad.

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;  //allows us to access the character controller for movement

    public float sensitivity = 1f;          //controls the sensitivity of the movement
    public float speed = 1f;

    public float minY = -60f;               //used to restrict the movement in the Y direction
    public float maxY =  60f;

    public float rotationY =  0f;
    public float rotationX =  0f;

    bool blueHasBeenPressed = false;
    bool greenHasBeenPressed = false;
    bool yellowHasBeenPressed = false;
    bool redHasBeenPressed = false;

    //used to access the button properties
    GameObject blueButton;
    GameObject greenButton;
    GameObject redButton;
    GameObject yellowButton;
    public GameObject sounds;

    bool bounceFlag = true;               //tells us whether to shift the player up or down

	// Use this for initialization
	void Start () 
    {
        //CAN I FORCE THE CURSOR TO SHOW UP WHILE IN THE SCENE ?
        Cursor.visible = true;

        //refer to the instance of the gameobject we want to access
        redButton = GameObject.FindGameObjectWithTag ("REDLED");
        blueButton = GameObject.FindGameObjectWithTag ("BLUELED");
        greenButton = GameObject.FindGameObjectWithTag ("GREENLED");
        yellowButton = GameObject.FindGameObjectWithTag ("YELLOWLED");
        //refer to sound gameobject
        sounds = GameObject.FindWithTag("Sound");
      
	}
	
	// Update is called once per frame
	void Update ()
    {
        //method responsible for rotationg the camera
        rotateCamera();
        //move forward and backward directions
        moveForward();
        //move left and right directions
        strafing();
        //make player bounce when walking
        //bounce();
        //Debug.Log("("+Input.GetAxis("RightHorizontal")+","+Input.GetAxis("RightVertical")+")");

        //check to see if we need to light any buttons up
        checkButtons();

     



        


	}//end update

    //ROTATION IS OVERRIDED BY THE OCCULUS HEADSET!
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
        transform.localEulerAngles = new Vector3(-1 * rotationY, rotationX, 0);//new Vector3(-rotationY, rotationX, 0);

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
    void strafing()
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


    //method that will make the player bounce when walking
  /*  void bounce()
    {
        //if the player is moving invoke a timer to repeat at some interval
        //InvokeRepeating(string method to call, start time (zero is right now), how often to call in seconds)
        //(STRING, FLOAT, FLOAT)
        //once the player stops moving then cancel the timer
        if (isPlayerMoving())
            InvokeRepeating("shiftPlayer", 0f, 1f);
        else
            CancelInvoke();
    }//end bounce*/

   /* void shiftPlayer()
    {
        if (bounceFlag)
        {
            controller.SimpleMove(transform.up * speed); //move up
            bounceFlag = false; 
            Debug.Log("UP");
        }
        else
        {
            //Transform.position.y = transform.position.y - 0.1; //shift the player down slightly
            controller.SimpleMove(-1 * transform.up * speed);
            bounceFlag = true;
            Debug.Log("down");

        }


    }//end shiftPlayer*/
 

}//end script
