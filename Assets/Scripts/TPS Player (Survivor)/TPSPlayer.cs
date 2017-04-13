using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class TPSPlayer : Photon.MonoBehaviour
{
    public int currentHealth, stamina;
    public static int ammo = 25;
    public static bool setDamageUI = false, isShooting = false;

    float nextFire, fireRate = 0.3f;

    [SerializeField]
    string bulletName = "Bullet";

    Gun gun;

    void Start()
    {
        gun = GetComponent<Gun>();
    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire)
            {
                if (ammo <= 25 && ammo > 0)
                {
                    nextFire = Time.time + fireRate;
                    isShooting = true;
                    Shoot();
                    gun.Fire();
                }

                /*     if (ammo == 0)
                     {
                         Recharge();
                     } */
            }

            if (Input.GetButtonUp("Fire1"))
            {
                isShooting = false;
            }

            if (Input.GetKey("r"))
            {
                Recharge();
            }

            Fall();
        }
       
    }

    void Shoot()
    {
        ammo -= 1;
    }

    public static void Recharge()
    {
        isShooting = false;
        ammo = 25;
    }

    public void TakeDamage()
    {
        currentHealth -= 10;
        setDamageUI = true;
    }

    void Fall()
    {
        if (transform.position.y < -10.0f)
        {
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Alien" || col.gameObject.tag == "Queen")
        {
            TakeDamage();
        }
    }
}