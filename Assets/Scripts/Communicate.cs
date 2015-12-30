using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Text;

public class Communicate : MonoBehaviour {




	//Component KNOB_AND_RING_SCRIPT = KNOB_AND_RING.GetComponent<CallKnob>();
	public static int INTFROMBOX;
	private static bool YELLOWLEDSTATUS_COM = CallYellow.YELLOWLEDSTATUS;
	public static string STRINGFROMBOX;
	public static string STRINGFROMBOX2;
	public static SerialPort sp = new SerialPort ("COM3", 115200, Parity.None, 8, StopBits.One);
	private string tString = string.Empty;
	private byte _terminator = 0xFF;
	public static GameObject REDLED;
	public static GameObject BLUELED;
	public static GameObject GREENLED;
	public static GameObject YELLOWLED;
	public static GameObject[] RINGARRAY;
	public static GameObject Ring0,Ring1,Ring2,Ring3,Ring4,Ring5,Ring6,Ring7,Ring8,Ring9,Ring10,Ring11,Ring12,Ring13,Ring14,Ring15;
	public static GameObject KNOB_AND_RING;
	//open serial connection
	void Start () {
		sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
		sp.DataReceived += DataReceivedHandler2; //alternative experimental way im trying
		//if you want to find another object, tag it in editor and use this code:
		REDLED = GameObject.FindGameObjectWithTag ("REDLED");
		BLUELED = GameObject.FindGameObjectWithTag ("BLUELED");
		GREENLED = GameObject.FindGameObjectWithTag ("GREENLED");
		YELLOWLED = GameObject.FindGameObjectWithTag ("YELLOWLED");
		RINGARRAY = new GameObject[] {Ring0,Ring1,Ring2,Ring3,Ring4,Ring5,Ring6,Ring7,Ring8,Ring9,Ring10,Ring11,Ring12,Ring13,Ring14,Ring15};
		   //this has caused crashes


		sp.DtrEnable = true; //if you do not do this, the event handler method of serial receipt will not work
		sp.RtsEnable = true;    // Request-to-send
		sp.ReadTimeout=10;   
		sp.WriteTimeout=10;    //worked well with 5-10% timeout on knob at timeout=6;
		if (!sp.IsOpen) {
			sp.Open ();
			Debug.Log ("A serial port has been opened");
		sp.DiscardInBuffer ();
		sp.DiscardOutBuffer ();
		} else if (sp.IsOpen) {
			sp.DiscardInBuffer();
			sp.DiscardOutBuffer();
		}
	}//end start
	
	// Update is called once per frame/// <summary>
	/// /////////////////////////////////////////////////////////////////////////////////////////////
	/// </summary>
	void Update () {
	
		if (!sp.IsOpen) {
			Debug.Log ("The serial connection has been interrupted, attempting to reconnect");
			sp.Open ();
		}

		//Debug.Log(sp.ReadTo ("X"));

		//read input from box!!
		try{
			STRINGFROMBOX=sp.ReadTo("X");
			if(STRINGFROMBOX=="r0"){Debug.Log("r Status 0 from Box");
				CallRed.REDLEDSTATUS=false;
				REDLED.GetComponent<Renderer>().material.color = Color.white;
			}
			if(STRINGFROMBOX=="r1"){Debug.Log("r Status 1 from Box");
				CallRed.REDLEDSTATUS=true;
				REDLED.GetComponent<Renderer>().material.color = Color.red;
			}
			if(STRINGFROMBOX=="b0"){Debug.Log("b Status 0 from Box");
				CallBlue.BLUELEDSTATUS=false;
				BLUELED.GetComponent<Renderer>().material.color = Color.white;
			}
			if(STRINGFROMBOX=="b1"){Debug.Log("b Status 1 from Box");
				CallBlue.BLUELEDSTATUS=true;
				BLUELED.GetComponent<Renderer>().material.color = Color.blue;
			}
			if(STRINGFROMBOX=="g0"){Debug.Log("g Status 0 from Box");
				CallGreen.GREENLEDSTATUS=false;
				GREENLED.GetComponent<Renderer>().material.color = Color.white;
			}
			if(STRINGFROMBOX=="g1"){Debug.Log("g Status 1 from Box");
				CallGreen.GREENLEDSTATUS=true;
				GREENLED.GetComponent<Renderer>().material.color = Color.green;
			}
			if(STRINGFROMBOX=="y0"){Debug.Log("y Status 0 from Box");
				CallYellow.YELLOWLEDSTATUS=false;
				YELLOWLED.GetComponent<Renderer>().material.color = Color.white;
			}
			if(STRINGFROMBOX=="y1"){Debug.Log("y Status 1 from Box");
				CallYellow.YELLOWLEDSTATUS=true;
				YELLOWLED.GetComponent<Renderer>().material.color = Color.yellow;
			}
			if(STRINGFROMBOX[0]=='e'){      //check first character from the string, then move forward
															//string.remove(index,count)
				STRINGFROMBOX2=STRINGFROMBOX.Remove(0,1); 	//strings are immutable, make this a new one, what a waste of time!
															//removes first character from string
				//INTFROMBOX= Int32.Parse(STRINGFROMBOX);
				if (Int32.TryParse(STRINGFROMBOX2, out INTFROMBOX))
				{
					
					//need to light up correct ring led, need to go to callknob script
					CallKnob.LIGHTRINGLED(INTFROMBOX);
				}
				else
				{Debug.Log("String could not be parsed.");
				}
			}
			//if(sp.ReadByte()=='r'){Debug.Log("data was found, but the datareceived is not working");sp.ReadByte();}
		}catch{}


	}//end update




