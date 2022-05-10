using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************************************************************//
//  File:   SceneLoaderLv2.cs                                                                                                                //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Scene loader for CA scene                                                                                                        //
//                                                                                                                                           //
//  Notes:  Loads the relevent scene when the F1 or F2 buttons are pressed while in game mode                                                //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class SceneLoaderLv2 : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Load CA scene

            SceneManager.LoadScene(2);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            // Load front title scene
            SceneManager.LoadScene(0);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }

    }

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    //  ReloadGame                                                                                                                           //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //  Function to reload the CA scene                                                                                                      //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void ReloadGame()
    {
        // Check which scene is currenty loaded and load the relevent scene
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "BSP Dungeon Maker")
        {
            SceneManager.LoadScene(1);

        }
        else if (sceneName == "Cellular Automata Dungeon Maker")
        {
            SceneManager.LoadScene(2);

        }

        Time.timeScale = 1;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    //  ReturnToMainMenu                                                                                                                     //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //  Function that returns the user to the front scene                                                                                    //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    //  QuitGame                                                                                                                             //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //  Function that is called when the player quits the X button                                                                           //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void QuitGame()
    {
        Application.Quit();
    }

}
