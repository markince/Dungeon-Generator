//*******************************************************************************************************************************************//
//  File:   PathfindingNode.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Pathfinding node class                                                                                                           //
//                                                                                                                                           //
//  Notes:  basic node class for the A* pathfinding algorithm. Nodes have two states, walkable or not walkable. This class Implements the    //
//          heap item interface. Nodes also have a movement penalty.                                                                         //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode : IHeapItem<PathfindingNode> // Implements the heap item interface
{
	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	int pathfindingHeapIndex;

	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//

	public bool            isWalkable;     // Is the node traversable?
	public Vector3         worldLocation;  // Position of the node in the world
	public int             gCost;          // G cost of each node, how far away this node is from the starting node
	public int             hCost;          // Heuristic cost of each node, how far away the node is from the target node
	public int             gridXPos;       // X position of the node in the grid
	public int             gridYPos;       // Y position of the node in the grid
	public PathfindingNode parentNode;     // Parent node of the current node

	public int             gridMovementPenalty; // Movement penalty value of this particular node

	public int fCost // Calculated by adding g cost to the h cost
	{
		get
		{
			return gCost + hCost; // Set f cost
		}
	}

	public int PathfindingHeapIndex // Getters and setters for heap index
	{
		get
		{
			return pathfindingHeapIndex;
		}
		set
		{
			pathfindingHeapIndex = value;
		}
	}

	//***************************************************************************************************************************************//
	//	Constructor                                                                                                                          //
	//***************************************************************************************************************************************//

	public PathfindingNode(bool _isWalkable, Vector3 _worldLocation, int _gridXPos, int _gridYPos, int _gridMovementPenalty)
	{
		// Set variables
		isWalkable          = _isWalkable;             // Set if walkable or not
		worldLocation       = _worldLocation;          // Set position in the world
		gridXPos            = _gridXPos;               // Set X position on the grid
		gridYPos            = _gridYPos;               // Set Y position on the grid
		gridMovementPenalty = _gridMovementPenalty;    // Set the nodes movement penalty on the grid

	} // End of Constructor

	//***************************************************************************************************************************************//
	//	CompareTo Function                                                                                                                   //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CompareTo                                                                                                                             //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that takes in a node and compares it to another node                                                                         //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	public int CompareTo(PathfindingNode pathfindingNodeToCompare)
	{
		// Compare the fcosts of the two nodes
		// temp int to compare value
		int comparison = fCost.CompareTo(pathfindingNodeToCompare.fCost);

		if (comparison == 0) // Are the two fcosts equal?
		{
			comparison = hCost.CompareTo(pathfindingNodeToCompare.hCost);
		}

		// Return 1 if the integer is higher
		return -comparison;

	} // End of CompareTo funtion

} // End of PathfindingNode class
