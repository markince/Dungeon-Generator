using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

//*******************************************************************************************************************************************//
//  File:   PlayerSound.cs                                                                                                                   //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Player sound class                                                                                                               //
//                                                                                                                                           //
//  Notes:  Actions walking and running sounds for the player                                                                                //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class PlayerSound : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public AudioSource audioSource;
    public AudioClip walkSound1;

    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S ) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            // ... and if the footsteps are not playing...
            if (!audioSource.isPlaying)
            {
                audioSource.clip = walkSound1;
                audioSource.Play();

            }
        }
        else
        {
            // Otherwise stop the footsteps.
            audioSource.Stop();
        }
    }
}



