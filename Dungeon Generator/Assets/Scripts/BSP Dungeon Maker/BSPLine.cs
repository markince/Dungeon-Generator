using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPLine.cs                                                                                                                       //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Dungeon Line Class                                                                                                           //
//                                                                                                                                           //
//  Notes:  Line class. This is used as a vertical or horizontal line at a random point in a space to split it into two spaces to create     //
//          new rooms.                                                                                                                       //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPLine
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    Alignment  alignment;   // Alignment of the line
    Vector2Int coordinates; // Location of the line

    public Alignment Alignment 
    {
        get => alignment;
        set => alignment = value;
    }

    public Vector2Int Coordinates
    {
        get => coordinates;
        set => coordinates = value;
    }

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                          //
    //***************************************************************************************************************************************//
    public BSPLine(Alignment alignment, Vector2Int coordinates)
    {
        // Set alignment and location
        this.alignment   = alignment;
        this.coordinates = coordinates;
    }

} // End of Line Class

//***************************************************************************************************************************************//
//	Enums                                                                                                                                //
//***************************************************************************************************************************************//

public enum Alignment // Horizonal or vertical?
{
    Horizontal = 0,
    Vertical = 1

} // End of Alignment Enum

