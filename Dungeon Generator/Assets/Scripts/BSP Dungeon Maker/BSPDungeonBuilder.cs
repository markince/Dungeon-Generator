using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPDungeonBuilder.cs                                                                                                             //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Dungeon Builder Class                                                                                                        //
//                                                                                                                                           //
//  Notes:  This class processes the dungeon using the other classes. it will first build the BSP tree and then generate the rooms and       //
//          generate the rooms and corridors from this tree.                                                                                 //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPDungeonBuilder
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    List<BSPRoomNode> listOfFinalRooms = new List<BSPRoomNode>(); // Final list of rooms that have been generated

    private int widthOfDungeon  = 0; // Total width size of the dungeon
    private int lengthOfDungeon = 0; // Total length size of the dungeon

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                          //
    //***************************************************************************************************************************************//

    public BSPDungeonBuilder(int widthOfDungeon, int lengthOfDungeon)
    {
        this.widthOfDungeon  = widthOfDungeon;  // Set size of dungeon
        this.lengthOfDungeon = lengthOfDungeon;
    }

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // ProcessDungeon                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Process the dungeon. Builds the tree and then calcualtes the rooms spaces. Builds the rooms in the spaces and then generates          //
    // the corridors and connects them all together.                                                                                          //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    public List<BSPNode> ProcessDungeon(int   maxNumOfIterations, int minWidthOfRoom, int minLengthOfRoom, float bottomCornerModifier, 
                                        float topCornerModifier,  int roomOffset,     int widthOfCorridor)
    {
        BSP binarySpacePartitioner = new BSP(widthOfDungeon, lengthOfDungeon);

        // prepare the 
        listOfFinalRooms = binarySpacePartitioner.PrepareNodes(maxNumOfIterations, minWidthOfRoom, minLengthOfRoom);
        
        List<BSPNode> roomSpaces = BSPDungeonHelper.NavigateBPSGraphToFindLowestNodes(binarySpacePartitioner.RootNode);

        BSPRoomBuilder roomBuilder = new BSPRoomBuilder(maxNumOfIterations, minLengthOfRoom, minWidthOfRoom);

        List<BSPRoomNode> roomList = roomBuilder.CreateRoomsinAreas(roomSpaces, bottomCornerModifier, topCornerModifier, roomOffset);

        BSPCorridorBuilder corridorBuilder = new BSPCorridorBuilder();

        var listOfCorridors = corridorBuilder.CreateCorridor(listOfFinalRooms, widthOfCorridor);

        return new List<BSPNode>(roomList).Concat(listOfCorridors).ToList();

    } // End of ProcessDungeon function

} // End of DungeonBuilder class
