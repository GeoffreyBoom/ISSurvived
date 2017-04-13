using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Photon;

public class Gun : Photon.MonoBehaviour
{

    [SerializeField]
    Transform gun = null;
    [SerializeField]
    GameObject bullet = null;

    public ThirdPersonCharacter player;
    Bullet b;
    Vector3 pos;
    float nextFire, fireRate = 0.2f;


    public void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

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

            PhotonNetwork.Instantiate("Bullet", pos, player.transform.rotation, 0);
        }
    }
}