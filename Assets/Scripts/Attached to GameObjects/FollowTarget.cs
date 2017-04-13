using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    [SerializeField]
    Vector3 mOffset = new Vector3(0,10,0); 
	
	// Update is called once per frame
	void Update () {

            
        if(DataHolder.player == false)
        {
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
