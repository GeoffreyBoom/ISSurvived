using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script is used for the TPS Mini map for the camera to follow the TPS player. 
 * 
 */

public class FollowTarget : MonoBehaviour {

    [SerializeField]
    Vector3 mOffset = new Vector3(0,10,0); // set the camera above the map.
	
	// Update is called once per frame
	void Update () {

        //If we have the at least a player in the room:
        if(DataHolder.player == false)
        {
            //Find the player and update the position with the position of that TPS player
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + mOffset;
            }
        }
        else if(DataHolder.player){
            // follow the RTS camera: 
            transform.position = GameObject.FindGameObjectWithTag("MainCamera").transform.position + mOffset;

        }
    }
}
