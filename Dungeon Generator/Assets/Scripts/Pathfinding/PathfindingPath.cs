//*******************************************************************************************************************************************//
//  File:   PathfindingPath.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Pathfinding path class                                                                                                           //
//                                                                                                                                           //
//  Notes:  This class creates the final path by using the Pathfindingline class to calculate the final smoothed path                        //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingPath
{
	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//

	public readonly Vector3[]         lookPositions;
	public readonly int               indexOfFinalFinishLine;
	public readonly int               slowMovementIndex;
	public readonly PathfindingLine[] turnBoundaries;

	//***************************************************************************************************************************************//
	//	Constructor                                                                                                                          //
	//***************************************************************************************************************************************//

	public PathfindingPath(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
	{
		// set lookpoints
		lookPositions = waypoints;
		
		// setup turn boundaries
		turnBoundaries = new PathfindingLine[lookPositions.Length];
		
		// Calculate finish line
		indexOfFinalFinishLine = turnBoundaries.Length - 1;

		// Set previous point
		Vector2 previousPoint = ConvertVector3ToVector2(startPos);

		// Loop through the points
		for (int i = 0; i < lookPositions.Length; i++)
		{
			// calculate current point
			Vector2 currentPoint = ConvertVector3ToVector2(lookPositions[i]);

			// Calculate direction to the current point
			Vector2 directionToCurrentPoint = (currentPoint - previousPoint).normalized;

			Vector2 pointOfTurnBoundary = new Vector2();

			// Calculate turn boundary point
			if (i == indexOfFinalFinishLine)
            {
				pointOfTurnBoundary = currentPoint;

			}
			else
            {
				pointOfTurnBoundary = currentPoint - directionToCurrentPoint * turnDst;
			}

			turnBoundaries[i] = new PathfindingLine(pointOfTurnBoundary, previousPoint - directionToCurrentPoint * turnDst);
			
			// Set previous point
			previousPoint = pointOfTurnBoundary;
		}

		// calcualte distance to Final point
		float distanceFromFinalPoint = 0;

		for (int i = lookPositions.Length - 1; i > 0; i--)
		{
			distanceFromFinalPoint += Vector3.Distance(lookPositions[i], lookPositions[i - 1]);
			if (distanceFromFinalPoint > stoppingDst)
			{
				slowMovementIndex = i;
				break;
			}
		}

	} // End of PathfindingPath Constructor

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// ConvertVector3ToVector2                                                                                                                //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to convert a vector3 into a vector2                                                                                          //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	Vector2 ConvertVector3ToVector2(Vector3 vector3)
	{
		return new Vector2(vector3.x, vector3.z);

	} // End of V3ToV2 function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// DrawWithGizmos                                                                                                                        //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to color the cubes in the scene view on the grid for the points                                                              //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void DrawWithGizmos()
	{

		Gizmos.color = Color.black;
		foreach (Vector3 positions in lookPositions)
		{
			Gizmos.DrawCube(positions + Vector3.up, Vector3.one);
		}

		Gizmos.color = Color.white;

		foreach (PathfindingLine line in turnBoundaries)
		{
			line.DrawWithGizmos(10);
		}

	} // End of DrawWithGizmos function

} // End of PathfindingPath class