using UnityEngine;
using System.Collections;


//Script dealing with all of the sounds effects that will get played

public class SoundEffects : MonoBehaviour 
{
    //add which ever sounds we need to "sounds" game object
    public static AudioSource buttonClick;
    public static AudioSource knobClick;

	void Start ()
    {   
        //get all of the different sounds and store them in an array
        AudioSource[] audios = GetComponents<AudioSource>();
        //instead of accessing the array directly store each element to another name or ease of use
        //store in the same order as in the "sounds" object
        buttonClick = audios[0];
        knobClick = audios[1];
	}
	
	// Update is called once per frame
	void Update () 
    {
      
	}
}
