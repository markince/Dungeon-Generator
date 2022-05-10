using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSP.cs                                                                                                                           //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Binary Space Partitioner class                                                                                                   //
//                                                                                                                                           //
//  Notes:  The algorithm will take a space and draws a line to partition that space into two spaces of smaller size. The spaces are         //
//          further divided until they can no longer be split. A tree like structure is then created that stores the parent/child            //
//          relationships.                                                                                                                   //
//                                                                                                                                           //
//          Main steps of the BSP Algorithm:                                                                                                 //
//                                                                                                                                           //
//          1. Define a root space.                                                                                                          //
//          2. Divide the root space using a vertical or horizontal line at a random point (using the minimum room size as a guide).         //
//          3. Add them to a tree structure.                                                                                                 //
//          4. Check if the new spaces can be divided further.                                                                               //
//          5. Repeat steps 2 and 3 until no more spaces can be divided.                                                                     //
//          6. For each space that has been created, create a room inside that space using random corner points within the space boundaries. //
//          7. Traversing the tree starting from the youngest tree branches, draw corridoes between the nodes of the same parent.            //
//          8. Go up a layer of the tree and repeat step 7 until all corridors are implemented.                                              //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSP
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    BSPRoomNode rootNode; // First node in the tree

    public BSPRoomNode RootNode
    { 
        get => rootNode; 
    }

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                          //
    //***************************************************************************************************************************************//

    public BSP(int widthOfDungeon, int lengthOfDungeon)
    {
        Vector2Int origion     = new Vector2Int(0, 0); // Set origion of the dungeon
        Vector2Int dungeonSize = new Vector2Int(widthOfDungeon, lengthOfDungeon); // Size of dungeon

        this.rootNode = new BSPRoomNode(origion, dungeonSize, null, 0); // Create starter room

    } // End of BSP Constructor

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // PrepareNodes                                                                                                                          //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that prepares the nodes in the BPS graph                                                                                     //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public List<BSPRoomNode> PrepareNodes(int maximumCount, int minimumRoomWidth, int minimumRoomLength)
    {
        Queue<BSPRoomNode> bspGraph = new Queue<BSPRoomNode>(); // Create graph (a Queue of rooms)

        List<BSPRoomNode> listOfRooms = new List<BSPRoomNode>(); // Final list of rooms

        bspGraph.Enqueue(this.rootNode); // Add rootnode to graph

        listOfRooms.Add(this.rootNode); // Add root node to final list

        int count = 0; // Start counter

        while (count < maximumCount && bspGraph.Count > 0)
        {
            count++; // Increment count
            BSPRoomNode currentNode = bspGraph.Dequeue(); // remove item from queue

            if (currentNode.roomWidth >= minimumRoomWidth * 2 || currentNode.roomLength >= minimumRoomLength * 2) // Check size
            {
                SplitArea(currentNode, listOfRooms, minimumRoomLength, minimumRoomWidth, bspGraph); // Split the area into 2 smaller areas
            }
        }

        return listOfRooms; // Return final list

    } // End of PrepareNodes function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // SplitArea                                                                                                                             //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to divide the space using a vertical or horizontal line at a random point.                                                   //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//


    private void SplitArea(BSPRoomNode currentRoom, List<BSPRoomNode> listOfRooms, int minimumRoomLength, int minimumRoomWidth, Queue<BSPRoomNode> bspGraph)
    {
        // Create the line
        BSPLine line = CalculateLineSpace(currentRoom.bottomLeftRegionCorner, currentRoom.topRightRegionCorner, minimumRoomWidth, minimumRoomLength);

        BSPRoomNode room1;
        BSPRoomNode room2;

        if (line.Alignment == Alignment.Horizontal) // Horizontal line?
        {
            // Create first room from the space given
            room1 = new BSPRoomNode(currentRoom.bottomLeftRegionCorner,
                    new Vector2Int(currentRoom.topRightRegionCorner.x, line.Coordinates.y),
                    currentRoom,
                    currentRoom.treeLayerValue + 1);

            // Create second room from the space given
            room2 = new BSPRoomNode(new Vector2Int(currentRoom.bottomLeftRegionCorner.x, line.Coordinates.y),
                    currentRoom.topRightRegionCorner,
                    currentRoom,
                    currentRoom.treeLayerValue + 1);
        }
        else // Vertical line?
        {
            // Create first room from the space given
            room1 = new BSPRoomNode(currentRoom.bottomLeftRegionCorner,
                    new Vector2Int(line.Coordinates.x, currentRoom.topRightRegionCorner.y),
                    currentRoom,
                    currentRoom.treeLayerValue + 1);

            // Create second room from the space given
            room2 = new BSPRoomNode(new Vector2Int(line.Coordinates.x, currentRoom.bottomLeftRegionCorner.y),
                    currentRoom.topRightRegionCorner,
                    currentRoom,
                    currentRoom.treeLayerValue + 1);
        }

        // Add newly created rooms to list
        AddNewRoomToList(listOfRooms, bspGraph, room1);
        AddNewRoomToList(listOfRooms, bspGraph, room2);

    } // End of SplitArea function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // AddNewRoomToList                                                                                                                      //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to add the new rooms created from the space to a list that can be used elswhere in the program                               //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void AddNewRoomToList(List<BSPRoomNode> listToReturn, Queue<BSPRoomNode> bspGraph, BSPRoomNode room)
    {
        listToReturn.Add(room); // Add to list of rooms
        bspGraph.Enqueue(room); // Add to graph queue

    }  // End of AddNewRoomToList function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CalculateLineSpace                                                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function used to calculate the line that is used to split up an area into two smaller areas so rooms can be created                   //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private BSPLine CalculateLineSpace(Vector2Int bottomLeftRegionCorner, Vector2Int topRightRegionCorner, int minimumRoomWidth, int minimumRoomLength)
    {
        Alignment alignment;

        // Booleans used to check length and width status
        bool length = (topRightRegionCorner.y - bottomLeftRegionCorner.y) >= 2 * minimumRoomLength;
        bool width  = (topRightRegionCorner.x - bottomLeftRegionCorner.x) >= 2 * minimumRoomWidth;

        if (length && width)
        {
            alignment = (Alignment)(Random.Range(0, 2));
        }
        else if (width)
        {
            alignment = Alignment.Vertical;
        }
        else
        {
            alignment = Alignment.Horizontal;
        }

        return new BSPLine(alignment, CalculateCoordinatesForDirection(alignment,        bottomLeftRegionCorner, topRightRegionCorner, 
                                                                       minimumRoomWidth, minimumRoomLength));

    } // End of CalculateLineSpace function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CalculateCoordinatesForDirection                                                                                                      //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to calculate the coordinates for the direction the line is to be created.                                                                                                                                      //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private Vector2Int CalculateCoordinatesForDirection(Alignment alignment, Vector2Int bottomLeftRegionCorner, Vector2Int topRightRegionCorner, 
                                                        int minimumRoomWidth, int minimumRoomLength)
    {
        Vector2Int location = new Vector2Int(0, 0);

        if (alignment == Alignment.Horizontal) // Horizontal line?
        {
            location = new Vector2Int(0, Random.Range((bottomLeftRegionCorner.y + minimumRoomLength), (topRightRegionCorner.y - minimumRoomLength)));
        }
        else // Vertical line?
        {
            location = new Vector2Int(Random.Range((bottomLeftRegionCorner.x + minimumRoomWidth), (topRightRegionCorner.x - minimumRoomWidth)), 0);
        }

        return location;

    } // End of CalculateCoordinatesForDirection function

} // End of BSP Class

