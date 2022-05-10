﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************************************************************//
//  File:   EnemiesKilled.cs                                                                                                                 //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Enmey killed class                                                                                                               //
//                                                                                                                                           //
//  Notes:  Deals with enemy death                                                                                                           //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class EnemiesKilled : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//


    [SerializeField] int enemiesKilled = 0;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public Text enemiesKilledText;

    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    private void Update()
    {
        enemiesKilledText.text = enemiesKilled.ToString();
    }

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // GetNumEnemiesKilled                                                                                                                   //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to get the number of enemies the player has killed                                                                           //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public int GetNumEnemiesKilled()
    {
        return enemiesKilled;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // IncreaseCurrentEnemiesKilled                                                                                                          //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to increase the number of enemies the player has killed by 1                                                                 //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//


    public void IncreaseCurrentEnemiesKilled()
    {
        enemiesKilled++;
    }
}