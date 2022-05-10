using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************************************************************//
//  File:   CADungeonMaker.cs                                                                                                                //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Main cellular automta map generator class                                                                                        //
//                                                                                                                                           //
//  Notes:  The algorithm starts with a random configurations of walls and spaces and number of smoothing iterations are done over the       //
//          entire map which makes each tile more like its neighbour. It when uses a algorithm called "Marching Squares" takes the shapes    //
//          generated and turns them into meshes for the walls. Each block in the array is either a wall (1) or not a wall (0). Each block   //
//          is then shrunk towards thier own centre so each block represents a coner of a square. Each block in the square represents 1      //
//          digit of a 4 digit binary number. If the top left block was a wall and the other 3 where empty, the binary number would be       //
//          1000 which in decimal is 8. so:                                                                                                  //
//                                                                                                                                           //
//          1*****0                        0*****1                       0*****0                      1*****1                                //
//          *     *                        *     *                       *     *                      *     *                                //
//          *     *                        *     *                       *     *                      *     *                                //
//          *     *   = 1000 = 8           *     *   = 0100 = 4          *     *   = 0011 = 3         *     *   = 0111 = 7                   //
//          *     *                        *     *                       *     *                      *     *                                //
//          *     *                        *     *                       *     *                      *     *                                //
//          0*****0                        0*****0                       1*****1                      1*****1                                //
//                                                                                                                                           //
//          and so on. There are 16 different combinations that can be done withe walls in the corner. These numbers from 0-15 can be used   //
//          to tell us what configuration of ons and offs we have and each configuration will represent a certain mesh. For example:         //
//                                                                                                                                           //
//          1*****0                        0*****1                       0*****0                      0*****1                                //
//          ****  *                        *  ****                       *     *                      *  ****                                //
//          ***   *                        *   ***                       *     *                      * *****                                //
//          **    *   = 1000 = 8           *    **   = 0100 = 4          *******   = 0011 = 3         *******   = 0111 = 7                   //
//          *     *                        *     *                       *******                      *******                                //
//          *     *                        *     *                       *******                      *******                                //
//          0*****0                        0*****0                       1*****1                      1*****1                                //
//                                                                                                                                           //
//                                                                                                                                           //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class CADungeonMaker : MonoBehaviour
{
	//***************************************************************************************************************************************//
	//	Structures                                                                                                                           //
	//***************************************************************************************************************************************//

	struct Coordinates
	{
		public int dungeonTileX;
		public int dungeonTileY;

		public Coordinates(int xPos, int yPos)
		{
			dungeonTileX = xPos;
			dungeonTileY = yPos;
		}
	}

	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//


	public int    dungeonWidth;    // Width of the dungeon
	public int    dungeonLength;   // Height of the dungeon
	public string randomSeed;      // Seed to stop program from generating same map each time
	public bool   useRandomSeed;   // Can switch random seed on and off in the inspector

	[Range(40, 55)]
	public int    wallFillPercent; // Loose guideline of how much of the map should be filled with walls
								   // Originally set to a range of 0 - 100 but may be modified in the inspector
								   // for the final release

	public int totalNumberOfEnemies = 0;         // Number of enemies randomly generated in the dungeon
	public int totalNumberOfGems = 0;


	public Slider dungeonWidthSlider;
	public Slider dungeonLengthSlider;
	public Slider wallFillPercentageSlider;

	public GameObject floor;
	public GameObject celing;

	public GameObject player;

	public GameObject enemy1;
	public GameObject enemy2;

	public GameObject cage1;                     // Prisoner cage model
	public GameObject crate1;                    // Set of wooden crates

	public GameObject gem1;                      // Collectable gems the player must collect to complete the level
	public GameObject ammoBook1;                 // Ammo the player can collect to shoot enemies

	public GameObject torch1;

	public Canvas gameOverCanvas;

	public Text totalGems;
	public Text totalEnemies;

	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	int[,] dungeonMap; // 2D array of ints that defines the map
					   // If map position = 0, will be empty tile
					   // If map position = 1. it is a wall

	bool notFirstGeneration = false;

	GameObject[] cages;
	GameObject[] crates;
	GameObject[] gems;
	GameObject[] books;
	GameObject[] torches;
	GameObject[] enemySolider;
	GameObject[] enemyWizard;


	//***************************************************************************************************************************************//
	//	Constants                                                                                                                            //
	//***************************************************************************************************************************************//

	const int smoothAmount = 5;
	const int maxNumWallTiles = 4;

	//***************************************************************************************************************************************//
	//	DungeonCave Class                                                                                                                    //
	//***************************************************************************************************************************************//

	class DungeonCave : IComparable<DungeonCave>
	{
		//***********************************************************************************************************************************//
		//	DungeonCave Class Variables                                                                                                      //
		//***********************************************************************************************************************************//

		public List<Coordinates> dungeonTile;
		public List<Coordinates> dungeonEdgeTile;
		public List<DungeonCave> connectedDungeonCaves;
		public int               sizeOfCave;
		public bool              isReachableFromMasterCave;
		public bool              isMasterCave;

		//***********************************************************************************************************************************//
		//	DungeonCave Class Constructors                                                                                                   //
		//***********************************************************************************************************************************//

		public DungeonCave()
		{

		}

		public DungeonCave(List<Coordinates> caveTiles, int[,] caveMap)
		{
			dungeonTile = caveTiles;
			sizeOfCave = dungeonTile.Count;

			connectedDungeonCaves = new List<DungeonCave>();
			dungeonEdgeTile = new List<Coordinates>();

			foreach (Coordinates tile in dungeonTile)
			{
				for (int x = tile.dungeonTileX - 1; x <= tile.dungeonTileX + 1; x++)
				{
					for (int y = tile.dungeonTileY - 1; y <= tile.dungeonTileY + 1; y++)
					{
						// Exclude the diagonal neighbours
						if (x == tile.dungeonTileX || y == tile.dungeonTileY)
						{
							if (caveMap[x, y] == 1)
							{
								dungeonEdgeTile.Add(tile);
							}
						}
					}
				}
			}
		}

		//***********************************************************************************************************************************//
		//	DungeonCave Class functions                                                                                                      //
		//***********************************************************************************************************************************//

		//-----------------------------------------------------------------------------------------------------------------------------------//
		// SetReachableFromMainCave                                                                                                          //
		// ----------------------------------------------------------------------------------------------------------------------------------//
		//                                                                                                                                   //
		//                                                                                                                                   //
		// ----------------------------------------------------------------------------------------------------------------------------------//

		public void SetReachableFromMainCave()
		{
			if (!isReachableFromMasterCave)
			{
				isReachableFromMasterCave = true;
				foreach (DungeonCave connectedCave in connectedDungeonCaves)
				{
					connectedCave.SetReachableFromMainCave();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------//
		// LinkCaves                                                                                                                         //
		// ----------------------------------------------------------------------------------------------------------------------------------//
		//                                                                                                                                   //
		//                                                                                                                                   //
		// ----------------------------------------------------------------------------------------------------------------------------------//

		public static void LinkCaves(DungeonCave caveA, DungeonCave caveB)
		{
			if (caveA.isReachableFromMasterCave)
			{
				caveB.SetReachableFromMainCave();
			}
			else if (caveB.isReachableFromMasterCave)
			{
				caveA.SetReachableFromMainCave();
			}

			caveA.connectedDungeonCaves.Add(caveB);
			caveB.connectedDungeonCaves.Add(caveA);

		}

		//-----------------------------------------------------------------------------------------------------------------------------------//
		// IsLinkedTogether                                                                                                                  //
		// ----------------------------------------------------------------------------------------------------------------------------------//
		//                                                                                                                                   //
		//                                                                                                                                   //
		// ----------------------------------------------------------------------------------------------------------------------------------//

		public bool IsLinkedTogether(DungeonCave cave)
		{
			return connectedDungeonCaves.Contains(cave);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------//
		// CompareTo                                                                                                                         //
		// ----------------------------------------------------------------------------------------------------------------------------------//
		//                                                                                                                                   //
		//                                                                                                                                   //
		// ----------------------------------------------------------------------------------------------------------------------------------//

		public int CompareTo(DungeonCave cave)
		{
			return cave.sizeOfCave.CompareTo(sizeOfCave);
		}

	} // End of DungeonCave class

	//***************************************************************************************************************************************//
	//	Start Function                                                                                                                       //
	//***************************************************************************************************************************************//

	void Start()
	{
		gameOverCanvas.enabled = false;

		//CreateNewDungeonMap();
	}

	//***************************************************************************************************************************************//
	//	Update Function                                                                                                                      //
	//***************************************************************************************************************************************//

	private void Update()
	{
		dungeonWidth = (int)dungeonWidthSlider.value;
		dungeonLength = (int)dungeonLengthSlider.value;
		wallFillPercent = (int)wallFillPercentageSlider.value;

		totalGems.text = totalNumberOfGems.ToString();
		totalEnemies.text = totalNumberOfEnemies.ToString();

	}

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CreateNewDungeonMap                                                                                                                   //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to generate a map firstly with random walls and spaces.  It then goes onto smooth the map to generate cave like rooms        //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void CreateNewDungeonMap()
	{
		// Delete all previous Game Objects when a new dungeon is created

		cages = GameObject.FindGameObjectsWithTag("Cage");
		crates = GameObject.FindGameObjectsWithTag("Crate");
		gems = GameObject.FindGameObjectsWithTag("Gem");
		books = GameObject.FindGameObjectsWithTag("Book");
		torches = GameObject.FindGameObjectsWithTag("Torch");
		enemySolider = GameObject.FindGameObjectsWithTag("EnemySolider");
		enemyWizard = GameObject.FindGameObjectsWithTag("EnemyWizard");
		foreach (GameObject cage    in cages)        Destroy(cage);
		foreach (GameObject crate   in crates)       Destroy(crate);
		foreach (GameObject gem     in gems)         Destroy(gem);
		foreach (GameObject book    in books)        Destroy(book);
		foreach (GameObject torch   in torches)      Destroy(torch);
		foreach (GameObject solider in enemySolider) Destroy(solider);
		foreach (GameObject wizard  in enemyWizard)  Destroy(wizard);




		// Create new map 
		dungeonMap = new int[dungeonWidth, dungeonLength];

		// Randomly fill the map with walls and spaces
		// Fill amount is determined by the randomFillPercent variable

		if (useRandomSeed)
		{
			randomSeed = Time.time.ToString();
		}

		// The next line returns a unique hash code for the seed
		System.Random randomSeedAmount = new System.Random(randomSeed.GetHashCode());

		// Loop through each of the tiles in the map
		for (int x = 0; x < dungeonWidth; x++)
		{
			for (int y = 0; y < dungeonLength; y++)
			{
				// Is the space a wall around the border of the map?
				if (x == 0 || x == dungeonWidth - 1 || y == 0 || y == dungeonLength - 1)
				{
					// Fill border with a wall
					dungeonMap[x, y] = 1;
				}
				else
				{
					// Populate the map with random walls
					// chhose a number between 0 and 100 and check if it is less than the
					// Random fill percent. If it is then fill the space with a wall
					// If not, will with a blank tile

					if (randomSeedAmount.Next(0, 100) < wallFillPercent)
					{
						dungeonMap[x, y] = 1;
					}
					else
					{
						dungeonMap[x, y] = 0;

					}

				}
			}
		}

		// Smooth the map

		// Examine every tile
		for (int i = 0; i < 5; i++)
		{
			for (int x = 0; x < dungeonWidth; x++)
			{
				for (int y = 0; y < dungeonLength; y++)
				{
					// Get the amount of neighbouring tiles this tile has that are walls
					int neighbourWallSections = CalculateNeighbouringWalls(x, y);

					// If the number of tiles is greater than 4 then populate it will a wall
					if (neighbourWallSections > maxNumWallTiles)
                    {
						dungeonMap[x, y] = 1;
					}
					// else populate is with an empty space
					else if (neighbourWallSections < maxNumWallTiles)
                    {
						dungeonMap[x, y] = 0;
					}

				}
			}
		}

		// Process the map

		List<List<Coordinates>> wallSection = GetWallSections(1);

		// Remove any wall regions that are made up of less than 50 tiles
		int sizeOfWallBoundary = 50;

		foreach (List<Coordinates> wallArea in wallSection)
		{
			if (wallArea.Count < sizeOfWallBoundary)
			{
				// Go through every tile in that region
				foreach (Coordinates tile in wallArea)
				{
					// Set it to an empty space
					dungeonMap[tile.dungeonTileX, tile.dungeonTileY] = 0;
				}
			}
		}

		List<List<Coordinates>> caveRegions = GetWallSections(0);

		int sizeOfCaveBoundary = 50;
		List<DungeonCave> cavesLeft = new List<DungeonCave>();

		foreach (List<Coordinates> cave in caveRegions)
		{
			if (cave.Count < sizeOfCaveBoundary)
			{
				// Go through every tile in that region
				foreach (Coordinates tile in cave)
				{
					dungeonMap[tile.dungeonTileX, tile.dungeonTileY] = 1;
				}
			}
			else
			{
				cavesLeft.Add(new DungeonCave(cave, dungeonMap));
			}
		}


		cavesLeft.Sort();

		cavesLeft[0].isMasterCave = true;
		cavesLeft[0].isReachableFromMasterCave = true;

		AttachClosestCavesTogether(cavesLeft);


		int dungeonBoundarySize = 1;

		int[,] boundaryMap = new int[dungeonWidth + dungeonBoundarySize * 2, dungeonLength + dungeonBoundarySize * 2];

		for (int x = 0; x < boundaryMap.GetLength(0); x++)
		{
			for (int y = 0; y < boundaryMap.GetLength(1); y++)
			{
				int totalDungeonWidth = dungeonWidth + dungeonBoundarySize;
				int totalDungeonLength = dungeonLength + dungeonBoundarySize;


				if (x >= dungeonBoundarySize && x < totalDungeonWidth &&
					y >= dungeonBoundarySize && y < totalDungeonLength)
				{
					boundaryMap[x, y] = dungeonMap[x - dungeonBoundarySize, y - dungeonBoundarySize];
				}
				else
				{
					boundaryMap[x, y] = 1;
				}
			}
		}

		CAMeshCreator dungeonMeshCreator = GetComponent<CAMeshCreator>();
		dungeonMeshCreator.BuildDungeonMesh(boundaryMap, 1);

		// Position the player in centre of the first cave

		DungeonCave playerSpawnRoom = cavesLeft[0];
		int randomTile = UnityEngine.Random.Range(0, playerSpawnRoom.dungeonTile.Count);
		player.transform.position = CoordToWorldPoint(playerSpawnRoom.dungeonTile[randomTile]);

		

		int numOfCaves = cavesLeft.Count;
		
		print(numOfCaves);

		// Cages

		for (int i = 1; i < numOfCaves; i++)
		{
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject cage = Instantiate(cage1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.identity);
			cage.transform.position = new Vector3(cage.transform.position.x, 0.0f, cage.transform.position.z);
			cage.tag = "Cage";

		}

		// Crates

		for (int i = 1; i < numOfCaves; i++)
		{
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject crate = Instantiate(crate1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.identity);
			crate.transform.position = new Vector3(crate.transform.position.x, 0.0f, crate.transform.position.z);
			crate.tag = "Crate";
		}

		// Gems

		for (int i = 1; i < numOfCaves; i++)
		{
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject gem = Instantiate(gem1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.Euler(90.0f, 0.0f, 0.0f));

			

			gem.transform.position = new Vector3(gem.transform.position.x, 0.5f, gem.transform.position.z);
			gem.tag = "Gem";

			totalNumberOfGems++;
		}


		// Books

		for (int i = 1; i < numOfCaves; i++)
		{
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject book = Instantiate(ammoBook1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.Euler(90.0f, 0.0f, 0.0f));
			book.transform.position = new Vector3(book.transform.position.x, 0.5f, book.transform.position.z);
			book.tag = "Book";

		}

		// Torches

		for (int i = 1; i < numOfCaves; i++)
		{
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject torch = Instantiate(torch1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.identity);
			torch.transform.position = new Vector3(torch.transform.position.x, 0.0f, torch.transform.position.z);
			torch.tag = "Torch";


		}


		// Enemies

		for (int i = 1; i < numOfCaves; i++)
        {
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject skellySolider = Instantiate(enemy1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.identity);
			skellySolider.transform.position = new Vector3(skellySolider.transform.position.x, 0.0f, skellySolider.transform.position.z);

			// Set the target of the enemy as the player
			skellySolider.GetComponent<Enemy>().target = player.transform;
			// Set the player as the unit to be attacked
			skellySolider.GetComponent<EnemyAttack>().target = player.transform;
			skellySolider.tag = "EnemySolider";

			totalNumberOfEnemies++;
		}

		for (int i = 1; i < numOfCaves; i++)
		{
			DungeonCave currentCave = cavesLeft[i];
			int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
			GameObject skellyMage = Instantiate(enemy2, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.identity);
			skellyMage.transform.position = new Vector3(skellyMage.transform.position.x, 0.0f, skellyMage.transform.position.z);

			// Set the target of the enemy as the player
			skellyMage.GetComponent<Enemy>().target = player.transform;
			// Set the player as the unit to be attacked
			skellyMage.GetComponent<EnemyAttack>().target = player.transform;
			skellyMage.tag = "EnemyWizard";

			totalNumberOfEnemies++;
		}



		notFirstGeneration = true;

		//DungeonCave currentCave = cavesLeft[0];
		//int randomTile2 = UnityEngine.Random.Range(0, currentCave.dungeonTile.Count);
		//Instantiate(enemy1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), enemy1.transform.rotation);

		//GameObject skellySolider1 = Instantiate(enemy1, CoordToWorldPoint(currentCave.dungeonTile[randomTile2]), Quaternion.identity);

		//skellySolider1.transform.position = new Vector3(skellySolider1.transform.position.x, -5.0f, skellySolider1.transform.position.x);

	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// AttachClosestCavesTogether                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void AttachClosestCavesTogether(List<DungeonCave> caves, bool applyAPathFromMainCave = false)
	{
		int distance = 0;


		List<DungeonCave> caveList1 = new List<DungeonCave>();
		List<DungeonCave> caveList2 = new List<DungeonCave>();

		if (applyAPathFromMainCave)
		{
			foreach (DungeonCave cave in caves)
			{
				if (cave.isReachableFromMasterCave)
				{
					caveList2.Add(cave);
				}
				else
				{
					caveList1.Add(cave);
				}
			}
		}
		else
		{
			caveList1 = caves;
			caveList2 = caves;
		}

		Coordinates selectedTile1 = new Coordinates();
		Coordinates selectedTile2 = new Coordinates();
		DungeonCave selectedCave1 = new DungeonCave();
		DungeonCave selectedCave2 = new DungeonCave();
		bool attachmentBetweenCavesFound = false;

		foreach (DungeonCave cave1 in caveList1)
		{
			if (!applyAPathFromMainCave)
			{
				attachmentBetweenCavesFound = false;

				if (cave1.connectedDungeonCaves.Count > 0)
				{
					continue;
				}
			}

			foreach (DungeonCave cave2 in caveList2)
			{
				if (cave1 == cave2 || cave1.IsLinkedTogether(cave2))
				{
					continue;
				}

				// Look at the distance between the edge tiles between cave 1 and cave 2

				for (int section1Index = 0; section1Index < cave1.dungeonEdgeTile.Count; section1Index++)
				{
					for (int section2Index = 0; section2Index < cave2.dungeonEdgeTile.Count; section2Index++)
					{
						Coordinates sectionToBeChecked1 = cave1.dungeonEdgeTile[section1Index];
						Coordinates sectionToBeChecked2 = cave2.dungeonEdgeTile[section2Index];

						int xDistance = sectionToBeChecked1.dungeonTileX - sectionToBeChecked2.dungeonTileX;
						int yDistance = sectionToBeChecked1.dungeonTileY - sectionToBeChecked2.dungeonTileY;

						float totalDistance = Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2);

						int distanceBetweenCave = (int)(totalDistance);

						if (distanceBetweenCave < distance || !attachmentBetweenCavesFound)
						{
							distance = distanceBetweenCave;
							attachmentBetweenCavesFound = true;
							selectedTile1 = sectionToBeChecked1;
							selectedTile2 = sectionToBeChecked2;
							selectedCave1 = cave1;
							selectedCave2 = cave2;
						}
					}
				}
			}

			if (attachmentBetweenCavesFound && !applyAPathFromMainCave)
			{
				CreatePathBetweenCaves(selectedCave1, selectedCave2, selectedTile1, selectedTile2);
			}

		}

		if (attachmentBetweenCavesFound && applyAPathFromMainCave)
		{
			CreatePathBetweenCaves(selectedCave1, selectedCave2, selectedTile1, selectedTile2);
			AttachClosestCavesTogether(caves, true);
		}

		if (!applyAPathFromMainCave)
		{
			AttachClosestCavesTogether(caves, true);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CreatePathBetweenCaves                                                                                                                //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	void CreatePathBetweenCaves(DungeonCave cave1, DungeonCave cave2, Coordinates tile1, Coordinates tile2)
	{
		DungeonCave.LinkCaves(cave1, cave2);
		//Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

		List<Coordinates> lineBetweenCaves = CreateLineBetweenCaves(tile1, tile2);

		foreach (Coordinates circle in lineBetweenCaves)
		{
			for (int x = -5; x <= 5; x++)
			{
				for (int y = -5; y <= 5; y++)
				{
					if (x * x + y * y <= 5 * 5)
					{
						int drawX = circle.dungeonTileX + x;
						int drawY = circle.dungeonTileY + y;

						if (IsInMapRange(drawX, drawY))
						{
							dungeonMap[drawX, drawY] = 0;
						}
					}
				}
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CreateLineBetweenCaves                                                                                                                //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	List<Coordinates> CreateLineBetweenCaves(Coordinates caveLocationFrom, Coordinates caveLocationTo)
	{
		List<Coordinates> lineBetweenCaves = new List<Coordinates>();
		bool isInverted = false;

		int xPosition = caveLocationFrom.dungeonTileX;
		int yPosition = caveLocationFrom.dungeonTileY;

		int dxPosition = caveLocationTo.dungeonTileX - caveLocationFrom.dungeonTileX;
		int dyPosition = caveLocationTo.dungeonTileY - caveLocationFrom.dungeonTileY;


		int section = Math.Sign(dxPosition);

		int gradientSection = Math.Sign(dyPosition);

		int longestPossibleLength  = Mathf.Abs(dxPosition);
		int shortestPossibleLength = Mathf.Abs(dyPosition);

		if (longestPossibleLength < shortestPossibleLength)
		{
			isInverted = true;
			longestPossibleLength = Mathf.Abs(dyPosition);
			shortestPossibleLength = Mathf.Abs(dxPosition);

			section = Math.Sign(dyPosition);
			gradientSection = Math.Sign(dxPosition);
		}

		int gradientAccumulation = longestPossibleLength / 2;

		for (int i = 0; i < longestPossibleLength; i++)
		{
			lineBetweenCaves.Add(new Coordinates(xPosition, yPosition));

			if (isInverted)
			{
				yPosition += section;
			}
			else
			{
				xPosition += section;
			}

			gradientAccumulation += shortestPossibleLength;

			if (gradientAccumulation >= longestPossibleLength)
			{
				if (isInverted)
				{
					xPosition += gradientSection;
				}
				else
				{
					yPosition += gradientSection;
				}
				gradientAccumulation -= longestPossibleLength;
			}
		}

		return lineBetweenCaves;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GetWallSections                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	List<List<Coordinates>> GetWallSections(int typeOfTile)
	{
		List<List<Coordinates>> sections = new List<List<Coordinates>>();

		int[,] mapAreas = new int[dungeonWidth, dungeonLength];

		for (int x = 0; x < dungeonWidth; x++)
		{
			for (int y = 0; y < dungeonLength; y++)
			{
				if (mapAreas[x, y] == 0 && dungeonMap[x, y] == typeOfTile)
				{
					List<Coordinates> newCave = GetCaveSections(x, y);
					sections.Add(newCave);

					// mark all the sections in the cave as looked at
					foreach (Coordinates section in newCave)
					{
						mapAreas[section.dungeonTileX, section.dungeonTileY] = 1;
					}
				}
			}
		}

		return sections;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// GetCaveSections                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	List<Coordinates> GetCaveSections(int startXPosition, int startYPosition)
	{
		List<Coordinates> sections = new List<Coordinates>();

		int[,] mapAreas = new int[dungeonWidth, dungeonLength];

		int sectionType = dungeonMap[startXPosition, startYPosition];

		Queue<Coordinates> queueOfCoordinates = new Queue<Coordinates>();

		queueOfCoordinates.Enqueue(new Coordinates(startXPosition, startYPosition));

		mapAreas[startXPosition, startYPosition] = 1;

		while (queueOfCoordinates.Count > 0)
		{
			Coordinates section = queueOfCoordinates.Dequeue();

			sections.Add(section);

			// Look at adjacent tiles
			for (int x = section.dungeonTileX - 1; x <= section.dungeonTileX + 1; x++)
			{
				for (int y = section.dungeonTileY - 1; y <= section.dungeonTileY + 1; y++)
				{
					// Discard diagonal tiles
					if (IsInMapRange(x, y) && (y == section.dungeonTileY || x == section.dungeonTileX))
					{
						if (mapAreas[x, y] == 0 && dungeonMap[x, y] == sectionType)
						{
							mapAreas[x, y] = 1;
							queueOfCoordinates.Enqueue(new Coordinates(x, y));
						}
					}
				}
			}
		}
		return sections;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CalculateNeighbouringWalls                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that returns the number of tiles around a specific tile that are walls (1). It takes in the length and width of the grid     //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	int CalculateNeighbouringWalls(int xGridPosition, int yGridPosition)
	{
		// Create wall count
		int numberOfWalls = 0;

		// Loop through a 3x3 grid around each tile individually
		for (int wallNeighbourX = xGridPosition - 1; wallNeighbourX <= xGridPosition + 1; wallNeighbourX++)
		{
			for (int wallNeighbourY = yGridPosition - 1; wallNeighbourY <= yGridPosition + 1; wallNeighbourY++)
			{
				// Check that the neightbour is actuall inside the map itself
				if (IsInMapRange(wallNeighbourX, wallNeighbourY))
				{
					// Exclude the current tile in the middle
					if (wallNeighbourX != xGridPosition || wallNeighbourY != yGridPosition)
					{
						// If it is wall, the count will increment by 1
						numberOfWalls += dungeonMap[wallNeighbourX, wallNeighbourY];
					}
				}
				else
				{
					// If this is an edge tile then increase the wall count by 1
					numberOfWalls++;
				}
			}
		}

		return numberOfWalls;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CoordToWorldPoint                                                                                                                     //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	//                                                                                                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	Vector3 CoordToWorldPoint(Coordinates tile)
	{
		return new Vector3(-dungeonWidth / 2 + .5f + tile.dungeonTileX, 2, -dungeonLength / 2 + .5f + tile.dungeonTileY);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// IsInMapRange                                                                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to check if a position on the map is actually within the map boundarys. Returns true if it is and false if not               //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//


	bool IsInMapRange(int x, int y)
	{
		return x >= 0 && x < dungeonWidth && y >= 0 && y < dungeonLength;
	}

}

