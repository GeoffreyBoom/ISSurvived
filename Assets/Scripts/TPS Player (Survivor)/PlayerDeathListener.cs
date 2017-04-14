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
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "|OVERLORD|";
            Time.timeScale = 0; //freeze the game
        }     
    }

}
