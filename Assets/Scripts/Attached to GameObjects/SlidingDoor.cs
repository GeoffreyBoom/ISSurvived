using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This script is used for the sliding doors of the game by updating its animation component.
 * 
 */ 

public class SlidingDoor : MonoBehaviour {


    private Animator anim = null;
    private AudioSource doorAudio; 

    void Start()
    {
        anim = GetComponent<Animator>();
        doorAudio = GetComponent<AudioSource>();

    }

    //when the player is in front of the door:
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Alien" || other.gameObject.tag == "Queen")
        {
            anim.SetBool("isOpen", true);

            doorAudio.Play();

        }
    }

    //When the player is exiting the door:
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Alien" || other.gameObject.tag == "Queen")
        {
            StartCoroutine(wait2Secs());

            anim.SetBool("isOpen", false);

            doorAudio.Play();
        }
    }

    //wait 2 seconds for better visuals:
    IEnumerator wait2Secs()
    {
        yield return new WaitForSeconds(2);
    }


}
