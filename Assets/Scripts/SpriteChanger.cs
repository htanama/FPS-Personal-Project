using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    public static SpriteChanger instance;

    public Image uiImage;          // Assign the Image component in the Inspector
    public Sprite displayWeapon;
    public Sprite weapon1;         // Assign the "weapon1" sprite in the Inspector
    public Sprite weapon2;         // Assign the "weapon2" sprite in the Inspector
    public Sprite weapon3;         // Assign the "weapon3" sprite in the Inspector

    private int currentWeapon = 1; // Tracks the current weapon (1, 2, 3)

    void Start()
    {
        instance = this;

        if (uiImage == null)
        {
            Debug.LogError("UI Image is NOT assigned in the Inspector!");
            return;
        }

        // Set the initial sprite to weapon1
        uiImage.sprite = displayWeapon;
    }

    void Update()
    {
        //Check if the E key is pressed
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SwitchWeapon();
        //}

        //uiImage.sprite = displayWeapon;
        
    }

    public void Initialize(Sprite initialSprite)
    {
        if (initialSprite == null)
        {
            Debug.LogError("initialSprite is null. Ensure a valid sprite is being passed.");
            return;
        }

        displayWeapon = initialSprite;
        Debug.Log($"Sprite assigned: {initialSprite.name}");
        uiImage.sprite = displayWeapon;
    }

    public void SwitchWeapon()
    {
        // Increment the weapon index
        currentWeapon++;
        if (currentWeapon > 3) currentWeapon = 1; // Cycle back to weapon1 after weapon3

        // Use a switch statement to change the sprite
        switch (currentWeapon)
        {
            case 1:

                displayWeapon = weapon1;
                uiImage.sprite = displayWeapon;
                Debug.Log("Switched to Weapon 1");
                break;
            case 2:
                displayWeapon = weapon2;
                uiImage.sprite = displayWeapon;
                Debug.Log("Switched to Weapon 2");
                break;
            case 3:
                displayWeapon = weapon3;
                uiImage.sprite = displayWeapon;
                Debug.Log("Switched to Weapon 3");
                break;
            default:
                Debug.LogError("Invalid weapon index!");
                break;
        }
    }

    public void changeM1Garand(Sprite newSprite)
    {
        //displayWeapon = weapon1;
        displayWeapon = newSprite;
        uiImage.sprite = displayWeapon;
    }

    public void changeM19(Sprite newSprite)
    {
        //displayWeapon = weapon2;
        displayWeapon = newSprite;
        uiImage.sprite = displayWeapon;
    }
    public void changeThomson(Sprite newSprite)
    {
        //displayWeapon = weapon3;
        displayWeapon = newSprite;
        uiImage.sprite = displayWeapon;
    }

    
}
