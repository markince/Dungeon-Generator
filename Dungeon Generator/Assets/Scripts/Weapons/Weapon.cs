using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   Weapon.cs                                                                                                                        //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Weapon class                                                                                                                     //
//                                                                                                                                           //
//  Notes:  Deals with the staff of pain weapon                                                                                              //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class Weapon : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Constant Variables                                                                                                                   //
    //***************************************************************************************************************************************//

    const int projectileFiringPeriod = 2;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public AudioSource audioSource;
    public AudioClip shoot1;

    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    [SerializeField] Camera firstPersonCamera = null;
    [SerializeField] GameObject fireballPrefab = null;
    [SerializeField] GameObject fireballStartPosition = null;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject player;
    [SerializeField] ParticleSystem staffFireFlash = null;
    GameObject fireBall;
    [SerializeField] Ammo ammoSlot = null;

    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    void Update()
    {
        // Has the player pressed fire?
        if (Input.GetButtonDown("Fire1"))
        {
            // Check if enough ammo
            if (ammoSlot.GetCurrentAmmout() > 0)
            {
                // Particle effect
                staffFireFlash.Play();

                // Create a fireball at the top position of the staff
                fireBall = Instantiate(fireballPrefab, fireballStartPosition.transform.position, Quaternion.identity) as GameObject;

                // move the fireball in the direction the camera is facing
                Vector3 dir = firstPersonCamera.transform.forward;
                fireBall.GetComponent<Rigidbody>().velocity = projectileSpeed * dir * Time.deltaTime * 100;

                // Play Sound effect
                audioSource.PlayOneShot(shoot1);

                ammoSlot.ReduceCurrentAmmo();

            }
        }
    }

} // End of Weapon class
