using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TPSInterface : MonoBehaviour
{
    public Slider healthBar;
    public Slider staminaBar;
    public ThirdPersonCharacter player;
    public Text ammoText;

    int currentHealth, currentStamina, currentAmmo;
    int amount = 3, recharge = 2;
    float nextMove, nextFire, moveRate = 0.1f, fireRate = 0.2f;
    bool running;

    // Use this for initialization
    void Start()
    {
        currentHealth = 100;
        currentStamina = 100;
        currentAmmo = 25;
        setAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {

        if (TPSPlayer.setDamageUI == true)
        {
            setHealthUI();
        }

        if (TPSPlayer.isShooting == true && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (currentAmmo > 0)
            {
                loseAmmo();
                setAmmoUI();
            }

            if (currentAmmo == 0)
            {
                setAmmoUI();
                TPSPlayer.Recharge();
                currentAmmo = 25;
                setAmmoUI();
            }
        }

        if (Input.GetKey("r"))
        {
            TPSPlayer.Recharge();
            currentAmmo = 25;
            setAmmoUI();
        }

        if (Input.GetKey(KeyCode.LeftShift) && staminaBar.value > 0 && Time.time > nextMove)
        {
            running = true;
            nextMove = Time.time + moveRate;
            setStaminaUI();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            running = false;
        }

        if (staminaBar.value < 100)
        {
            setStaminaUI();
        }
    }

    private void setHealthUI()
    {
        currentHealth -= amount;
        healthBar.value = currentHealth;
        TPSPlayer.setDamageUI = false;
    }

    private void setStaminaUI()
    {
        if (running == true)
        {
            currentStamina -= amount;
            staminaBar.value = currentStamina;
        }
        else
        {
            currentStamina += recharge;
            staminaBar.value = currentStamina;
        }
    }

    private void loseAmmo()
    {
        currentAmmo -= 1;
    }

    private void setAmmoUI()
    {
        ammoText.text = currentAmmo + "  /  25";
    }
}