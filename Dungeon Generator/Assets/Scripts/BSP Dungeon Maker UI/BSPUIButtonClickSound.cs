using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPUIButtonClickSound.cs                                                                                                         //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Button Click Sound Class                                                                                                         //
//                                                                                                                                           //
//  Notes:  Plays a sound when the button is clicked                                                                                         //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class BSPUIButtonClickSound : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public AudioClip buttonClickSound;
    public Camera uiCamera;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // OnButtonClick                                                                                                                         //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to play a sound when the button is clicked                                                                                   //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void OnButtonClick()
    {
        PlayAudioClipAtPoint(uiCamera.transform.position, 0.0f, buttonClickSound);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // PlayAudioClipAtPoint                                                                                                                  //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to play a sound at a specific point in the world                                                                             //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public AudioSource PlayAudioClipAtPoint(Vector3 position, float spatialBlend, AudioClip audioClip)
    {

        GameObject tempAudioClip = new GameObject("TmpAudio");
        tempAudioClip.transform.position = position;
        AudioSource audio_source = tempAudioClip.AddComponent<AudioSource>();
        audio_source.spatialBlend = spatialBlend;         // Set the spatial blend
        audio_source.clip = audioClip;
        audio_source.Play();
        Destroy(tempAudioClip, audioClip.length); // Destroy the game object after clip has finised playing
        return audio_source;
    }

}
