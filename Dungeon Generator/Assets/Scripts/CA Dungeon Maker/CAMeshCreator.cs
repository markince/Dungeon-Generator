//*******************************************************************************************************************************************//
//  File:   CAMeshGenerator.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Main cellular automta mesh Generator class                                                                                       //
//                                                                                                                                           //
//  Notes:  Class to generate the mesh for the caves and the walls around the edge of the caves/corridors                                    //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMeshCreator : MonoBehaviour
{
	//***************************************************************************************************************************************//
	//	Structures                                                                                                                           //
	//***************************************************************************************************************************************//

	struct Triangle
	{
		public int indexOfVertexA;
		public int indexOfVertexB;
		public int indexOfVertexC;

		int[] triangleVertices;

		public Triangle(int pointA, int pointB, int pointC)
		{
			indexOfVertexA = pointA;
			indexOfVertexB = pointB;
			indexOfVertexC = pointC;

			triangleVertices = new int[3];
			triangleVertices[0] = pointA;
			triangleVertices[1] = pointB;
			triangleVertices[2] = pointC;
		}

		public int this[int i]
		{
			get
			{
				return triangleVertices[i];
			}
		}


		public bool Contains(int vertexIndex)
		{
			if (vertexIndex == indexOfVertexA || vertexIndex == indexOfVertexB || vertexIndex == indexOfVertexC)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	//***************************************************************************************************************************************//
	//	Node Class - Class to represent each node in the world                                                                               //
	//***************************************************************************************************************************************//

	public class CANode
	{
		//***********************************************************************************************************************************//
		//	Node Class Variables                                                                                                             //
		//***********************************************************************************************************************************//

		public Vector3 nodePosition; // Position of node
		public int     nodeVertextIndex = -1; // No idea what vertex index is at start so just set it to -1 to begin with.

		//***********************************************************************************************************************************//
		//	Node Class Constructor                                                                                                           //
		//***********************************************************************************************************************************//

		public CANode(Vector3 position)
		{
			nodePosition = position; // set position to start
		}

	} // End of Node Class

	//***************************************************************************************************************************************//
	//	ControlNode Class - This class inherits from node and represents the control nodes                                                   //
	//***************************************************************************************************************************************//

	public class CAControlNode : CANode
	{
		//***********************************************************************************************************************************//
		//	ControlNode Class Variables                                                                                                      //
		//***********************************************************************************************************************************//

		public bool isActive; // Is this node active? True - wall, false = empty space
		public CANode nodeAbove;
		public CANode nodeRight;

		//***********************************************************************************************************************************//
		//	ControlNode Class Constructor                                                                                                    //
		//***********************************************************************************************************************************//

		public CAControlNode(Vector3 position, bool active, float sizeOfSquare) : base(position)
		{
			// Set variables
			isActive = active;

			// Set control nodes to a thier correct position
			nodeAbove = new CANode(nodePosition + Vector3.forward * sizeOfSquare / 2.0f);
			nodeRight = new CANode(nodePosition + Vector3.right   * sizeOfSquare / 2.0f);
		}

	} // End of control node class

	//***************************************************************************************************************************************//
	//	Square Class                                                                                                                         //
	//***************************************************************************************************************************************//

	public class CASquare
	{
		//***********************************************************************************************************************************//
		//	Square Class Variables                                                                                                           //
		//***********************************************************************************************************************************//

		// refernce to all 4 control nodes at the corners of the node
		public CAControlNode nodeTopLeft;
		public CAControlNode nodeTopRight;
		public CAControlNode nodeBottomLeft;
		public CAControlNode nodeBottomRight;

		// Reference to to midpoint nodes
		public CANode nodeCentreTop;
		public CANode nodeCentreRight;
		public CANode nodeCentreBottom;
		public CANode nodeCentreLeft;

		// Create configuration value. There are 16 different cofigurations of the square avalible.
		// Each one represents 1 bit in a 4 bit binary number
		public int configuration;

		//***********************************************************************************************************************************//
		//	Square Class Constructor                                                                                                         //
		//***********************************************************************************************************************************//

		public CASquare(CAControlNode topLeftControlNode, CAControlNode topRightControlNode, CAControlNode bottomRightControlNode, CAControlNode bottomLeftControlNode)
		{
			// Set variables
			nodeTopLeft     = topLeftControlNode;
			nodeTopRight    = topRightControlNode;
			nodeBottomLeft  = bottomLeftControlNode;
			nodeBottomRight = bottomRightControlNode;

			// Set the nodes
			nodeCentreTop    = nodeTopLeft.nodeRight;
			nodeCentreRight  = nodeBottomRight.nodeAbove;
			nodeCentreBottom = nodeBottomLeft.nodeRight;
			nodeCentreLeft   = nodeBottomLeft.nodeAbove;

			// If top left is active  = binary 1000 = 8
			// So we add 8 to the configuration
			if (nodeTopLeft.isActive)
            {
				configuration += 8;

			}
			// If top right is active  = binary 0100 = 4 
			// So we add 4 to the configuration
			if (nodeTopRight.isActive)
            {
				configuration += 4;

			}
			// If bottom right is active  = binary 0010 = 2 
			// So we add 2 to the configuration
			if (nodeBottomRight.isActive)
            {
				configuration += 2;

			}
			// If bottom left is active  = binary 0001 = 1 
			// So we add 1 to the configuration
			if (nodeBottomLeft.isActive)
            {
				configuration += 1;

			}
		}

	} // End of Square Class

	//***************************************************************************************************************************************//
	//	Square Grid Class - Class to hold the 2D array of the squares                                                                        //
	//***************************************************************************************************************************************//

	public class CASquareGrid
	{
		//***********************************************************************************************************************************//
		//	SquareGrid Class Variables                                                                                                       //
		//***********************************************************************************************************************************//

		// Create 2D array of squares
		public CASquare[,] squares;

		//***********************************************************************************************************************************//
		//	SquareGrid Class Constructor                                                                                                     //
		//***********************************************************************************************************************************//

		// Takes in the 2D array of intergers that we get from the map generator class
		// and a float for the actual size that we want each square to be displayed as
		public CASquareGrid(int[,] gridMap, float sizeOfSquare)
		{
			// Calculate number of nodes needed
			int countNodeX = gridMap.GetLength(0);
			int countNodeY = gridMap.GetLength(1);

			// Calcualte the width and length of the map
			float widthOfGridMap  = countNodeX * sizeOfSquare;
			float heightOfGridMap = countNodeY * sizeOfSquare;

			// Create 2D array of control nodes side of nodecount
			CAControlNode[,] controlNodes = new CAControlNode[countNodeX, countNodeY];

			// Cycle through the control node map
			for (int x = 0; x < countNodeX; x++)
			{
				
				for (int y = 0; y < countNodeY; y++)
				{
					// Calculate position
					Vector3 nodePosition = new Vector3(-widthOfGridMap / 2 + x * sizeOfSquare + sizeOfSquare / 2, 0, -heightOfGridMap / 2 + y * sizeOfSquare + sizeOfSquare / 2);
					// Pass in position and weather or not it is active or not (we can get this information from the map)
					controlNodes[x, y] = new CAControlNode(nodePosition, gridMap[x, y] == 1, sizeOfSquare);
				}
			}

			// next we need to create a grid squares from the control nodes

			// Create new square map
			squares = new CASquare[countNodeX - 1, countNodeY - 1];

			// Loop through the square map
			for (int x = 0; x < countNodeX - 1; x++)
			{
				for (int y = 0; y < countNodeY - 1; y++)
				{
					// Set each one equal to a new square and give it a top left, top right, bottom left and bottom right
					// control node
					squares[x, y] = new CASquare(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
				}
			}

		}

	} // End of SquareGrid Class

	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	List<Vector3> vertices; // List of positions of the meshes
	List<int>     triangles; // Triangles used in the meshes

	Dictionary<int, List<Triangle>> dictionaryOfTriangles = new Dictionary<int, List<Triangle>>();

	// List used to store multiple outlines
	List<List<int>> outlines = new List<List<int>>();

	// Optimisation to make sure when we check a vertex, we dont check it again
	HashSet<int> checkedVertices = new HashSet<int>();

	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//

	public CASquareGrid gridOfSquares; // Create an instance of the squaregrid
	public MeshFilter dungeonWalls;
	public MeshFilter dungeonCaves;

	//***************************************************************************************************************************************//
	//	CAMeshCreator Class functions                                                                                                        //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// BuildDungeonMesh                                                                                                                      //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//  Function to takes in a 2D array of ints for the map and the size of each square and generates a mesh for the cave layout             //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void BuildDungeonMesh(int[,] gridMap, float sizeOfSquares)
	{
		// Create mesh of dungeon layout

		dictionaryOfTriangles.Clear();
		outlines.Clear();
		checkedVertices.Clear();

		// Set squaregrid variable
		gridOfSquares = new CASquareGrid(gridMap, sizeOfSquares);

		// set verticies and triangles lists
		vertices = new List<Vector3>();
		triangles = new List<int>();

		// Go through each of the squares in the square grid
		for (int x = 0; x < gridOfSquares.squares.GetLength(0); x++)
		{
			for (int y = 0; y < gridOfSquares.squares.GetLength(1); y++)
			{
				// Triangulate each square
				SeperateSquareIntoTriangles(gridOfSquares.squares[x, y]);
			}
		}

		// Create new mesh
		Mesh mesh = new Mesh();
		dungeonCaves.mesh = mesh;

		// Set verticies
		mesh.vertices = vertices.ToArray();
		// Set triangles
		mesh.triangles = triangles.ToArray();
		// Recalculate normals
		mesh.RecalculateNormals();

		int tileAmount = 10;

		Vector2[] uvs = new Vector2[vertices.Count];

		for (int i = 0; i < vertices.Count; i++)
		{
			float xPercentage = Mathf.InverseLerp(-gridMap.GetLength(0) / 2 * sizeOfSquares, gridMap.GetLength(0) / 2 * sizeOfSquares, vertices[i].x) * tileAmount;
			float yPercentage = Mathf.InverseLerp(-gridMap.GetLength(0) / 2 * sizeOfSquares, gridMap.GetLength(0) / 2 * sizeOfSquares, vertices[i].z) * tileAmount;

			uvs[i] = new Vector2(xPercentage, yPercentage);
		}
		mesh.uv = uvs;

		

		// Create the walls around the edge of the inside of the dungeon mesh

		MeshCollider currentCollider = GetComponent<MeshCollider>();
		Destroy(currentCollider);

		// Calculate the outlines of the Mesh

		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
		{
			if (!checkedVertices.Contains(vertexIndex))
			{
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1)
				{
					checkedVertices.Add(vertexIndex);

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutlineOfMesh(newOutlineVertex, outlines.Count - 1);
					outlines[outlines.Count - 1].Add(vertexIndex);
				}
			}
		}

		// Simplify the outlines of the mesh

		for (int outlineIndex = 0; outlineIndex < outlines.Count; outlineIndex++)
		{
			List<int> simplifiedOutline = new List<int>();
			Vector3 dirOld = Vector3.zero;
			for (int i = 0; i < outlines[outlineIndex].Count; i++)
			{
				Vector3 p1 = vertices[outlines[outlineIndex][i]];
				Vector3 p2 = vertices[outlines[outlineIndex][(i + 1) % outlines[outlineIndex].Count]];
				Vector3 dir = p1 - p2;
				if (dir != dirOld)
				{
					dirOld = dir;
					simplifiedOutline.Add(outlines[outlineIndex][i]);
				}
			}
			outlines[outlineIndex] = simplifiedOutline;
		}

		// Generate the walls

		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();
		Mesh wallMesh = new Mesh();
		float wallHeight = 5;

		foreach (List<int> outline in outlines)
		{
			for (int i = 0; i < outline.Count - 1; i++)
			{
				int startIndex = wallVertices.Count;
				wallVertices.Add(vertices[outline[i]]); // left
				wallVertices.Add(vertices[outline[i + 1]]); // right
				wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
				wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);

				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}
		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		dungeonWalls.mesh = wallMesh;

		MeshCollider wallCollider = gameObject.AddComponent<MeshCollider>();
		wallCollider.sharedMesh = wallMesh;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// SeperateSquareIntoTriangles                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to triangulate each square with triangular meshes to create the walls. Looks at the binary number configuration of the       //
	// square and creates the mesh accordingly                                                                                               //                                                                                                                                      //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void SeperateSquareIntoTriangles(CASquare square)
	{
		// First we look individually through each of the 16 cases
		// to create the meshes from the points
		switch (square.configuration)
		{
			case 0:
                {
					break; // No mesh so just break
				}
			// 1 point:
			case 1:
				{
					CreateMeshFromPoints(square.nodeCentreLeft, square.nodeCentreBottom, square.nodeBottomLeft);
					break;
				}
			case 2:
				{
					CreateMeshFromPoints(square.nodeBottomRight, square.nodeCentreBottom, square.nodeCentreRight);
					break;
				}
			case 4:
				{
					CreateMeshFromPoints(square.nodeTopRight, square.nodeCentreRight, square.nodeCentreTop);
					break;
				}
			case 8:
				{
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeCentreTop, square.nodeCentreLeft);
					break;
				}
			// 2 points:
			case 3:
				{
					CreateMeshFromPoints(square.nodeCentreRight, square.nodeBottomRight, square.nodeBottomLeft, square.nodeCentreLeft);
					break;
				}
			case 6:
				{
					CreateMeshFromPoints(square.nodeCentreTop, square.nodeTopRight, square.nodeBottomRight, square.nodeCentreBottom);
					break;
				}
			case 9:
				{
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeCentreTop, square.nodeCentreBottom, square.nodeBottomLeft);
					break;
				}
			case 12:
                {
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeTopRight, square.nodeCentreRight, square.nodeCentreLeft);
					break;
				}
			case 5:
				{
					CreateMeshFromPoints(square.nodeCentreTop, square.nodeTopRight, square.nodeCentreRight, square.nodeCentreBottom, square.nodeBottomLeft, square.nodeCentreLeft);
					break;
				}
			case 10:
                {
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeCentreTop, square.nodeCentreRight, square.nodeBottomRight, square.nodeCentreBottom, square.nodeCentreLeft);
					break;
				}
			// 3 points:
			case 7:
                {
					CreateMeshFromPoints(square.nodeCentreTop, square.nodeTopRight, square.nodeBottomRight, square.nodeBottomLeft, square.nodeCentreLeft);
					break;
				}
			case 11:
                {
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeCentreTop, square.nodeCentreRight, square.nodeBottomRight, square.nodeBottomLeft);
					break;
				}
			case 13:
                {
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeTopRight, square.nodeCentreRight, square.nodeCentreBottom, square.nodeBottomLeft);
					break;
				}
			case 14:
                {
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeTopRight, square.nodeBottomRight, square.nodeCentreBottom, square.nodeCentreLeft);
					break;
				}
			// 4 point:
			case 15:
                {
					CreateMeshFromPoints(square.nodeTopLeft, square.nodeTopRight, square.nodeBottomRight, square.nodeBottomLeft);
					checkedVertices.Add(square.nodeTopLeft.nodeVertextIndex);
					checkedVertices.Add(square.nodeTopRight.nodeVertextIndex);
					checkedVertices.Add(square.nodeBottomRight.nodeVertextIndex);
					checkedVertices.Add(square.nodeBottomLeft.nodeVertextIndex);
					break;
				}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CreateMeshFromPoints                                                                                                                  //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to create a mesh from the points that are sent into the function. Function receieves a array of nodes.                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void CreateMeshFromPoints(params CANode[] points)
	{
		// Assign the verticies
		for (int i = 0; i < points.Length; i++)
		{
			if (points[i].nodeVertextIndex == -1)
			{
				points[i].nodeVertextIndex = vertices.Count;
				vertices.Add(points[i].nodePosition);
			}
		}

		if (points.Length >= 3)
        {
			GenerateTriangle(points[0], points[1], points[2]);
		}
		if (points.Length >= 4)
        {
			GenerateTriangle(points[0], points[2], points[3]);
		}
		if (points.Length >= 5)
        {
			GenerateTriangle(points[0], points[3], points[4]);
		}
		if (points.Length >= 6)
        {
			GenerateTriangle(points[0], points[4], points[5]);
		}

	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GenerateTriangle                                                                                                                      //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// funtion that recives 3 nodes and creates a triangle out of those nodes.                                                                                                                                      //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void GenerateTriangle(CANode pointA, CANode pointB, CANode pointC)
	{
		// Assign the triangles list
		// Triangles are made out of 3 vertices
		// Specify the index of those vertices in the vertex list
		triangles.Add(pointA.nodeVertextIndex);
		triangles.Add(pointB.nodeVertextIndex);
		triangles.Add(pointC.nodeVertextIndex);

		Triangle currentTriangle = new Triangle(pointA.nodeVertextIndex, pointB.nodeVertextIndex, pointC.nodeVertextIndex);

		AddTriangleToDictionaryList(currentTriangle.indexOfVertexA, currentTriangle);
		AddTriangleToDictionaryList(currentTriangle.indexOfVertexB, currentTriangle);
		AddTriangleToDictionaryList(currentTriangle.indexOfVertexC, currentTriangle);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// AddTriangleToDictionaryList                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void AddTriangleToDictionaryList(int dictionaryIndex, Triangle triangle)
	{
		if (dictionaryOfTriangles.ContainsKey(dictionaryIndex))
		{
			dictionaryOfTriangles[dictionaryIndex].Add(triangle);
		}
		else
		{
			List<Triangle> tempTriangleList = new List<Triangle>();

			tempTriangleList.Add(triangle);
			dictionaryOfTriangles.Add(dictionaryIndex, tempTriangleList);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// FollowOutlineOfMesh                                                                                                                   //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void FollowOutlineOfMesh(int vertexIndex, int outlineIndex)
	{
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if (nextVertexIndex != -1)
		{
			FollowOutlineOfMesh(nextVertexIndex, outlineIndex);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GetConnectedOutlineVertex                                                                                                             //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	int GetConnectedOutlineVertex(int vertexIndex)
	{
		List<Triangle> trianglesContainingVertex = dictionaryOfTriangles[vertexIndex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++)
		{
			Triangle triangle = trianglesContainingVertex[i];

			for (int j = 0; j < 3; j++)
			{
				int vertexB = triangle[j];
				if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
				{
					if (IsOutlineEdge(vertexIndex, vertexB))
					{
						return vertexB;
					}
				}
			}
		}

		return -1;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// IsOutlineEdge                                                                                                                         //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to calculate is an edge is an outline edge. See the discription in the report to see how this is done                        //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	bool IsOutlineEdge(int vertexA, int vertexB)
	{
		List<Triangle> trianglesContainingVertexA = dictionaryOfTriangles[vertexA];
		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i++)
		{
			if (trianglesContainingVertexA[i].Contains(vertexB))
			{
				sharedTriangleCount++;
				if (sharedTriangleCount > 1)
				{
					break;
				}
			}
		}
		return sharedTriangleCount == 1;
	}



}