using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   PathfindingHeap.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Pathfinding heap class                                                                                                           //
//                                                                                                                                           //
//  Notes:  Heap class used to sort the node. The heap is basically a binary tree where each node in the tree has 2 child nodes. In the      //
//          code this is an array with the elements ordered in the indicies from left to right. The tree is struceted by a rule where each   //
//          parent node must be less than both of its child nodes. When adding new nodes to the heap, we can check if it is lower than its   //
//          parent node and if it is not, we just swap it with its parent node. Then we compare it to its new parent and do the same and so  //
//          on. When removing a node from the heap (with its lowest f cost for example), it will always be the node at the top of the heap   //
//          and we remove that node and to fill that position we take the node at the end of the heap and put it at the top. Then we compare //
//          it to its child nodes and if it is greater than either of them we swap it with its lowest one and so on, till it is sorted.      //
//          Because of the small amount of comparisons, this makes a heap a lot faster.                                                      //
//                                                                                                                                           //
//          Where n is equal to the current node, the forumlas for finding items on the heap are as follows:                                 //
//                                                                                                                                           //
//          Parent node                  =      (n - 1) / 2                                                                                  //
//          Child node on the left       =      2 * n + 1                                                                                    //
//          Child node on the right      =      2 * n + 2                                                                                    //
//                                                                                                                                           //
//          This is a generic class that takes a type of T so its not only specifically designed to work with the node class but will        //
//          work with other classes too.                                                                                                     //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class PathfindingHeap<T> where T : IHeapItem<T>
{
	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	T[] heapItems;             // Specify an array of type T
	int currentHeapItemCount;  // Current item count

	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//

	public int Count
	{
		get
		{
			return currentHeapItemCount; // returns amount of items in heap
		}
	}

	//***************************************************************************************************************************************//
	//	Constructor                                                                                                                          //
	//***************************************************************************************************************************************//

	public PathfindingHeap(int maximumSizeOfHeap) // Takes in maximum amount of nodes that can be a grid at any one time
	{
		heapItems = new T[maximumSizeOfHeap]; // create new heap
	}

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// AddToHeap                                                                                                                             //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to add an item to the heap. All thess function use the interface class.                                                      //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void AddToHeap(T heapItem)
	{

		heapItem.PathfindingHeapIndex = currentHeapItemCount;

		// Add item to end of items array
		heapItems[currentHeapItemCount] = heapItem;
		
		// Sort the heap
		SortHeapUp(heapItem);
		
		// Increase the count
		currentHeapItemCount++;

	} // End of AddToHeap funtion

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// RemoveFirstItemFromHeap                                                                                                               //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that removes the first item from the heap.                                                                                   //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public T RemoveFirstItemFromHeap()
	{
		// Save first item
		T firstItemInHeap = heapItems[0];

		// Decrement cound (1 less item in heap)
		currentHeapItemCount--;

		// Take item at end of heap and put it into the first position
		heapItems[0] = heapItems[currentHeapItemCount];

		// Set heapindex item to 0
		heapItems[0].PathfindingHeapIndex = 0;

		// Sort the heap down
		SortHeapDown(heapItems[0]);

		return firstItemInHeap;

	} // End of RemoveFirstItemFromHeap function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// UpdateItemInHeap                                                                                                                      //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to change the priority of an item. In the pathfinding situation, we might find a node in the open list that we want to       //
	// update with a lower fcost so we can update its position in the heap                                                                   //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void UpdateItemInHeap(T item)
	{
		// If priority has been increase, just call the sortheapup function
		SortHeapUp(item);

	} //  Enf of UpdateItemInHeap function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// IfHeapContains                                                                                                                        //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to check if the heap contains a specific items                                                                               //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public bool IfHeapContains(T item)
	{
		// Uses the equals method to check if the items are equal
		// Return true or false
		return Equals(heapItems[item.PathfindingHeapIndex], item);

	} // End of IfHeapContains function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// SortHeapDown                                                                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//  Function that sorts the heap tree downwards                                                                                          //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void SortHeapDown(T heapItem)
	{
		while (true)
		{
			// get teh index of the items 2 children
			int childIndexLeft  = heapItem.PathfindingHeapIndex * 2 + 1;
			int childIndexRight = heapItem.PathfindingHeapIndex * 2 + 2;

			// Create swap index temp value
			int swapIndex = 0;

			// Does this item have at least one child? (check left one first)
			if (childIndexLeft < currentHeapItemCount)
			{
				// Set swapindex
				swapIndex = childIndexLeft;

				// Does the item have a child on the right? (second child)
				if (childIndexRight < currentHeapItemCount)
				{
					// Check which of the two children has a higher priority
					if (heapItems[childIndexLeft].CompareTo(heapItems[childIndexRight]) < 0)
					{
						// Set swap index
						swapIndex = childIndexRight;
					}
				}

				// Has the parent got a lower priority than its highest priority child?
				if (heapItem.CompareTo(heapItems[swapIndex]) < 0)
				{
					// Swap the items
					SwapItemInHeap(heapItem, heapItems[swapIndex]);
				}
				else
				{
					// If parent has a high priority with its children? then it is in correct position so exit
					return;
				}

			}
			else
			{
				// If the parent does not have any children? then it is in correct position so exit 
				return;
			}

		}

	} // End of SortHeapDown function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// SortHeapUp                                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Fucntion that sorts the heap tree upwards.                                                                                            //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void SortHeapUp(T item)
	{
		// create parent index using the forumla
		int parentHeapIndex = (item.PathfindingHeapIndex - 1) / 2;

		while (true)
		{
			// Set parent item
			T parentHeapItem = heapItems[parentHeapIndex];
			
			// Compare current item to parent item
			// If it has a haigher pirority it returns 1
			// If it has the smae priority it returns 0
			// If it has a lower priority it retuns -1
			// in this case, if it has a higher priority, it means it has a lower fcost
			if (item.CompareTo(parentHeapItem) > 0)
			{
				// Swap it with its parent item.
				SwapItemInHeap(item, parentHeapItem);
			}
			else
			{
				break; // break out of loop
			}

			// Keep re-calculating the parent index and comparing it to its new parent 
			parentHeapIndex = (item.PathfindingHeapIndex - 1) / 2;
		}

	} // End of SortHeapUp function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// SwapItemInHeap                                                                                                                        //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that swaps an item in the heap with its parent item                                                                          //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void SwapItemInHeap(T itemOne, T itemTwo)
	{
		// First swap them in the array
		heapItems[itemOne.PathfindingHeapIndex] = itemTwo;
		heapItems[itemTwo.PathfindingHeapIndex] = itemOne;

		// Swap the heap index values using a temp integer
		int itemOneIndex = itemOne.PathfindingHeapIndex;

		// Set correct indexes
		itemOne.PathfindingHeapIndex = itemTwo.PathfindingHeapIndex;
		itemTwo.PathfindingHeapIndex = itemOneIndex;

	} // End of SwapItemInHeap

} // End of PathfindingHeap class

//*******************************************************************************************************************************************//
//	Interface Class                                                                                                                          //
//*******************************************************************************************************************************************//


public interface IHeapItem<T> : IComparable<T>
{
	int PathfindingHeapIndex
	{
		get;
		set;
	}
}

