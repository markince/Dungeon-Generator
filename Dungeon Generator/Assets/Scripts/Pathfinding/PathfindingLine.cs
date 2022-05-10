using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   PathfindingLine.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Pathfinding smoothing line class                                                                                                 //
//                                                                                                                                           //
//  Notes:  Line class that draws a line from each point towards the previous point in the path. Length of the line is controlled by         //
//          the turn distance variable. At the end of each lline we draw a perpendicular line called turnBounary (except the final point)    //
//          In the pathfinding, the enemy will walk from the start point straight towards the first point, but when it passes the turn       //
//          boundary, it will start to turn towards the next point and when it passes the final turn boundary, the path is complete.         //
//          This class creates a line given a point on that line, and a line perpendicular to the line.                                      //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public struct PathfindingLine
{
	//***************************************************************************************************************************************//
	//	Constants                                                                                                                            //
	//***************************************************************************************************************************************//

	const float gradientOfVerticalLine = 1e5f; // Set to 10 ^ 5, 100,000

	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	float    lineGradient;                // Gridient of the line
	float    lineYIntercept;              // Y intercept of the line
	float    lineGradientPerpendicular;   // Gradient of the perpendicular line
	Vector2  pointOnTheLine1;             // Point on line 1 
	Vector2  pointOnTheLine2;             // Point on line 2
	bool     lineApproachSide;

	//***************************************************************************************************************************************//
	//	Structure Constructor                                                                                                                //
	//***************************************************************************************************************************************//

	public PathfindingLine(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
	{
		// Calculate the gradient perpendicular 
		float dx = pointOnLine.x - pointPerpendicularToLine.x;
		float dy = pointOnLine.y - pointPerpendicularToLine.y;

		if (dx == 0)
		{
			lineGradientPerpendicular = gradientOfVerticalLine;
		}
		else
		{
			lineGradientPerpendicular = dy / dx;
		}


		if (lineGradientPerpendicular == 0)
		{
			lineGradient = gradientOfVerticalLine;
		}
		else
		{
			// Gradient of a line multiplied by the gradient of a line that is perpendicular to it = -1
			lineGradient = -1 / lineGradientPerpendicular;
		}

		// Calculate y intercept
		lineYIntercept = pointOnLine.y - lineGradient * pointOnLine.x;
		pointOnTheLine1 = pointOnLine;
		pointOnTheLine2 = pointOnLine + new Vector2(1, lineGradient); // Can be anywhere on the line

		// Set approach side
		lineApproachSide = false; // Give it a starting value
		lineApproachSide = GetSideOfLine(pointPerpendicularToLine);

	} // End of Constructor

	//***************************************************************************************************************************************//
	//	Structure Functions                                                                                                                  //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GetSideOfLine                                                                                                                         //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that returns true if p is on one side of the line defined by gradient  and y intercept and false if it is on the other       //
	// side                                                                                                                                  //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	bool GetSideOfLine(Vector2 point)
	{
		
		return (point.x - pointOnTheLine1.x) * (pointOnTheLine2.y - pointOnTheLine1.y) > (point.y - pointOnTheLine1.y) * 
			   (pointOnTheLine2.x - pointOnTheLine1.x);

	} // End of GetSideOfLine function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// HasCrossedTheLine                                                                                                                     //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to see if a point is on the other side of the line from the perpendicular point that we are given in the constructor         //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public bool HasCrossedTheLine(Vector2 point)
	{
		return GetSideOfLine(point) != lineApproachSide;

	} // End of HasCrossedTheLine function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CalculateDistanceFromPointToLine                                                                                                      //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to calculate the distance from the line to a particular point. This is used when the enemy reaches the end line and          //
	// slows down on approaching the final line                                                                                              //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public float CalculateDistanceFromPointToLine(Vector2 point)
	{
		// Calcualte y intercept from point
		float perpendicularyIntercept = point.y - lineGradientPerpendicular * point.x;
		
		// Calcualte x cooridinate of the point of intersection
		float xIntersect = (perpendicularyIntercept - lineYIntercept) / (lineGradient - lineGradientPerpendicular);
		// Calcualte y cooridinate of the point of intersection
		float yIntersect = lineGradient * xIntersect + lineYIntercept;

		// Calcualte distance and return value
		return Vector2.Distance(point, new Vector2(xIntersect, yIntersect));

	} // End of CalculateDistanceFromPointToLine function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// DrawWithGizmos                                                                                                                        //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that will draw the lines in the Scene view so user can see the lines on the path for smoothing                               //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void DrawWithGizmos(float length)
	{
		Vector3 lineDir = new Vector3(1, 0, lineGradient).normalized;
		Vector3 lineCentre = new Vector3(pointOnTheLine1.x, 0, pointOnTheLine1.y) + Vector3.up;
		Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);

	} // End of DrawWithGizmos function

} // End of PathfindingLine class