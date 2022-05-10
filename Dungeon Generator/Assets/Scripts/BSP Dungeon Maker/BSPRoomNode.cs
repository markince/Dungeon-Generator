using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPRoomNode.cs                                                                                                                   //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BPS Dungeon Room Node Class                                                                                                      //
//                                                                                                                                           //
//  Notes:  Floors of each room are created using meshes and the walls are generated around the meshes to create rooms and cooridors.        //
//          Two enemies are spawned inside each room, and random objects are also placed as obsticles to help demonstrate the A*             //
//          pathfinding algorithm.                                                                                                           //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPRoomNode : BSPNode
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public int roomWidth // Width of the room
    { 
        get => (int)(topRightRegionCorner.x - bottomLeftRegionCorner.x);
    }
    public int roomLength // Length of the room
    { 
        get => (int)(topRightRegionCorner.y - bottomLeftRegionCorner.y);
    }

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                            //
    //***************************************************************************************************************************************//

    public BSPRoomNode(Vector2Int bottomLeftRegionCorner, Vector2Int topRightRegionCorner, BSPNode parentNode, int count) : base(parentNode)
    {
        this.bottomLeftRegionCorner  = bottomLeftRegionCorner;
        this.topRightRegionCorner    = topRightRegionCorner;
        this.bottomRightRegionCorner = new Vector2Int(topRightRegionCorner.x, bottomLeftRegionCorner.y);
        this.topLeftRegionCorner     = new Vector2Int(bottomLeftRegionCorner.x, topRightRegionCorner.y);
        this.treeLayerValue          = count;

    } // End of Room Constructor

} // End of Room Class
