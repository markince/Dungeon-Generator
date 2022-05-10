using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPCorridorNode.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Dungeon Corridor Node Class                                                                                                  //
//                                                                                                                                           //
//  Notes:  Rooms in the dungeon are connected together via cooridors. The user can specific the width of the corridors in the options.      //
//          Corridors are created using meshes and the walls are generated on the left and right, or top and bottom sides of the corridors   //
//          depending on teh allignment. Enemies and objects/items can spawn inside corridors if they are large enough to be classed         //
//          as rooms                                                                                                                         //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPCorridorNode : BSPNode
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private BSPNode area1;                         // Area 1 to be connect via a corridor
    private BSPNode area2;                         // Area 2 to be connect via a corridor
    private int     widthOfCorridor;               // Width of corridor
    private int     distanceFromWallModifier = 1;  // Distance 

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                          //
    //***************************************************************************************************************************************//

    public BSPCorridorNode(BSPNode area1, BSPNode area2, int widthOfCorridor) : base (null)
    {
        // Set variables
        this.area1           = area1;
        this.area2           = area2;
        this.widthOfCorridor = widthOfCorridor;

        // Create the corridor
        CreateCorridor();
    }

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // GetXPosForRoomUpOrDown                                                                                                                //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                       //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private int GetXPosForRoomUpOrDown(Vector2Int bottomRoomLeft, Vector2Int bottomRoomRight,
                                       Vector2Int topRoomLeft, Vector2Int topRoomRight)
    {
        if (topRoomLeft.x < bottomRoomLeft.x && bottomRoomRight.x < topRoomRight.x)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(bottomRoomLeft + new Vector2Int(distanceFromWallModifier, 0),
                                                      bottomRoomRight - new Vector2Int(this.widthOfCorridor + distanceFromWallModifier, 0)).x;
        }
        if (topRoomLeft.x >= bottomRoomLeft.x && bottomRoomRight.x >= topRoomRight.x)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(topRoomLeft + new Vector2Int(distanceFromWallModifier, 0),
                                                      topRoomRight - new Vector2Int(this.widthOfCorridor + distanceFromWallModifier, 0)).x;
        }
        if (bottomRoomLeft.x >= topRoomLeft.x && bottomRoomLeft.x <= topRoomRight.x)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(bottomRoomLeft + new Vector2Int(distanceFromWallModifier, 0),
                                                      topRoomRight - new Vector2Int(this.widthOfCorridor + distanceFromWallModifier, 0)).x;
        }
        if (bottomRoomRight.x <= topRoomRight.x && bottomRoomRight.x >= topRoomLeft.x)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(topRoomLeft + new Vector2Int(distanceFromWallModifier, 0),
                                                      bottomRoomRight - new Vector2Int(this.widthOfCorridor + distanceFromWallModifier, 0)).x;
        }

        return -1;

    } // End of GetXPosForRoomUpOrDown function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // ProcessRoomUpOrDown                                                                                                                   //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to create a corridor with rooms that are connected above and below each other.                                               //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void ProcessRoomUpOrDown(BSPNode area1, BSPNode area2)
    {
        BSPNode bottomArea = null;
        List<BSPNode> areaBelowChildren = BSPDungeonHelper.NavigateBPSGraphToFindLowestNodes(area1);
        BSPNode topArea = null;
        List<BSPNode> areaAboveChildren = BSPDungeonHelper.NavigateBPSGraphToFindLowestNodes(area2);

        var bottomAreaSorted = areaBelowChildren.OrderByDescending(child => child.topRightRegionCorner.y).ToList();

        if (bottomAreaSorted.Count == 1)
        {
            bottomArea = areaBelowChildren[0];
        }
        else
        {
            int maximumY = bottomAreaSorted[0].topLeftRegionCorner.y;
            bottomAreaSorted = bottomAreaSorted.Where(child => Mathf.Abs(maximumY - child.topLeftRegionCorner.y) < 10).ToList();
            int i = UnityEngine.Random.Range(0, bottomAreaSorted.Count);
            bottomArea = bottomAreaSorted[i];
        }

        var neighboursInTopArea = areaAboveChildren.Where(child => GetXPosForRoomUpOrDown(bottomArea.topLeftRegionCorner,
                                                                                               bottomArea.topRightRegionCorner,
                                                                                               child.bottomLeftRegionCorner,
                                                                                               child.topRightRegionCorner)
                                                                                               != -1).OrderBy(child => 
                                                                                               child.bottomRightRegionCorner.y).ToList();

        if (neighboursInTopArea.Count == 0)
        {
            topArea = area2;
        }
        else
        {
            topArea = neighboursInTopArea[0];
        }

        int xPos = GetXPosForRoomUpOrDown(bottomArea.topLeftRegionCorner, bottomArea.topRightRegionCorner,
                                          topArea.bottomLeftRegionCorner, topArea.topRightRegionCorner);

        while (xPos == -1 && bottomAreaSorted.Count() > 1)
        {
            bottomAreaSorted = bottomAreaSorted.Where(child => child.topLeftRegionCorner.x != topArea.topLeftRegionCorner.x).ToList();
            bottomArea = bottomAreaSorted[0];
            xPos = GetXPosForRoomUpOrDown(bottomArea.topLeftRegionCorner, bottomArea.topRightRegionCorner, topArea.bottomLeftRegionCorner,
                                           topArea.topRightRegionCorner);
        }

        bottomLeftRegionCorner = new Vector2Int(xPos, bottomArea.topLeftRegionCorner.y);
        topRightRegionCorner   = new Vector2Int(xPos + this.widthOfCorridor, topArea.bottomLeftRegionCorner.y);

    } // End of ProcessRoomUpOrDown function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // GetYPosForRoomLeftOrRight                                                                                                              //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                       //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private int GetYPosForRoomLeftOrRight(Vector2Int leftRoomUp, Vector2Int leftRoomDown, Vector2Int rightRoomUp, Vector2Int rightRoomDown)
    {
        if (rightRoomUp.y >= leftRoomUp.y && leftRoomDown.y >= rightRoomDown.y)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(
                leftRoomDown + new Vector2Int(0, distanceFromWallModifier),
                leftRoomUp - new Vector2Int(0, distanceFromWallModifier + this.widthOfCorridor)).y;
        }
        if (rightRoomUp.y <= leftRoomUp.y && leftRoomDown.y <= rightRoomDown.y)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(
                rightRoomDown + new Vector2Int(0, distanceFromWallModifier),
                rightRoomUp - new Vector2Int(0, distanceFromWallModifier + this.widthOfCorridor)).y;
        }
        if (leftRoomUp.y >= rightRoomDown.y && leftRoomUp.y <= rightRoomUp.y)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(
                rightRoomDown + new Vector2Int(0, distanceFromWallModifier),
                leftRoomUp - new Vector2Int(0, distanceFromWallModifier + this.widthOfCorridor)).y;
        }
        if (leftRoomDown.y >= rightRoomDown.y && leftRoomDown.y <= rightRoomUp.y)
        {
            return BSPDungeonHelper.CalculateMiddlePoint(
                leftRoomDown + new Vector2Int(0, distanceFromWallModifier),
                rightRoomUp - new Vector2Int(0, distanceFromWallModifier + this.widthOfCorridor)).y;
        }

        return -1;

    } // End of GetYPosForRoomLeftOrRight function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // ProcessRoomLeftOrRight                                                                                                                //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    //                                                                                                                                       //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void ProcessRoomLeftOrRight(BSPNode area1, BSPNode area2)
    {
        BSPNode leftArea = null;
        List<BSPNode> areaLeftOfChildren = BSPDungeonHelper.NavigateBPSGraphToFindLowestNodes(area1);

        BSPNode rightArea = null;
        List<BSPNode> areaRightOfChildren = BSPDungeonHelper.NavigateBPSGraphToFindLowestNodes(area2);

        var leftAreaSorted = areaLeftOfChildren.OrderByDescending(child => child.topRightRegionCorner.x).ToList();
        
        if (leftAreaSorted.Count == 1)
        {
            leftArea = leftAreaSorted[0];
        }
        else
        {
            int maximumX = leftAreaSorted[0].topRightRegionCorner.x;
            leftAreaSorted = leftAreaSorted.Where(children => Math.Abs(maximumX - children.topRightRegionCorner.x) < 10).ToList();

            int i = UnityEngine.Random.Range(0, leftAreaSorted.Count);
            leftArea = leftAreaSorted[i];
        }

        var neighboursInRightArea = areaRightOfChildren.Where(child => GetYPosForRoomLeftOrRight(leftArea.topRightRegionCorner,
                                                                                                       leftArea.bottomRightRegionCorner,
                                                                                                       child.topLeftRegionCorner,
                                                                                                       child.bottomLeftRegionCorner) != -1)
                                                                                                       .OrderBy(child => child.bottomRightRegionCorner.x).ToList();

        if (neighboursInRightArea.Count <= 0)
        {
            rightArea = area2;
        }
        else
        {
            rightArea = neighboursInRightArea[0];
        }

        int yPos = GetYPosForRoomLeftOrRight(leftArea.topLeftRegionCorner, leftArea.bottomRightRegionCorner,
                                             rightArea.topLeftRegionCorner, rightArea.bottomLeftRegionCorner);

        while (yPos == -1 && leftAreaSorted.Count > 1)
        {
            leftAreaSorted = leftAreaSorted.Where(child => child.topLeftRegionCorner.y != leftArea.topLeftRegionCorner.y).ToList();
            leftArea = leftAreaSorted[0];
            
            yPos = GetYPosForRoomLeftOrRight(leftArea.topLeftRegionCorner, leftArea.bottomRightRegionCorner,
                                             rightArea.topLeftRegionCorner, rightArea.bottomLeftRegionCorner);
        }

        bottomLeftRegionCorner = new Vector2Int(leftArea.bottomRightRegionCorner.x, yPos);
        topRightRegionCorner   = new Vector2Int(rightArea.topLeftRegionCorner.x, yPos + this.widthOfCorridor);

    } // End of ProcessRoomLeftOrRight function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateCorridor                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to generate a corridor that connects two rooms together                                                                      //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateCorridor()
    {
        // Find in what relation is the first node to the second node?
        var positionOfArea2 = CheckPosition();

        //  Create the corridor depending on which position the rooms are connected in.
        switch (positionOfArea2)
        {
            case CorrespondingPosition.Up:
                {
                    ProcessRoomUpOrDown(this.area1, this.area2);
                    break;
                }

            case CorrespondingPosition.Down:
                {
                    ProcessRoomUpOrDown(this.area2, this.area1);
                    break;
                }

            case CorrespondingPosition.Right:
                {
                    ProcessRoomLeftOrRight(this.area1, this.area2);
                    break;
                }

            case CorrespondingPosition.Left:
                {
                    ProcessRoomLeftOrRight(this.area2, this.area1);
                    break;
                }

            default:
                break;

        } // End of Switch statment

    } //End of CreateCorridor function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CheckPosition                                                                                                                         //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that checks in what relation is the first node to the second node. it does this by calculating the angle between the area in //
    // the centre and second area. It uses the mathf funtion "Atan2" to calculate the angle and if the angle is between -45 and 45, then it  //
    // to the right. If it is between 45 and 135, it is above (see report for more details). It returns an enum (correspondingPosition) as   //
    // the direction.                                                                                                                        //
    //------------------------------------------------------------------------------------------------------------------------------------- -//

    private CorrespondingPosition CheckPosition()
    {
        // Calcualte middle points of the rooms
        Vector2 middlePointRoom1 = ((Vector2)area1.topRightRegionCorner + area1.bottomLeftRegionCorner) / 2;
        Vector2 middlePointRoom2 = ((Vector2)area2.topRightRegionCorner + area2.bottomLeftRegionCorner) / 2;

        // Calculate the angle
        float angle = CalculateAngle(middlePointRoom1, middlePointRoom2);

        // Check the angle and calculate the position in which the corridor should be generated

        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0)) // To the right?
        {
            return CorrespondingPosition.Right;
        }
        else if (angle > 45 && angle < 135) // Above?
        {
            return CorrespondingPosition.Up;
        }
        else if (angle > -135 && angle < -45) // Below?
        {
            return CorrespondingPosition.Down;
        }
        else
        {
            return CorrespondingPosition.Left; // To the left?

        } // End of switch statment

    } // End of CheckPosition function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CalculateAngle                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that calculates the angle between two points. It takes in two points and uses the mathf funtion "Atan2" to calculate the     //
    // angle. See explanation in "CheckPosition" function for more details.                                                                  //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    float CalculateAngle(Vector2 middlePoint1, Vector2 middlePoint2)
    {
        // Calcualte points
        float yTemp = middlePoint2.y - middlePoint1.y;
        float xTemp = middlePoint2.x - middlePoint1.x;

        float angle = Mathf.Atan2(yTemp, xTemp) * Mathf.Rad2Deg; // Convert to degrees

        return angle;

    } // End of CalculateAngle function

} // End of Corridor Class
