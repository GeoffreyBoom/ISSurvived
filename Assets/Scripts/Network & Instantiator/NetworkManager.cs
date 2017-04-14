using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


/* 
 *This is the network manager script which instantiate the network room for a player, then instantiate the players depending on the side 
 * the user chose to be.  
 * 
 */
  
public class NetworkManager : Photon.PunBehaviour {

    private string VERSION = "v0.0.1";
    public string roomName = "ISS";

    [SerializeField]
    string playerPrefabName = "";
    [SerializeField]
    string queenPrefabName = "";

    AudioSource getReadyAudio;

    GameObject []  weight; // those weights is used to randomly choose a start position for the player

    // Use this for initialization
    void Start ()
    {
        weight = GameObject.FindGameObjectsWithTag("Weight");
        
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.autoJoinLobby = true;
        getReadyAudio = GetComponent<AudioSource>();
    }

    void OnGUI()
    {

        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }


    //Spawn player:
    public override void OnJoinedRoom()
    {
        int r = Random.Range(0, weight.Length - 1);
        Vector3 startPosition = Vector3.zero;

        startPosition = weight[r].transform.position; // randomly selected start position

        if (DataHolder.player == false)
        {
            //Instantiate the TPS player:  
           PhotonNetwork.Instantiate(playerPrefabName, startPosition, this.transform.rotation, 0);
        }
        else
        {
            //Instantiate the RTS player:
            PhotonNetwork.Instantiate(queenPrefabName, startPosition, this.transform.rotation, 0);
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.position = new Vector3(startPosition.x, cam.transform.position.y, startPosition.z);

        }

        getReadyAudio.Play();

        //Put back the weight scene objects back to false:
        GameObject.FindGameObjectWithTag("WeightParent").SetActive(false);

    }
}
