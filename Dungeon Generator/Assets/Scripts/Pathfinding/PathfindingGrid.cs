using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//*******************************************************************************************************************************************//
//  File:   PathfindingGrid.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   A* pathfinding Grid class                                                                                                        //
//                                                                                                                                           //
//  Notes:  Basic grid for the A* pathfinding algorithm to be calculated on. This grid is always 100x100 as this is the max size of the      //
//          largest dungeon in both algorithms. This may be modified at a later date to resize to the size of the actual dungeon.            //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class PathfindingGrid : MonoBehaviour
{
	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	PathfindingNode[,] pathfindingGrid; // Two dimentional array that will represent our nodes
	float              diameterOfNode;  // Diameter size of each individual node
	int                sizeOfGridX;     // X length of the grid
	int                sizeOfGridY;     // Y length of the grid (Z in world space)

	LayerMask          traversableMask;  // layermask that will contain all of the layers in the walkable regions array

	// Dictionary to store information about each walkable node. Has a key and a value.
	Dictionary<int, int> traversableAreasDictionary = new Dictionary<int, int>();
	
	int minPenaltyValue = int.MaxValue; // blur penalty minimum value
	int maxPenaltyValue = int.MinValue; // blur penalty maximum value


	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//

	public Vector2       sizeOfGridInWorld;   // Area in world corrdinates that the grid is going to cover
	public float         radiusOfNode;        // How much space each individual node covers
	public LayerMask     untraversableMask;   // Layer for items to be placed in that are not walkable by player or enemies (ie walls, obsticles)
	public bool          displayGridFeatures; // Allows the user to choose if they want the grid gizmos displayed in the scene or not
	public TerrainType[] traversableAreas;    // Regions that are walkable stored in an array. Can be changed in the inspector

	public int obstacleProximityPenalty = 10; // A pentaly for walking too close to obsticles. Set to 10 by default

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CreatePathfindingGrid                                                                                                                 //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to set he values used for the CreateGrid function. First the grid size is calculated.                                        //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void CreatePathfindingGrid()
	{
		// Calculate node diameter
		diameterOfNode = radiusOfNode * 2;
	
		// Calculate X size of grid
		sizeOfGridX = Mathf.RoundToInt(sizeOfGridInWorld.x / diameterOfNode); // Can't have half a node so value is rounded to int
		
		// Calculate Y size of grid
		sizeOfGridY = Mathf.RoundToInt(sizeOfGridInWorld.y / diameterOfNode);

		foreach (TerrainType area in traversableAreas)
		{
			// Layers are stored in a 32 bit integer 
			// For example:
			//     layer 9  is 1000000000  which is 512
			//     layer 10 is 10000000000 which is 1024
			//     so add them together would produce 11000000000 = 1536
			traversableMask.value = traversableMask | area.terrainMaskType.value;

			// Add the terrain penalty as the value
			// As layer 9 has a value of 512, we can use mathf.log and give it the value and 2
			// To what power do we need to raise 2 to to get this value? 2 ^ 9 = 512
			// Mathf.log returns a float so need to cast to an integer
			traversableAreasDictionary.Add((int)Mathf.Log(area.terrainMaskType.value, 2), area.terrainPenaltyValue);
		}

		CreateFinalPathfindingGrid();

	} // End of CreatePathfindingGrid function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// maximumSizeOfGrid                                                                                                                     //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Integer to calculate the maximum heap side by taking the dimensons of the grid and multiplying them together                          //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public int maximumSizeOfGrid
	{
		get
		{
			return sizeOfGridX * sizeOfGridY;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CreateFinalPathfindingGrid                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to create the pathfinding grid in worldspace that will be used to calculate the A* pathfinding algorithm on.                 //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void CreateFinalPathfindingGrid()
	{
		// create new grid with grid sizes
		pathfindingGrid = new PathfindingNode[sizeOfGridX, sizeOfGridY];

		// Calculate bottom left position of the grid
		Vector3 bottomLeftWorldPosition = transform.position - Vector3.right * sizeOfGridInWorld.x / 2 - Vector3.forward * sizeOfGridInWorld.y / 2;

		// Get the currently active scene
		Scene currentlyActiveScene = SceneManager.GetActiveScene();

		// Check to see which scene is currently active and position the grid accordingly
		if (currentlyActiveScene.name == "Cellular Automata Dungeon Maker")
        {
			bottomLeftWorldPosition = new Vector3(bottomLeftWorldPosition.x, 0.0f, bottomLeftWorldPosition.z);
		}
		
		// Loop through all the positions of the nodes to do a collision check to see if they are walkable or not
		for (int x = 0; x < sizeOfGridX; x++)
		{
			for (int y = 0; y < sizeOfGridY; y++)
			{
				// Calculate world point of the node position
				Vector3 currentWorldPoint = bottomLeftWorldPosition + Vector3.right * (x * diameterOfNode + radiusOfNode) + Vector3.forward * (y * diameterOfNode + radiusOfNode);

				// Check if the node is walkable. This uses the Checksphere function to do a collision check 
				// with the node and any other objects on the unwalkable layer
				bool isWalkable = !(Physics.CheckSphere(currentWorldPoint, radiusOfNode, untraversableMask));

				// Create moevment penalty temp variable
				int nodeMovementPenalty = 0;

				// Fire rays down into the node in the grid to detect which type of mask it is on

				Ray rayCast1 = new Ray(currentWorldPoint + Vector3.up * 50, Vector3.down);
				RaycastHit rayHitResult; // store hit information

				// Get the layer of the object the ray has hit
				// Also find its corrisponding 
				if (Physics.Raycast(rayCast1, out rayHitResult, 100, traversableMask))
				{
					// Store this information in a dictionary , key = layer value = movement penalty
					traversableAreasDictionary.TryGetValue(rayHitResult.collider.gameObject.layer, out nodeMovementPenalty);
				}

				// Set obsticle proximity penalty
				// This stops the unit moving too close to obsticles that are not walkable. Can be changed in the inspector 
				if (!isWalkable)
				{
					// Increase movement penalty by proximity Penalty
					nodeMovementPenalty += obstacleProximityPenalty;
				}

				// populate the grid with nodes. Pass in if it is walkable or not and the position/movement penalty
				pathfindingGrid[x, y] = new PathfindingNode(isWalkable, currentWorldPoint, x, y, nodeMovementPenalty);
			}
		}

		// Blue the penalty map using box blur

		BoxBlurredPenaltyMap(3);

	} // End of CreateFinalPathfindingGrid function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// BoxBlurredPenaltyMap                                                                                                                  //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to add a blur penalty to all the nodes in the map. This procedure uses an algorithm known as "box blur".                     //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void BoxBlurredPenaltyMap(int blurSize)
	{
		 // Calculate kernel size. Must be an odd number so it has a central sqaure
		int mapKernelSize = blurSize * 2 + 1;

		// Calculate how many squares there are between the central square and the edge of the square
		// So for a 3x3 kernal there will only be 1 square to the edge
		int mapKernelExtents = (mapKernelSize - 1) / 2;


		// Create array to store horizontal passes in and set it to size of grid
		int[,] mapPenaltiesHorizontal = new int[sizeOfGridX, sizeOfGridY];

		// Create array to store vertical passes in
		int[,] mapPenaltiesVertical = new int[sizeOfGridX, sizeOfGridY];

		// Horizontal pass
		// Go through row by row
		for (int y = 0; y < sizeOfGridY; y++)
		{
			// First node
			// Loop through all of the nodes in the kernal and add them together
			for (int x = -mapKernelExtents; x <= mapKernelExtents; x++)
			{
				// Clamp value if it is negative and dont let it exceed kernelextents
				int xSample = Mathf.Clamp(x, 0, mapKernelExtents);
				
				// Add this nodes value at the correct position
				mapPenaltiesHorizontal[0, y] += pathfindingGrid[xSample, y].gridMovementPenalty;
			}

			// new loop for all the remaining columns in this row
			for (int x = 1; x < sizeOfGridX; x++)
			{
				// Calcualte index of node that is no longer in the kernal
				// Clamp between 0 and gridSizeX as max value
				int deleteIndex = Mathf.Clamp(x - mapKernelExtents - 1, 0, sizeOfGridX);
				
				// node that has just entered the kernal, clamp value also
				int addIndex = Mathf.Clamp(x + mapKernelExtents, 0, sizeOfGridX - 1);

				// Add penatly node value
				mapPenaltiesHorizontal[x, y] = mapPenaltiesHorizontal[x - 1, y] - pathfindingGrid[deleteIndex, y].gridMovementPenalty + pathfindingGrid[addIndex, y].gridMovementPenalty;
			}
		}


		// Vertical pass (similar process to above except it is vertical
		for (int x = 0; x < sizeOfGridX; x++)
		{
			for (int y = -mapKernelExtents; y <= mapKernelExtents; y++)
			{
				int ySample = Mathf.Clamp(y, 0, mapKernelExtents);

				// Sample grid from horizontal pass
				mapPenaltiesVertical[x, 0] += mapPenaltiesHorizontal[x, ySample];
			}

			// Blue the outisde edge of the map
			int blurredPenaltyValue = Mathf.RoundToInt((float)mapPenaltiesVertical[x, 0] / (mapKernelSize * mapKernelSize));
			pathfindingGrid[x, 0].gridMovementPenalty = blurredPenaltyValue;

			for (int y = 1; y < sizeOfGridY; y++)
			{
				int deleteIndex = Mathf.Clamp(y - mapKernelExtents - 1, 0, sizeOfGridY);
				int addIndex    = Mathf.Clamp(y + mapKernelExtents, 0, sizeOfGridY - 1);

				mapPenaltiesVertical[x, y] = mapPenaltiesVertical[x, y - 1] - mapPenaltiesHorizontal[x, deleteIndex] + mapPenaltiesHorizontal[x, addIndex];

				// Get the final blurred penalty for each node
				blurredPenaltyValue = Mathf.RoundToInt((float)mapPenaltiesVertical[x, y] / (mapKernelSize * mapKernelSize));

				// Set grid movement penalty to the new penalty value
				pathfindingGrid[x, y].gridMovementPenalty = blurredPenaltyValue;

				// Set blurred penalty
				if (blurredPenaltyValue > maxPenaltyValue) 
				{
					maxPenaltyValue = blurredPenaltyValue;
				}
				if (blurredPenaltyValue < minPenaltyValue)
				{
					minPenaltyValue = blurredPenaltyValue;
				}
			}
		}

	} // End of BoxBlurredPenaltyMap funtion

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GetNeighboursOfNode                                                                                                                   //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//  Function that gets the neighbours of a node passed into it. Returns a list of nodes. Calculates all the nodes around the passed in   //
	//  node.                                                                                                                                //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public List<PathfindingNode> GetNeighboursOfNode(PathfindingNode currentNode)
	{
		// Create a new list to store the neighbour nodes
		List<PathfindingNode> neighboursOfNode = new List<PathfindingNode>();

		// Loop that searches in a 3x3 block around the node
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) // Not a neighbouring node
                {
					continue; // Skip iteration
				}
					
				// Calculate nodes grid X position
				int checkXPosition = currentNode.gridXPos + x;
				// Calculate nodes grid Y position
				int checkYPosition = currentNode.gridYPos + y;

				// Check if the surrounding node is inside the grid boundaries itself
				if (checkXPosition >= 0 && checkXPosition < sizeOfGridX && checkYPosition >= 0 && checkYPosition < sizeOfGridY)
				{
					neighboursOfNode.Add(pathfindingGrid[checkXPosition, checkYPosition]); // Add node to the neighbours list
				}
			}
		}

		return neighboursOfNode; // Return list

	} // End of GetNeighboursOfNode function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CalculateNodeFromAWorldPoint                                                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that takes in a Vector3 world position and returns the current node of that position in world space. This is used to         //
	// calculate which nodes the player and enemies are standing on                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public PathfindingNode CalculateNodeFromAWorldPoint(Vector3 worldPosition)
	{
		// Convert the world position into a percentage of how far along the X and Y grid it is located
		// If it is located at the far left it will have a percentage of 0
		// if it is located in the middle it will have a percentage of 0.5
		// If it is located at the far right it will have a percentage of 1

		// Calculate percent X
		float percentPositionX = (worldPosition.x + sizeOfGridInWorld.x / 2) / sizeOfGridInWorld.x;
		percentPositionX = Mathf.Clamp01(percentPositionX); // Clamp between 0 and 1 to check if it outside the grid

		// Calcualte percent Y
		float percentPositionY = (worldPosition.z + sizeOfGridInWorld.y / 2) / sizeOfGridInWorld.y;
		percentPositionY = Mathf.Clamp01(percentPositionY); // Clamp between 0 and 1 to check if it outside the grid

		// Get the x and y indicies of the 2D grid array
		int xIndex = Mathf.RoundToInt((sizeOfGridX - 1) * percentPositionX);
		int yIndex = Mathf.RoundToInt((sizeOfGridY - 1) * percentPositionY);

		// Return the grid position
		return pathfindingGrid[xIndex, yIndex];

	} // End of CalculateNodeFromAWorldPoint function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// OnDrawGizmos                                                                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to draw gizmos in the Scene view. in this class it is used to visualise our grid size. First it draws a wirecube around the  //
	// edge of the grid and will draw small cubes for each node within the grid.                                                             //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void OnDrawGizmos()
	{
		// Draw a wirecube to show the outline of the grid
		Gizmos.DrawWireCube(transform.position, new Vector3(sizeOfGridInWorld.x, 1, sizeOfGridInWorld.y)); 

		if (pathfindingGrid != null && displayGridFeatures) // Has user selected to show gridgizmos?
		{
			foreach (PathfindingNode n in pathfindingGrid) // for each node in grid
			{
				// Fade the color of the node from white to black depending on the value of the weight
				// value of 0 if it is equal to the lowest penalty value
				// value of 1 if it is equal to the highest penalty value
				Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(minPenaltyValue, maxPenaltyValue, n.gridMovementPenalty));

				// Colour the nodes accordingly. Set to red if not walkable
				if (n.isWalkable)
                {
					Gizmos.color = Gizmos.color; // Keep colour if not walkable

				}
				else
                {
					Gizmos.color = Color.red; // Set as red for unwalkable
				}


				Gizmos.DrawCube(n.worldLocation, Vector3.one * (diameterOfNode));
			}
		}

	} // End of OnDrawGizmos function

	//***************************************************************************************************************************************//
	//	Classes                                                                                                                              //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// TerrainType                                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//  Class to store the different terrain types the player or enemies may encounter                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	[System.Serializable] // Show in inspector
	public class TerrainType
	{
		public LayerMask terrainMaskType;     // The layer this terrain is set too
		public int       terrainPenaltyValue;  // The penalty cost for walking on this terrain

	} // End of TerrainType Class


} // End of PathfindingGrid class