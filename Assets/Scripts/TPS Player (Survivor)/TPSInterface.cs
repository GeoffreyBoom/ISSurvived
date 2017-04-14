using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TPSInterface : MonoBehaviour
{
    // GameObjects in our scene, they are components of the UI
    Slider healthBar;
    public Slider staminaBar;
    public ThirdPersonCharacter player;
    public Text ammoText;

    // Different variables for the TPSPlayer UI (ammo, health and stamina) 
    // The moveRate and fireRate affect the Player's number of consecutive moves or shots
    int currentHealth, currentStamina, currentAmmo;
    int amount = 3, recharge = 2;
    float nextMove, nextFire, moveRate = 0.1f, fireRate = 0.2f;
    bool running;

    // Initialization of the TPSPlayer UI 
    void Start()
    {
        currentHealth = 100;
        currentStamina = 100;
        currentAmmo = 25;
        setAmmoUI();

        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
    }

    void Update()
    {
        // If the TPSPlayer receives damage, set his health UI
        if (TPSPlayer.setDamageUI == true)
        {
            setHealthUI();
        }

        // If the TPSPlayer is shooting and the time 
        // between his last fire and this one is less then nextFire`
        if (TPSPlayer.isShooting == true && Time.time > nextFire)
        {
            // Here we set a value to nextFire, the time plus the fireRate
            nextFire = Time.time + fireRate;

            // If the Ammo is more than 0, lose Ammo and set the UI accordingly
            if (currentAmmo > 0)
            {
                loseAmmo();
                setAmmoUI();
            }

            // If the Ammo is equal to 0 then Recharge automatically (and set the UI)
            if (currentAmmo == 0)
            {
                setAmmoUI();
                TPSPlayer.Recharge();
                currentAmmo = 25;
                setAmmoUI();
            }
        }

        // If the TPSPlayer presses R, he reloads and the ammoUI is set
        if (Input.GetKey("r"))
        {
            TPSPlayer.Recharge();
            currentAmmo = 25;
            setAmmoUI();
        }

        // If the TPSPlayer presses Left Shift and his stamina bar is over 0 
        // and if the time from his last sprint is superior to nextMove
        if (Input.GetKey(KeyCode.LeftShift) && staminaBar.value > 0 && Time.time > nextMove)
        {
            // The character starts running, set the StaminaUI while he runs
            running = true;
            nextMove = Time.time + moveRate;
            setStaminaUI();
        }

        // If he stops pressing Left Shift, the bool running becomes false
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            running = false;
        }

        // This sets the StaminaUI when the character does not run, it fills back up
        if (staminaBar.value < 100)
        {
            setStaminaUI();
        }
    }

    // Function to set the HealthUI, decrease the currentHealth and update the UI
    private void setHealthUI()
    {
        currentHealth -= amount;
        healthBar.value = currentHealth;
        TPSPlayer.setDamageUI = false;
    }

    // Function to update StaminaUI, if he is running, lower the value, else add a recharge amount to it
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

    // Two functions used to make the TPSPlayer lose ammo and update the UI
    private void loseAmmo()
    {
        currentAmmo -= 1;
    }

    private void setAmmoUI()
    {
        ammoText.text = currentAmmo + "  /  25";
    }
}