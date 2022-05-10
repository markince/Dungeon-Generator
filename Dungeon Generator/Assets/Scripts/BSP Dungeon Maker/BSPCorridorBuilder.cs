using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPCorridorBuilder.cs                                                                                                            //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Dungeon Corridor Builder Class                                                                                               //
//                                                                                                                                           //
//  Notes:  Class to build the cooridors that connect the rooms together. This was originally an option in the inspector but has now been    //
//          modified so that the user can select the width of the corridor. The current range is 3 - 5 units. This may be altered at a       //
//          later date to stop items and enemies spawning in the corridor and potentionally blocking pathfinding and the player unable to    //
//          traverse through the level via the corridors                                                                                     //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPCorridorBuilder
{
    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateCorridor                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to create a corridor that connects two rooms together. User can specify the width of the corridor in the UI.                 //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public List<BSPNode> CreateCorridor(List<BSPRoomNode> roomsList, int widthOfCorridor)
    {
        List<BSPNode> listOfCorridors = new List<BSPNode>();

        // Create a queue of nodes starting with the lowest index gradually moving up
        Queue<BSPRoomNode> areasToCheck = new Queue<BSPRoomNode>(roomsList.OrderByDescending(node => node.treeLayerValue).ToList());

        while (areasToCheck.Count > 0)
        {
            var node = areasToCheck.Dequeue();

            if (node.ListOfChildren.Count == 0) // Does the node have children?
            {
                continue; // Cant create a corridor between nothing
            }

            // if true, create a new corridor node
            // Pass in children nodes and the width of the cooridor as set by the user
            BSPCorridorNode corridorNode = new BSPCorridorNode(node.ListOfChildren[0], node.ListOfChildren[1], widthOfCorridor);
            
            // Add newly create corridor to list
            listOfCorridors.Add(corridorNode);

        } // End of while loop

        return listOfCorridors; // Return list

    } // End of CreateCorridor function

} // End of CorridorBuilder class
