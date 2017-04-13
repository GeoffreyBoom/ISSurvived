using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkManager : Photon.PunBehaviour {

    private string VERSION = "v0.0.1";
    public string roomName = "ISS";

    [SerializeField]
    string playerPrefabName = "";
    [SerializeField]
    string queenPrefabName = "";

    AudioSource getReadyAudio;

    GameObject []  weight;

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

     //   Debug.Log("Can't join random room!  DON'T MIND ME, I'LL JUST MAKE MY OWN ROOM.");
        PhotonNetwork.CreateRoom(null);
    }


    //Spawn player:
    public override void OnJoinedRoom()
    {
        int r = Random.Range(0, weight.Length - 1);
        Vector3 startPosition = Vector3.zero;

        startPosition = weight[r].transform.position;

        if (DataHolder.player == false)
        {
        //when we want to instantiate objects that we want ALL players to see:
           PhotonNetwork.Instantiate(playerPrefabName, startPosition, this.transform.rotation, 0);
        }
        else
        {
            PhotonNetwork.Instantiate(queenPrefabName, startPosition, this.transform.rotation, 0);
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.position = new Vector3(startPosition.x, cam.transform.position.y, startPosition.z);

        }

        getReadyAudio.Play();

        GameObject.FindGameObjectWithTag("WeightParent").SetActive(false);

    }
}
