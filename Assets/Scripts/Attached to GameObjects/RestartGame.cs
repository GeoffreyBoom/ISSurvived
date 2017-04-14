using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

//Add the end of the game it is asked if you want to restart or change sides:
public class RestartGame : Photon.MonoBehaviour {


    public void changeSides()
    {
        photonView.RPC("handleNetwork1", PhotonTargets.All);
    }

    public void restartGame()
    {
        photonView.RPC("handleNetwork2", PhotonTargets.All);
    }

    [PunRPC]
    void handleNetwork1()
    {  
        if (PhotonNetwork.isNonMasterClientInRoom || PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player.ID);
            PhotonNetwork.LoadLevel(0);
        }      
    }

    [PunRPC]
    void handleNetwork2()
    {
        if (PhotonNetwork.isNonMasterClientInRoom || PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player.ID);
            PhotonNetwork.LoadLevel(1);
        }
    }

    void OnLeftRoom()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player.ID);
    }

}
