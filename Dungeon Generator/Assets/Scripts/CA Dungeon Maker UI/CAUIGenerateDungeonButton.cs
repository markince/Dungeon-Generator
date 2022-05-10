using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   CAUIGenerateDungeonButton.cs                                                                                                     //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   CA UI Generate Dungeon Class                                                                                                     //
//                                                                                                                                           //
//  Notes:  Used to generate a new dungeon when the generate dungeon button is clicked                                                      //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class CAUIGenerateDungeonButton : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public CADungeonMaker dungeonMaker;

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // GenerateDungeon                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function is called to generate a new dungeon when the generate dungeon button is clicked                                              //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void GenerateDungeon()
    {
        dungeonMaker.CreateNewDungeonMap();

        Vector3 planeSize = new Vector3(dungeonMaker.dungeonWidth / 10.0f, 0.1f, dungeonMaker.dungeonLength / 10.0f);

        dungeonMaker.floor.transform.localScale = planeSize;
        dungeonMaker.celing.transform.localScale = planeSize;

    }

}
