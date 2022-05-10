﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPRoomBuilder.cs                                                                                                                //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Dungeon Room Builder Class                                                                                                   //
//                                                                                                                                           //
//  Notes:  Class that builds the rooms in the spaces created by the algorithm                                                               //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPRoomBuilder
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private int maxNumOfRooms;     // Maximum number of rooms that can be generated in the dungeon
    private int minLengthOfRoom;   // Minimum number of rooms that can be generated in the dungeon
    private int minWidthOfRoom;    // minimum width of the rooms

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                          //
    //***************************************************************************************************************************************//
    public BSPRoomBuilder(int maxNumOfRooms, int minLengthOfRoom, int minWidthOfRoom)
    {
        // Set values
        this.maxNumOfRooms   = maxNumOfRooms;
        this.minLengthOfRoom = minLengthOfRoom;
        this.minWidthOfRoom  = minWidthOfRoom;
    }

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateRoomsinAreas                                                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to create a room in the space generated by the algorithm. All spaces will be filled with a single room.                      //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public List<BSPRoomNode> CreateRoomsinAreas(List<BSPNode> roomAreas, float roomBottomCornerConverter, float roomTopCornerConverter, int roomOffset)
    {
        // Create new list to store room data
        List<BSPRoomNode> listOfRooms = new List<BSPRoomNode>();

        foreach (var area in roomAreas)
        {
            // Create 2 points to in the space to create the room
            Vector2Int bottomLeftPosition = BSPDungeonHelper.CreateBottomLeftCornerPoint (area.bottomLeftRegionCorner, area.topRightRegionCorner, 
                                                                                          roomBottomCornerConverter,   roomOffset);
            Vector2Int topRightPosition   = BSPDungeonHelper.CreateTopRightCornerPoint   (area.bottomLeftRegionCorner, area.topRightRegionCorner, 
                                                                                          roomTopCornerConverter,      roomOffset);

            // Set positions
            area.bottomLeftRegionCorner  = bottomLeftPosition;
            area.topRightRegionCorner    = topRightPosition;
            area.bottomRightRegionCorner = new Vector2Int(topRightPosition.x, bottomLeftPosition.y);
            area.topLeftRegionCorner     = new Vector2Int(bottomLeftPosition.x, topRightPosition.y);

            // Add the generated room data to the list
            listOfRooms.Add((BSPRoomNode)area);
        }

        // Return the list
        return listOfRooms;

    } // End of CreateRoomsinAreas

} // End of RoomBuilder class