using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;


/* 
 *This script is used for each hits the aliens or overlord get. Destroy the object when the health == 0 
 * 
 */
  
public class AliensDeathListener : Photon.MonoBehaviour
{

    private int health = 3; //default health for the aliens
    Animator anim;
    Slider healthBar;
    private AudioSource hitSound; 



    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("QueenHealthSlider").GetComponent<Slider>();

        if (this.gameObject.tag == "Queen")
        {
            health = 100; //default health for the Overlord.
            healthBar.value = 100;
        }

        anim = GetComponent<Animator>();
        hitSound = GetComponent<AudioSource>();
    }

    void Update()
    {

        //If the alien or overlord is dead (health == 0) then start the animation and destroy the object:
        if (health <= 0)
        {
            anim.SetBool("dead", true);

            //If the overlord is dead then the RTS player won the game: 
            if (this.gameObject.tag == "Queen")
            {
                //Show the Winner Screen:
                Instantiator.winnerScreen.SetActive(true);
                Time.timeScale = 0; //freeze the game
                GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "|SURVIVOR|";
            }
            else
            {
                photonView.RPC("deleteThis", PhotonTargets.MasterClient, this.gameObject.name);
            }
        }
    }
    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == "Bullet")
        {
            hitSound.Play();
            anim.SetBool("hit", true);

            if (this.gameObject.tag == "Queen")
            {
                health -= 3; // after getting hit by a bullet, decrease the health value.
                healthBar.value -= 3; // the Queen's health bar on the screen (seen by all players)

                if (anim.GetBool("start") == false)
                {
                    anim.SetBool("start", true);
                }
                else
                {
                    anim.SetBool("start", false);
                }

            }
            else
            {
                health--;
            }
        }

        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<TPSPlayer>().TakeDamage();
        }

        anim.SetBool("hit", false);


    }

    //Destroy the game object: 
    [PunRPC]
    void deleteThis(string dot)
    {
        if (gameObject.name == dot)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public int getHealth()
    {
        return health;
    }
}
