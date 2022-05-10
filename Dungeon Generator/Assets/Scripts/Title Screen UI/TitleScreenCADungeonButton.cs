using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   TitleScreenBSPDungeonButton.cs                                                                                                   //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Title screen CA dungeon buttton class                                                                                            //
//                                                                                                                                           //
//  Notes:  Called when the user selects to create a CA dungeon on the title screen                                                          //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class TitleScreenCADungeonButton : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    [SerializeField] public AudioClip clip;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // LoadCAPDungeonScene                                                                                                                   //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //  Function to load the CA scene                                                                                                        //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//


    public void LoadCAPDungeonScene()
    {
        SceneManager.LoadScene(2);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // PlayAudioClipAtPoint                                                                                                                  //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that plays a scount at a particular point in the world                                                                       //
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

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // playButtonClickSFX                                                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to play the sound effect when a button is clicked                                                                            //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void playButtonClickSFX()
    {
        PlayAudioClipAtPoint(Camera.main.transform.position, 0.0F, clip);
    }
}
