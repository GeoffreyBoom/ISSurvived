using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

/*
 * This script is used to destroy the resources when in collision trigger and make it rotate on itself. 
 * 
 * 
 * 
 */
public class Resource : Photon.MonoBehaviour {

    private AudioSource sound; 

	void Start ()
    {
        sound = GetComponent<AudioSource>();
    }

    void Update () {
        //make the resource rotate on it self:
		transform.RotateAround(transform.position, transform.up, 1f);
	}
	
	private void OnTriggerEnter(Collider collision)
	{
        if (collision.gameObject.tag == "Queen" || collision.gameObject.tag == "Alien"){
            FindObjectOfType<RTSInterface>().GotResource();
        }
        else if(collision.gameObject.tag == "Player")
        {
            FindObjectOfType<TPSInterface>().increaseHealthUI();
        }
        //if a any player hits a resource then we destroy it:
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Queen" || collision.gameObject.tag == "Alien")
		{
            sound.Play();
            photonView.RPC("consume", PhotonTargets.All);
            
		}
	}

	[PunRPC]
	private void consume ()
	{
		Destroy(gameObject);
	}
}
