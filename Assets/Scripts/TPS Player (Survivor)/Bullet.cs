using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityStandardAssets.Characters.ThirdPerson;


public class Bullet : Photon.MonoBehaviour
{
    // Bullet class to shoot
    // We attach it to the TPSPlayer and give it position and a velocity
    public ThirdPersonCharacter player;
    Rigidbody bullet;
    Vector3 initPos;
    float mSpeed = 30.0f;
    float deathTimer = 0;
    private AudioSource sound;

    void Awake()
    {
        // We get the Bullet Component and we apply a velocity to it so it shoots out of the gun
        bullet = GetComponent<Rigidbody>();
        initPos = bullet.transform.position;
        bullet.velocity = transform.forward * mSpeed;
        sound = GetComponent<AudioSource>();
    }

    void Start()
    {
        sound.Play();
    }
    // Update is called once per frame
    void Update()
    {
        deathTimer += Time.deltaTime;

        // If the bullet is far enough from its original position we destroy it to avoid overcrowding the scene
        if ((transform.position - initPos).magnitude > 50.0f || deathTimer > 4.0f)
        {
            Destroy(this.gameObject);
            deathTimer = 0; 
        }
    }

    void OnTriggerEnter(Collider other)
    {

        // Collision function for the Bullets, we use a PunRPC to get the best result
        if (other.gameObject.tag == "Queen" || other.gameObject.tag == "Alien" || other.gameObject.tag == "Environment")
        {
            photonView.RPC("deleteThis", PhotonTargets.MasterClient, this.gameObject.name);
        }

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