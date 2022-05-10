using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPNode.cs                                                                                                                       //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BPS Dungeon Base Node Class                                                                                                      //
//                                                                                                                                           //
//  Notes:  Abstract class. This is a base node used in the BSP tree                                                                         //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public abstract class BSPNode
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private List<BSPNode> listOfChildren;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public List<BSPNode> ListOfChildren // List of children nodes in the tree
    { 
        get => listOfChildren;
    }

    public bool visited // Has this node been visited?
    { 
        get;
        set; 
    }

    public Vector2Int bottomLeftRegionCorner // Bottom left corner details of the region of the node
    { 
        get;
        set; 
    }

    public Vector2Int bottomRightRegionCorner // Bottom right corner details of the region of the node
    { 
        get;
        set; 
    }

    public Vector2Int topRightRegionCorner // Top Right corner details of the region of the node
    { 
        get;
        set; 
    }

    public Vector2Int topLeftRegionCorner // Top Left corner details of the region of the node
    { 
        get;
        set; 
    }

    public BSPNode parentNode // Details of this nodes parent node
    { 
        get;
        set;
    }


    public int treeLayerValue // The layer of the tree the node is located
    {
        get;
        set;
    }

    //***************************************************************************************************************************************//
    //	Constructor                                                                                                                          //
    //***************************************************************************************************************************************//

    public BSPNode(BSPNode parentNode)
    {
        listOfChildren = new List<BSPNode>(); // List of children nodes connected to this node

        this.parentNode = parentNode; // Set parent node

        if (parentNode != null)
        {
            parentNode.AddChildToList(this); // Add parent node to list
        }

    } // End of Constructor

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // AddChildToList                                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to add this node as a child of another node to the list of children. each node has its own list.                            //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void AddChildToList(BSPNode node)
    {
        listOfChildren.Add(node); // Add child node to list

    } // End of AddChildToList function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // RemoveChildFromList                                                                                                                   //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to remove this node from a list of children of a previous node.                                                              //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void RemoveChildFromList(BSPNode node)
    {
        listOfChildren.Remove(node);

    } // End of RemoveChildFromList function

} // End of Node class
