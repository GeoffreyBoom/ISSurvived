using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {

    //Change camera position based on empty game objects placed in the world. 
    public Transform camPosAndRot;
    [SerializeField]
    float smoothSpeedCam = 0.1f;

    GameObject construction; 

    
    AsyncOperation loadLevelCompletely;

    void Awake()
    {
        construction = GameObject.Find("Construction");
        construction.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        transform.position = Vector3.Lerp(transform.position,camPosAndRot.position, smoothSpeedCam);
        transform.rotation = Quaternion.Slerp(transform.rotation, camPosAndRot.rotation, smoothSpeedCam);    
    }

    public void setNextSpace(Transform nextSpace)
    {     
        camPosAndRot = nextSpace;
    }


    public void inConstruction()
    {
        construction.SetActive(true);
    }

    public void desactivateConstruction()
    {
        construction.SetActive(false);
    }

    //Make sure that the scene is fully loaded before doing anything else: 
    IEnumerator switchScene()
    {
        loadLevelCompletely = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        while (!loadLevelCompletely.isDone) yield return null;
    }


    /* This function is called when pressing the the "GOD" or "Survivor" player selectors. It loads the game testCamera scene and sets 
     the DataHolder.player static variable which will enable the right camera for the selected player: */
    public void selectPlayer()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        if (clickedButton.name == GameObject.Find("TPS Player Button").name)
        {
            DataHolder.player = false; 

        }
        else if (clickedButton.name == GameObject.Find("RTS Player Button").name)
        {
            DataHolder.player = true;
        }

        StartCoroutine(switchScene());
    }
}
