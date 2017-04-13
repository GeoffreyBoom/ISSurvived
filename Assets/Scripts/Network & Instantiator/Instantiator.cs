using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *This script is attached to the plane in the testCamera scene. it enables and disable cameras based on the player selection from the UI scene.
 * DataHolder.player is a static variable which is set to false if the player selection is TPS, true otherwise.  
 * 
 */

public class Instantiator : MonoBehaviour {

    public static GameObject winnerScreen; 

    // Use this for initialization
    void Start ()
    {
        Camera currentCamera = null;

        winnerScreen = GameObject.FindGameObjectWithTag("Screen"); 
        winnerScreen.SetActive(false);
       // GameObject.FindGameObjectWithTag("QueenHealth").gameObject.SetActive(false);

        if (DataHolder.player == false)
        {
            currentCamera = GameObject.FindGameObjectWithTag("TPS").GetComponent<Camera>();

            currentCamera.enabled = true;

            GameObject.FindGameObjectWithTag("MainCamera").gameObject.SetActive(false);
            GameObject.Find("RTSInterface").gameObject.SetActive(false);
            GameObject.Find("Mini Map RTS").gameObject.SetActive(false);
           
         }
        else 
        {
            currentCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            currentCamera.enabled = true;

            GameObject.FindGameObjectWithTag("TPS").gameObject.SetActive(false);
            GameObject.Find("TPSInterface").gameObject.SetActive(false);
            GameObject.Find("Mini Map TPS").gameObject.SetActive(false);

        }
    }
}
