using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class TPSPlayer : Photon.MonoBehaviour
{
    // TPSPlayer class, with variables to represent the character (same as in UI)
    public int currentHealth = 100, stamina;
    public static int ammo = 25;
    public static bool setDamageUI = false, isShooting = false;

    float nextFire, fireRate = 0.3f;

    // GameObjects in the scene that are used for shooting
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
            // If the TPSPlayer is shooting and the time from his last shot is superior to nextFire
            if (Input.GetButton("Fire1") && Time.time > nextFire)
            {
                // If he has enough ammo he shoots
                if (ammo <= 25 && ammo > 0)
                {
                    nextFire = Time.time + fireRate;
                    isShooting = true;
                    Shoot();
                    gun.Fire();
                }

                // If not he automatically recharges
                if (ammo == 0)
                {
                    Recharge();
                }
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

    // TPSPlayer function to update his variables (health, ammo)
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

    // Respawn function is he falls out the window
    void Fall()
    {
        if (transform.position.y < -10.0f)
        {
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}