	public static void sendYellow (bool YELLOWLEDSTATUS)
	{
		if (YELLOWLEDSTATUS==true) {
			sp.Write ("y1");
			Debug.Log ("YELLOW STATUS 1");
		} else {
			if(YELLOWLEDSTATUS==false)
			sp.Write ("y0");
			Debug.Log ("YELLOW STATUS 0");
		}
	}

	public static void sendGreen(bool GREENLEDSTATUS){
		if (GREENLEDSTATUS==true) {
			sp.Write ("g1");
			Debug.Log ("green STATUS 1");
		} else {
			if(GREENLEDSTATUS==false)
				sp.Write ("g0");
			Debug.Log ("green STATUS 0");
		}
	}

		public static void sendRed(bool REDLEDSTATUS){
		
		if (REDLEDSTATUS==true) {
			sp.Write ("r1");
			
			Debug.Log ("red STATUS 1");
		} else {
			if(REDLEDSTATUS==false)
				sp.Write ("r0");
			Debug.Log ("red STATUS 0");
		}
		}

			public static void sendBlue(bool BLUELEDSTATUS){
		if (BLUELEDSTATUS==true) {
			sp.Write ("b1");
			Debug.Log ("blue STATUS 1");
		} else {
			if(BLUELEDSTATUS==false)
				sp.Write ("b0");
			Debug.Log ("blue STATUS 0");
		}
	}
	public static void sendKnob(int ENCODERLEDSTATUS){
		Debug.Log ("e"+ENCODERLEDSTATUS.ToString());
		//sp.Write ("e100x4");
		sp.Write("e"+ENCODERLEDSTATUS.ToString()+'X');;
	}

	/*
	public static void sendBlueTEST(bool BLUELEDSTATUS){
		sp.Write ("t");
	}

	public static void sendRedTEST(bool BLUELEDSTATUS){
		sp.Write ("k13X");
	}
*/



	private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) 
	{
		SerialPort sp = (SerialPort) sender;
		byte[] buf = new byte[sp.BytesToRead];
		Debug.Log("DATA RECEIVED!");
		sp.Read(buf, 0, buf.Length);
		foreach (Byte b in buf)
		{
			Debug.Log(b.ToString());
		}

	}//end of data received handler 



	private void DataReceivedHandler2(object sender, SerialDataReceivedEventArgs e) {
		SerialPort comm = (SerialPort)sender;
		string incoming_Data = comm.ReadExisting();
		Debug.Log ("THIS IS THE ARDUINO CODE WORKING");
		//richTextBox1.AppendText("Received Data: \n");
		//richTextBox1.AppendText(incoming_Data + "\n");
		//Console.WriteLine(incoming_Data + "\n");
			}




	private void LIGHTRINGLED_FROMBOXINPUT(){
	}
		


}//end of communicate
