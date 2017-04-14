using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Photon;

public class Gun : Photon.MonoBehaviour
{
    // The Gun class is what we use to Instantiate our Bullet objects 
    [SerializeField]
    Transform gun = null;
    [SerializeField]
    GameObject bullet = null;

    // The Gun is attached to our TPSPlayer
    public ThirdPersonCharacter player;
    Bullet b;
    Vector3 pos;
    float nextFire, fireRate = 0.2f;

    // When the TPSPlayer calls the gun's Fire function, our character shoots a bullet
    public void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            // We calculate a value position for our bullet to have the smoothest gun position
            if (player.transform.position.z >= 0)
            {
                if (player.transform.position.x >= 0)
                {
                    pos = gun.transform.position + new Vector3(0.2f, 0f, 0.2f);
                }
                else
                {
                    pos = gun.transform.position + new Vector3(-0.2f, 0f, 0.2f);
                }
            }
            else if (player.transform.position.z < 0)
            {
                if (player.transform.position.x >= 0)
                {
                    pos = gun.transform.position + new Vector3(0.2f, 0f, -0.2f);
                }
                else
                {
                    pos = gun.transform.position + new Vector3(-0.2f, 0f, -0.2f);
                }
            }
            // And we instantiate a Bullet with PhotonNetwork.Instantiate (so every client can see) 
            // at the postion we calculated with the rotation of the player (always shoots in front of the character)
            PhotonNetwork.Instantiate("Bullet", pos, player.transform.rotation, 0);
        }
    }
}