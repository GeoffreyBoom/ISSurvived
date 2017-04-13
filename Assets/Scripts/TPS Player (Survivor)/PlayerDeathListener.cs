using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class PlayerDeathListener : Photon.MonoBehaviour {

    
    void Update()
    {
        if (GetComponent<TPSPlayer>().currentHealth <= 0)
        {
            //RTS player won the game 
            Instantiator.winnerScreen.SetActive(true);
            GameObject.Find("winnerText").GetComponent<Text>().text = "|QUEEN|";

            Debug.Log("RTS Player is dead");

        }     
    }

    [PunRPC]
    void deleteThis(string dot)
    {
        if (gameObject.name == dot)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
