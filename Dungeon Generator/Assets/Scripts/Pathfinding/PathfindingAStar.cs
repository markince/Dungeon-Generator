using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System;

//*******************************************************************************************************************************************//
//  File:   PathfindingAStar.cs                                                                                                              //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Main pathfinding class                                                                                                           //
//                                                                                                                                           //
//  Notes:  Class that is used in the enemy class to calculcate and the path for the enemies to follow the player                            //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class PathfindingAStar : MonoBehaviour
{
	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	PathfindingGrid pathfindingGrid; // reference to the pathfinding grid

	//***************************************************************************************************************************************//
	//	Awake Function - Called once when program is first started                                                                           //
	//***************************************************************************************************************************************//

	void Awake()
	{
		pathfindingGrid = GetComponent<PathfindingGrid>(); // Get the component of the grid
	}

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// FindValidPath                                                                                                                         //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to find a valid path between the enemy and the player. it uses the A* algorith using the G cost, h cost and f cost (g + h)   //
	// It also uses a heap to add, remove and check items on the lists.                                                                      //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void FindValidPath(RequestPath pathRequest, Action<ResultOfPath> callback)
	{

		Vector3[] pathWaypoints = new Vector3[0]; // Array of waypoints for the path

		bool isPathFoundSucessfully = false; // Was the path a success?

		// Get the start and target nodes using the NodeFromWorldPoint function
		PathfindingNode pathStartNode  = pathfindingGrid.CalculateNodeFromAWorldPoint(pathRequest.startPosOfPath);
		PathfindingNode pathEndNode    = pathfindingGrid.CalculateNodeFromAWorldPoint(pathRequest.endPosOfPath);

		// Set parent node
		pathStartNode.parentNode = pathStartNode;

		// Is the start node and target nodes walkable?
		if (pathStartNode.isWalkable && pathEndNode.isWalkable)
		{
			// Create open list used in the algorithm
			PathfindingHeap<PathfindingNode> openHeapList = new PathfindingHeap<PathfindingNode>(pathfindingGrid.maximumSizeOfGrid);

			// Create closed list used in the algorith
			// A hashset is used for improved speed performance
			HashSet<PathfindingNode> closedHeapList = new HashSet<PathfindingNode>();

			// Add startnode to open list heap
			openHeapList.AddToHeap(pathStartNode);

			// Loop through the open list
			while (openHeapList.Count > 0)
			{
				// Remove first item from heap
				PathfindingNode currentNode = openHeapList.RemoveFirstItemFromHeap();

				// Add current node to closed list
				closedHeapList.Add(currentNode); 

				// If path has been found?
				if (currentNode == pathEndNode)
				{
					isPathFoundSucessfully = true;
					break; // Return
				}

				// foreach neighbour of the current node
				foreach (PathfindingNode neighbouringNode in pathfindingGrid.GetNeighboursOfNode(currentNode))
				{
					// Is the neighbour not traversable or is it on the closed list?
					if (!neighbouringNode.isWalkable || closedHeapList.Contains(neighbouringNode))
					{
						continue; // Skip to the next neighbour
					}

					// Calculate the movment cost to the neighbour. Also adds on the movement penalty
					int newMovementValue = currentNode.gCost + GetDistanceBetweenTwoNodes(currentNode, neighbouringNode) + neighbouringNode.gridMovementPenalty;

					// Check if the new path to the neighbour is shorter OR
					// Check if neighbour is not in the open list
					if (newMovementValue < neighbouringNode.gCost || !openHeapList.IfHeapContains(neighbouringNode))
					{
						// Set gcost  of neighbour
						neighbouringNode.gCost = newMovementValue;

						// Calculate the f cost of the neighbour by getting the distance between the neighbour node and the target node
						neighbouringNode.hCost = GetDistanceBetweenTwoNodes(neighbouringNode, pathEndNode);

						// Set the parent of the neighbour to the current node
						neighbouringNode.parentNode = currentNode;

						// if the neighbour is not in the open list
						if (!openHeapList.IfHeapContains(neighbouringNode))
                        {
							// Add neighbour to open list
							openHeapList.AddToHeap(neighbouringNode);         
						}
						else
                        {
							// Already in open set so update its value
							openHeapList.UpdateItemInHeap(neighbouringNode); 
						}
							
					}
				}
			}
		}
		
		if (isPathFoundSucessfully) // Has a successful path been found?
		{
			pathWaypoints = RetracePath(pathStartNode, pathEndNode);

			isPathFoundSucessfully = pathWaypoints.Length > 0;
		}

		callback(new ResultOfPath(pathWaypoints, isPathFoundSucessfully, pathRequest.callback));

	} // End of FindValidPath function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	//  RetracePath                                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//  Function that retraces the steps from the start node to the end node.                                                                //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	Vector3[] RetracePath(PathfindingNode startNode, PathfindingNode endNode)
	{

		// Create new path to store retraced version
		List<PathfindingNode> validPath = new List<PathfindingNode>();

		// set current node to end node
		PathfindingNode currentNode = endNode;

		// Loop through nodes
		while (currentNode != startNode)
		{
			// Add nodes
			validPath.Add(currentNode);

			// set to new parent node
			currentNode = currentNode.parentNode;
		}

		// Simplfy the path so that it only includes the waypoints where the path changes direction
		Vector3[] pathWaypoints = CleanUpPath(validPath);

		// Reverse the path
		Array.Reverse(pathWaypoints);

		return pathWaypoints;

	} // End of RetracePath path

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CleanUpPath                                                                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that simplfies a path so that the waypoints are only placed along the path where the path changes direction                  //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	Vector3[] CleanUpPath(List<PathfindingNode> currentPath)
	{
		// Create waypoint path
		List<Vector3> waypoints = new List<Vector3>();

		// Create a varibale to store the direction of the last two nodes
		Vector2 oldDirectionOfNode = Vector2.zero;

		for (int i = 1; i < currentPath.Count; i++) // Count all nodes through the path
		{
			// Set the new direction
			Vector2 newDirectionOfNode = new Vector2(currentPath[i - 1].gridXPos - currentPath[i].gridXPos, currentPath[i - 1].gridYPos - currentPath[i].gridYPos);

			// If they are not the same then the path has changed direction
			if (newDirectionOfNode != oldDirectionOfNode)
			{
				// Add new waypoint
				waypoints.Add(currentPath[i].worldLocation);
			}

			// set old direction to new one
			oldDirectionOfNode = newDirectionOfNode;
		}

		// Return the waypoints
		return waypoints.ToArray();


	} // End of CleanUpPath function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GetDistanceBetweenTwoNodes                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to calculate distance between two nodes. It starts by counting on the x axis how far away we are from the target node. It    //
	// then does the same on the Y axis. It then takes the lowest number and that gives us how many diaganal moves it takes to reach the     //
	// target node. It then calculates how many vertical or horizontal moves we need by subtracting the lower number by the higher number.   //
	// Final equation is 14y + 10(x-y) only in the case that x is greater than y else it would be 14x + 10(y - x). This gives us the         //
	// total distance.                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	int GetDistanceBetweenTwoNodes(PathfindingNode node1, PathfindingNode node2)
	{
		// Calculate distnace x
		int distanceX = Mathf.Abs(node1.gridXPos - node2.gridXPos);

		// Calculate distance y
		int distanceY = Mathf.Abs(node1.gridYPos - node2.gridYPos);

		// Do check
		if (distanceX > distanceY)
        {
			// return 14y + 10(x-y)
			return 14 * distanceY + 10 * (distanceX - distanceY);
		}

		// else return 14x + 10(y - x)
		return 14 * distanceX + 10 * (distanceY - distanceX);

	} //  End of GetDistanceBetweenTwoNodes function


} // End of PathfindingAStar class
