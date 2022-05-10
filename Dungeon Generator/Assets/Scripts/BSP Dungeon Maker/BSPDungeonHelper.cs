using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPDungeonHelper.cs                                                                                                              //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Dungeon Helper Class                                                                                                         //
//                                                                                                                                           //
//  Notes:  Helper class. This is used in the builder class to traverse the graph and find nodes, and to create the corner points of the     //
//          rooms.                                                                                                                           //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPDungeonHelper
{
    //***************************************************************************************************************************************//
    //	Static Class functions                                                                                                               //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // NavigateBPSGraphToFindLowestNodes                                                                                                     //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function used to traverse the BPS graph and find the lowests nodes from a particular parent node. Nodes that are to be examined are   //
    // stored in a queue and the final list of nodes are stored in a list of Nodes.                                                          //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public static List<BSPNode> NavigateBPSGraphToFindLowestNodes(BSPNode parentNode)
    {

        Queue<BSPNode> nodesToExamine = new Queue<BSPNode>(); // Queue of nodes to examine
        List<BSPNode> finalListOfNodes = new List<BSPNode>(); // list of final lowest nodes

        if (parentNode.ListOfChildren.Count == 0) // Does the node not have any children?
        {
            return new List<BSPNode>() // Return empty list
            { 
                parentNode 
            };
        }

        foreach (var node in parentNode.ListOfChildren)
        {
            nodesToExamine.Enqueue(node); // Add children nodes to list
        }

        while (nodesToExamine.Count > 0) 
        {
            var node = nodesToExamine.Dequeue(); // remove node from queue

            if (node.ListOfChildren.Count == 0) // No children?
            {
                finalListOfNodes.Add(node); // add the node to list
            }
            else
            {
                foreach (var childNode in node.ListOfChildren) 
                {
                    nodesToExamine.Enqueue(childNode); // add all children nodes to list
                }
            }
        }

        return finalListOfNodes; // return list

    } // End of ExamineBSPGraphToFindLowestNodes function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateBottomLeftCornerPoint                                                                                                           //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to create the bottom left corner point of the regions seperated by the algorithm. One room will be created in each           //
    // region                                                                                                                                //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public static Vector2Int CreateBottomLeftCornerPoint(Vector2Int leftPointLimit, Vector2Int rightPointLimit,
                                                         float      pointModifier,  int        pointOffset)
    {
        // Calcualte minimum points use for region creation
        int minimumX = leftPointLimit.x  + pointOffset;
        int maximumX = rightPointLimit.x - pointOffset;
        int minimumY = leftPointLimit.y  + pointOffset;
        int maximumY = rightPointLimit.y - pointOffset;

        // Randomly generate points in that region where the room corners of the room will be created. This makes all the rooms a random size
        int xPointToReturn = Random.Range(minimumX, (int)(minimumX + (maximumX - minimumX) * pointModifier));
        int yPointToReturn = Random.Range(minimumY, (int)(minimumY + (minimumY - minimumY) * pointModifier));

        // Calculate bottom left point to return
        Vector2Int pointToReturn = new Vector2Int(xPointToReturn, yPointToReturn);

        // Return final point
        return pointToReturn;

    } // End of CreateBottomLeftCornerPoint function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateTopRightCornerPoint                                                                                                             //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to create the Top right corner point of the regions seperated by the algorithm. One room will be created in each             //
    // region                                                                                                                                //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public static Vector2Int CreateTopRightCornerPoint(Vector2Int leftPointLimit, Vector2Int rightPointLimit,
                                                         float pointModifier, int pointOffset)
    {
        // Calcualte minimum points use for region creation
        int minimumX = leftPointLimit.x  + pointOffset;
        int maximumX = rightPointLimit.x - pointOffset;
        int minimumY = leftPointLimit.y  + pointOffset;
        int maximumY = rightPointLimit.y - pointOffset;

        // Randomly generate points in that region where the room corners of the room will be created. This makes all the rooms a random size
        int xPointToReturn = Random.Range((int)(minimumX + (maximumX - minimumX) * pointModifier), maximumX);
        int yPointToReturn = Random.Range((int)(minimumY + (maximumY - minimumY) * pointModifier), maximumY);

        // Calculate top right point to return
        Vector2Int pointToReturn = new Vector2Int(xPointToReturn, yPointToReturn);

        // Return final point
        return pointToReturn;

    } // End of CreateTopRightCornerPoint function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CalculateMiddlePoint                                                                                                                  //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to calcualte the middle point of a region created by the BSP algorithm.                                                       //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public static Vector2Int CalculateMiddlePoint(Vector2Int vector1, Vector2Int vector2)
    {
        Vector2 total = vector1 + vector2;
        Vector2 tempVector = total / 2;

        Vector2Int vectorToReturn = new Vector2Int((int)tempVector.x, (int)tempVector.y);

        return vectorToReturn;

    } // End of CalculateMiddlePoint Function

} // End of DungeonHelper class

//***************************************************************************************************************************************//
//	Enums                                                                                                                                //
//***************************************************************************************************************************************//

public enum CorrespondingPosition // Corresponding Position of corridor connected to a room
{
    Up,
    Down,
    Right,
    Left
}
