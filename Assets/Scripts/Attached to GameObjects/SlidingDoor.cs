using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {


    private Animator anim = null;
    private AudioSource doorAudio; 


    void Start()
    {
        anim = GetComponent<Animator>();
        doorAudio = GetComponent<AudioSource>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Alien" || other.gameObject.tag == "Queen")
        {
            Debug.Log("opening "+ anim.GetBool("isOpen").ToString());

            anim.SetBool("isOpen", true);

            doorAudio.Play();

        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Alien" || other.gameObject.tag == "Queen")
        {
            Debug.Log("closing "+ anim.GetBool("isOpen").ToString());

            StartCoroutine(wait2Secs());

            anim.SetBool("isOpen", false);

            doorAudio.Play();
        }
    }

    IEnumerator wait2Secs()
    {
        yield return new WaitForSeconds(2);
    }


}
