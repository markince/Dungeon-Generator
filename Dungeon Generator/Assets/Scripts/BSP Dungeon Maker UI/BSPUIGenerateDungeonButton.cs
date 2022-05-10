using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPUIGenerateDungeonButton.cs                                                                                                    //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Generate Dungeon Button Class                                                                                                    //
//                                                                                                                                           //
//  Notes:  Action performed when generation dungeon button is clicked                                                                       //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class BSPUIGenerateDungeonButton : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public BSPDungeonMaker dungeonMaker;
    public GameObject      generateDungeonButton;
    public GameObject      playButton;
    public GameObject      pleaseWaitButton;
    public float           startDelay = 0.5f;
    public float           interval = 1f;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // GenerateDungeon                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that generates a new dungeon. This is called when the generate dungeon button is clicked                                     //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//


    public void GenerateDungeon()
    {
        generateDungeonButton.SetActive(false);
        playButton.transform.position = new Vector3(2000, 2000, 0);
        pleaseWaitButton.SetActive(true);
        dungeonMaker.CreateNewDungeon();

    }

}
