using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityStandardAssets.Characters.ThirdPerson;


public class Bullet : Photon.MonoBehaviour
{

    public ThirdPersonCharacter player;
    Rigidbody bullet;
    Vector3 initPos;
    float mSpeed = 30.0f;

    void Awake()
    {
        // Must be done in Awake() because SetDirection() will be called early. Start() won't work.
        bullet = GetComponent<Rigidbody>();
        initPos = bullet.transform.position;
        // Set a default direction
        bullet.velocity = transform.forward * mSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        if ((transform.position - initPos).magnitude > 30.0f)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        photonView.RPC("deleteThis", PhotonTargets.MasterClient, this.gameObject.name);
       
        //TODO: implement enemy health reduction
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