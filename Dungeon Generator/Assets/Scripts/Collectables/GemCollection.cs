using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************************************************************//
//  File:   GemCollection.cs                                                                                                                 //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Gem collection class                                                                                                             //
//                                                                                                                                           //
//  Notes:  Modifies the player number of gems when they collect an gem item                                                                 //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class GemCollection : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public AudioSource audioSource;
    public AudioClip collectGemSFX;
    bool collected = false;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // OnTriggerEnter                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to check for collision with the player. The number of gems collected of the player is then increased.                        //
    // Player must collect all the gems to complete the level.                                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//


    private void OnTriggerEnter(Collider other)
    {
        print("TEST");

        if (other.gameObject.tag == "Player")
        {

            if (!collected)
            {
                FindObjectOfType<Gems>().IncreaseCurrentGems(1);
                collected = true;
            }


            // Play Sound effect
            audioSource.PlayOneShot(collectGemSFX);

            Destroy(gameObject, 0.5f);
        }


    }
}
