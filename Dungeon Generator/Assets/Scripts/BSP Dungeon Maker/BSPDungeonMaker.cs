using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************************************************************//
//  File:   BSPDungeonMaker.cs                                                                                                               //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   BSP Main Dungeon Maker Class                                                                                                     //
//                                                                                                                                           //
//  Notes:  Creates the dungeon within the world.                                                                                            //
//          Floors of each room are created using meshes and the walls are generated around the meshes to create rooms and cooridors.        //
//          Two enemies are spawned inside each room, and random objects are also placed as obsticles to help demonstrate the A*             //
//          pathfinding algorithm.                                                                                                           //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPDungeonMaker : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public int widthOfDungeon;                   // Overall width of the dungeon (max 100)
    public int lengthOfDungeon;                  // Overall length of the dungeon (max 100)
    public int minimumWidthOfRoom;               // Minimum width of each room
    public int minimumLengthOfRoom;              // Minimum length of each room
    public int maximumNumOfInterations;          // Maximum number of rooms within the dungeon
    public int widthOfCorridor;                  // width of the generated cooridors (4-6)
    public int roomOffset;                       // Value off offset in which the room is placed inside the partition
    public int totalNumberOfEnemies = 0;         // Number of enemies randomly generated in the dungeon
    public int totalNumberOfGems = 0;
    [Range(0.0f, 0.3f)] 
    public float bottomCornerModifierOfRoom;     // Modifiy the space between the bottom corner of the generated partition and the room
    [Range(0.7f, 1.0f)] 
    public float topCornerModifierOfRoom;        // Modifiy the space between the top corner of the generated partition and the room
    [Range(0, 2)] 

    public Text widthOfDungeonText;              // Text field used in the UI for width of dungeon
    public Text lengthOfDungeonText;             // Text field used in the UI for length of dungeon
    public Text minimumWidthOfRoomText;          // Text field used in the UI for minimum width of a room
    public Text minimumLengthOfRoomText;         // Text field used in the UI for minimum length of a room
    public Text maximumNumOfInterationsText;     // Text field used in the UI for max number of interations
    public Text widthOfCorridorText;             // Text field used in the UI for width of a cooridor
    public Text bottomCornerModifierOfRoomText;  // Text field used in the UI for bottom corner modifier
    public Text topCornerModifierOfRoomText;     // Text field used in the UI for top cornor modifier
    public Text roomOffsetText;                  // Text field used in the UI for room offset
    public Text totalNumOfEnemiesText;           // Text field used in the player UI to show total number of enemies generated
    public Text totalNumOfGemsText;              // Text field used in the player UI to show total number of gems generated

    public Material floorMaterial;               // Material used for the dungeon floor
    public Material ceilingMaterial;             // Material used for the dungeon ceiling

    public GameObject horizontalWall1;           // Horizontal wall model 1
    public GameObject horizontalWall2;           // Horizontal wall model 2
    public GameObject horizontalWall3;           // Horizontal wall model 3
    public GameObject verticalWall1;             // Vertical wall model 1
    public GameObject verticalWall2;             // Vertical wall model 2
    public GameObject verticalWall3;             // Vertical wall model 3
    public GameObject pillar1;                   // Dungeon pillar model
    public GameObject torch1;                    // Torch model used in the corner of the rooms
    public GameObject barrelSet1;                // Barrel model
    public GameObject cage1;                     // Prisoner cage model
    public GameObject crate1;                    // Set of wooden crates
    public GameObject enemy1;                    // Skeleton warrior model
    public GameObject enemy2;                    // Skeleton mage model
    public GameObject gem1;                      // Collectable gems the player must collect to complete the level
    public GameObject ammoBook1;                 // Ammo the player can collect to shoot enemies
    public GameObject generatingDungeonAlert;    // Generating Dungeon UI alert
    public GameObject generateDungeonButton;     // Generate dungeon UI button
    public GameObject playButton;                // Play dungeon UI button
    public GameObject player;                    // Player controller gameobject


    public int GetWidthOfDungeon()               // Allows other classes to access the width of the dungeon
    {
        return widthOfDungeon;
    }


    public int GetLengthOfDungeon()               // Allows other classes to access the length of the dungeon
    {
        return lengthOfDungeon;
    }

    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    List<Vector3Int> positionsOfVerticalDoors;      // List to store X, Y and Z positions of the gaps used to access the vertical cooridors
    List<Vector3Int> positionsOfHorizontalDoors;    // List to store X, Y and Z positions of the gaps used to access the horizonal cooridors
    List<Vector3Int> positionsOfVerticalWalls;      // List to store X, Y and Z positions of the vertical walls
    List<Vector3Int> positionsOfHorizontalWalls;    // List to store X, Y and Z positions of the horizontal walls

    //***************************************************************************************************************************************//
    //	Update Function - Called once per frame                                                                                              //
    //***************************************************************************************************************************************//

    private void Update()
    {
        
        widthOfDungeon             = int.Parse   (widthOfDungeonText.text);             // Updates width of dungeon in the generator UI
        lengthOfDungeon            = int.Parse   (lengthOfDungeonText.text);            // Updates length of dungeon in the generator UI
        minimumWidthOfRoom         = int.Parse   (minimumWidthOfRoomText.text);         // Updates min room width of dungeon in the generator UI
        minimumLengthOfRoom        = int.Parse   (minimumLengthOfRoomText.text);        // Updates min room length of dungeon in the generator UI
        maximumNumOfInterations    = int.Parse   (maximumNumOfInterationsText.text);    // Updates max interations in the generator UI
        widthOfCorridor            = int.Parse   (widthOfCorridorText.text);            // Updates width of cooridor in the generator UI
        bottomCornerModifierOfRoom = float.Parse (bottomCornerModifierOfRoomText.text); // Updates borrom corner modifier in the generator UI
        topCornerModifierOfRoom    = float.Parse (topCornerModifierOfRoomText.text);    // Updates top corner modifier in the generator UI
        roomOffset                 = int.Parse   (roomOffsetText.text);                 // Updates room offset in the generator UI

        totalNumOfEnemiesText.text = totalNumberOfEnemies.ToString();                   // Updates the number of enemies still alive
        totalNumOfGemsText.text    = totalNumberOfGems.ToString();                         // Updates the number of gems collected


    } // End of Update Function


    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateNewDungeon                                                                                                                      //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Creates a new randomly generated BSP dungeon. This funtion is called when the user clicks on the "Generate Dungeon" button in the     //
    // BSP dungeon generator UI.                                                                                                             //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    public void CreateNewDungeon()
    {
        
        // Move the player away from the dungeon generation area.
        // This stops the enemies spoting the player if they are randomly generated within chase distance of the players position

        player.transform.position = new Vector3(-100.0f, 0.0f, -100.0f);

        // Delete all previous children in the BSP tree (if the user has previously generated a dungeon)

        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        // Allocate memory for the Lists used for the dungeon generation

        positionsOfHorizontalWalls = new List<Vector3Int>();
        positionsOfVerticalWalls   = new List<Vector3Int>();
        positionsOfHorizontalDoors = new List<Vector3Int>();
        positionsOfVerticalDoors   = new List<Vector3Int>();

        // Call Dungeon builder class to build the dungeon, sending in the dungeons dimentions

        BSPDungeonBuilder dungeonBuilder = new BSPDungeonBuilder(widthOfDungeon, lengthOfDungeon);

        // Populate the room list

        var roomList = dungeonBuilder.ProcessDungeon(maximumNumOfInterations, minimumWidthOfRoom, minimumLengthOfRoom,
                                                     bottomCornerModifierOfRoom, topCornerModifierOfRoom, roomOffset, widthOfCorridor);

        // Create parent game objects to store the different types of dungeon objects in the inspector

        GameObject wallParent     = new GameObject("WallParent");
        GameObject torchParent    = new GameObject("TorchParent");
        GameObject barrelParent   = new GameObject("BarrelParent");
        GameObject cageParent     = new GameObject("CageParent");
        GameObject crateParent    = new GameObject("CrateParent");
        GameObject enemyParent    = new GameObject("EnemyParent");
        GameObject bookAmmoParent = new GameObject("BookAmmoParent");
        GameObject gemParent      = new GameObject("GemParent");

        wallParent.transform.parent    = transform;
        torchParent.transform.parent   = transform;
        barrelParent.transform.parent  = transform;
        cageParent.transform.parent    = transform;
        crateParent.transform.parent   = transform;
        enemyParent.transform.parent   = transform;
        gemParent.transform.parent     = transform;
        bookAmmoParent.transform.parent = transform;

        // Start Co-routine to populate the dungeon floor, walls and objects over a short amount of time
        StartCoroutine(CreateDungeon(roomList, torchParent, barrelParent, cageParent, wallParent, enemyParent, cageParent, gemParent, bookAmmoParent));

    } // End of CreateNewDungeon function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateDungeon Coroutine                                                                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Coroutine that is used to populate the dungeon with objects (floors, walls, torches, enemys, cages, barrles)                          //
    // Generation speed can be modified to make the generation process slow or fast depending on the dungeon size                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator CreateDungeon(List<BSPNode> roomList, GameObject torchParent, GameObject barrelParent, GameObject cageParent, GameObject wallParent, 
                                      GameObject enemyParent, GameObject crateParent, GameObject gemParent, GameObject bookAmmoParent)
    {
        // Dungeon generation time per frame (this may be added as an option for the user later on so they can see the generation processed
        // over time depending on thier choice of speed

        WaitForSeconds wait = new WaitForSeconds(0.15f);

        // Create the rooms and populate them with walls and objects
        
        for (int i = 0; i < roomList.Count; i++)
        {
            CreateNewFloorMesh   (roomList[i].bottomLeftRegionCorner,               0.0f, roomList[i].topRightRegionCorner);
            CreateNewCeilingMesh (roomList[i].bottomLeftRegionCorner,               1.9f, roomList[i].topRightRegionCorner);
            CreateTorches        (torchParent, roomList[i].bottomLeftRegionCorner,  1.0f, roomList[i].topRightRegionCorner);
            CreateBarrels        (barrelParent, roomList[i].bottomLeftRegionCorner, 1.0f, roomList[i].topRightRegionCorner);
            CreateCages          (cageParent, roomList[i].bottomLeftRegionCorner,   1.0f, roomList[i].topRightRegionCorner);
            CreateCrates         (crateParent, roomList[i].bottomLeftRegionCorner,  1.0f, roomList[i].topRightRegionCorner);

            yield return wait;
        }

        // Populate each room with enemies and items for player to collect 
        // (for loop starts at 1 so an enemy is not randomly generated in the first room where the player will be positioned

        for (int i = 1; i < roomList.Count; i++)
        {
            // Generate enemies
            CreateSoliderEnemy (enemyParent, roomList[i].bottomLeftRegionCorner, 1.0f, roomList[i].topRightRegionCorner);
            CreateMageEnemy    (enemyParent, roomList[i].bottomLeftRegionCorner, 1.0f, roomList[i].topRightRegionCorner);
            
            // Generate Collectable items
            CreateCollectableGems      (gemParent,      roomList[i].bottomLeftRegionCorner, 1.0f, roomList[i].topRightRegionCorner);
            CreateCollectableAmmoBooks (bookAmmoParent, roomList[i].bottomLeftRegionCorner, 1.0f, roomList[i].topRightRegionCorner);

            yield return wait;        
        }

        // Create the walls

        CreateWalls(wallParent);

        // Calculate mid point of the first generated room

        int playerStartX = (((roomList[0].topRightRegionCorner.x - roomList[0].bottomLeftRegionCorner.x) / 2) + roomList[0].bottomLeftRegionCorner.x) - 50;
        int playerStartZ = (((roomList[0].topRightRegionCorner.y - roomList[0].bottomLeftRegionCorner.y) / 2) + roomList[0].bottomLeftRegionCorner.y) - 50;

        // Set the players position in the centre of the first generated room
      
        player.transform.position = new Vector3(playerStartX, 0.5f, playerStartZ);

        // Activate the "Generating Dungeon" UI element
        generatingDungeonAlert.SetActive(false);
        
        // Deactivate "Generate Dungeon" Button
        generateDungeonButton.SetActive(true);

        // Deactivate the "Play Game" button
        playButton.transform.position = new Vector3(1540, 140, 0);

    } // End of CreateNewDungeon function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateWalls                                                                                                                           //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to populate each randomly generated floor mesh with walls around the edge to create a room. There are three types of wall    //
    // tiles that the dungeon can randomly select. More may be added at a later date                                                         //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateWalls(GameObject wallParent)
    {
        // Place the horizontal walls

        foreach (var wallPosition in positionsOfHorizontalWalls)
        {
            float wallPosX = (float)wallPosition.x;
            float wallPosY = (float)wallPosition.y;
            float wallPosZ = (float)wallPosition.z + 0.5f;

            Vector3 newWallPos = new Vector3(wallPosX, wallPosY, wallPosZ);

            // Select a random wall piece, will also place a pillar at each side of the wall

            int wallChoice = UnityEngine.Random.Range(0, 3);


            if (wallChoice == 1) // Plain wall
            {
                CreateNewHorizontalWall(wallParent, newWallPos, verticalWall1, pillar1);
            }
            else if (wallChoice == 2) // Wall with arch
            {
                CreateNewHorizontalWall(wallParent, new Vector3(newWallPos.x - 0.02f, newWallPos.y, newWallPos.z), verticalWall2, pillar1);
            }
            else // 
            {
                CreateNewHorizontalWall(wallParent, newWallPos, verticalWall3, pillar1); // Wall with grate
            }
        }

        // Place the Vertical walls

        foreach (var wallPosition in positionsOfVerticalWalls)
        {
        
            float wallPosX = (float)wallPosition.x + 0.5f;
            float wallPosY = (float)wallPosition.y;
            float wallPosZ = (float)wallPosition.z;
        
            Vector3 newWallPos = new Vector3(wallPosX, wallPosY, wallPosZ);

            // Select a random wall piece, will also place a pillar at each side of the wall

            int wallChoice = UnityEngine.Random.Range(0, 3);
        
            if (wallChoice == 1) // Plain wall
            {
                CreateNewVerticalWall(wallParent, newWallPos, horizontalWall1, pillar1);
            }
            else if (wallChoice == 2) // Wall with arch
            {
                CreateNewVerticalWall(wallParent, new Vector3(newWallPos.x, newWallPos.y, newWallPos.z + 0.02f), horizontalWall2, pillar1);
            }
            else
            {
                CreateNewVerticalWall(wallParent, newWallPos, horizontalWall3, pillar1); // Wall with grate
            }
        }

    } // End of CreateWalls function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateNewHorizontalWall                                                                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that instantiates the horizontal wall pieces within the game world. These are randomly generated in the "CreateWalls"        //
    // function                                                                                                                              //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateNewHorizontalWall(GameObject wallParent, Vector3 wallPosition, GameObject wallPrefab, GameObject pillarPrefab)
    {
        // Create wall piece in the world
        GameObject newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
        
        newWall.layer = 9;        // Set layer to 9 (unwalkable mask, used for pathfinding)
        newWall.isStatic = true;  // Makes the wall static to stop it moving and player walking through

        // Create pillars at each end of the wall piece in the world
        Vector3 pillarPos = new Vector3(wallPosition.x, wallPosition.y, wallPosition.z + 0.5f);
        GameObject newPillar = Instantiate(pillarPrefab, pillarPos, Quaternion.identity, wallParent.transform);

        newPillar.layer = 9; // Set layer to 9 (unwalkable mask, used for pathfinding)

    } // End of CreateNewHorizontalWall function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateNewVerticalWall                                                                                                                 //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that instantiates the vertical wall pieces within the game world. These are randomly generated in the "CreateWalls"          //
    // function                                                                                                                              //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateNewVerticalWall(GameObject wallParent, Vector3 wallPosition, GameObject wallPrefab, GameObject pillarPrefab)
    {
        // Create wall piece in the world
        GameObject newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);

        newWall.layer = 9;        // Set layer to 9 (unwalkable mask, used for pathfinding)
        newWall.isStatic = true;  // Makes the wall static to stop it moving and player walking through

        // Create pillars at each end of the wall piece in the world
        Vector3 pillarPos = new Vector3(wallPosition.x - 0.5f, wallPosition.y, wallPosition.z);
        GameObject newPillar = Instantiate(pillarPrefab, pillarPos, Quaternion.identity, wallParent.transform);

        newPillar.layer = 9; // Set layer to 9 (unwalkable mask, used for pathfinding)

        newWall.transform.Rotate(0, 90, 0); // Rotate the wall piece so it is vertical in the game world

    } // End of CreateNewVerticalWall function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateNewFloorMesh                                                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that creates the floor meshes of the rooms in the world. Takes in the bottom left nad top right corners and created a mesh   //
    // for each floor.                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateNewFloorMesh(Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of room
        Vector3 topLeftPosition     = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPosition    = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create vertices array used to store the mesh information
        Vector3[] vertices = new Vector3[]
        {
            topLeftPosition,
            topRightPosition,
            bottomLeftPosition,
            bottomRightPosition
        };

        // Create uvs array used to store UV information
        Vector2[] uvs = new Vector2[vertices.Length];

        // Populate UV array
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        // Create Triangles array used for mesh
        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        // Create new mesh
        Mesh mesh = new Mesh();

        // Setup mesh
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // Instanciate the floor using generated mesh and positions
        GameObject floor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        floor.transform.position   = Vector3.zero;
        floor.transform.localScale = Vector3.one;
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = floorMaterial;
        floor.transform.parent = transform;

        floor.AddComponent<MeshCollider>(); // Add a collider to stop the player falling through the floor
        floor.isStatic = true;

        floor.layer = 10; // Set layer to fllor (used in pathfinding algorith)

        // These next for loops populate the lists of the wall and door positions into the wall lists arrays

        for (int row = (int)bottomLeftPosition.x; row < (int)bottomRightPosition.x; row++)
        {
            var positionOfWall = new Vector3(row, 0, bottomLeftPosition.z);
            AddPositionOfWallToList(positionOfWall, positionsOfVerticalWalls, positionsOfHorizontalDoors);
        }

        for (int row = (int)topLeftPosition.x; row < (int)topRightPosition.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightPosition.z);
            AddPositionOfWallToList(wallPosition, positionsOfVerticalWalls, positionsOfHorizontalDoors);
        }
        
        for (int column = (int)bottomLeftPosition.z; column < (int)topLeftPosition.z; column++)
        {
            var wallPosition = new Vector3(bottomLeftPosition.x, 0, column);
            AddPositionOfWallToList(wallPosition, positionsOfHorizontalWalls, positionsOfVerticalDoors);
        }
        
        for (int column = (int)bottomRightPosition.z; column < (int)topRightPosition.z; column++)
        {
            var wallPosition = new Vector3(bottomRightPosition.x, 0, column);
            AddPositionOfWallToList(wallPosition, positionsOfHorizontalWalls, positionsOfVerticalDoors);
        }

    } // End of CreateNewFloorMesh function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateNewCeilingMesh                                                                                                                  //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that creates the cliling meshes of the rooms in the world. Takes in the bottom left nad top right corners and created a mesh //
    // for each floor.                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateNewCeilingMesh(Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of room
        Vector3 topLeftPostition    = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPostition   = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create vertices array used to store the mesh information
        Vector3[] vertices = new Vector3[]
        {
            topLeftPostition,
            topRightPostition,
            bottomLeftPosition,
            bottomRightPosition
        };

        // Create uvs array used to store UV information
        Vector2[] uvs = new Vector2[vertices.Length];

        // Populate UV array
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        // Create Triangles array used for mesh
        int[] triangles = new int[] { 3, 1, 2, 2, 1, 0 };

        // Create new mesh
        Mesh mesh = new Mesh();

        // Setup mesh
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // Instanciate the ceiling using generated mesh and positions
        GameObject ceiling = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        ceiling.transform.position = Vector3.zero;
        ceiling.transform.localScale = Vector3.one;
        ceiling.GetComponent<MeshFilter>().mesh = mesh;
        ceiling.GetComponent<MeshRenderer>().material = ceilingMaterial;
        ceiling.transform.parent = transform;

        // Add a collider to stop the player jumping through the ceiling
        ceiling.AddComponent<MeshCollider>();

    } // End of CreateNewCeilingMesh function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateTorches                                                                                                                         //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room with torches. A torch is placed in each corner of a room. The same is done with the                 //
    // dungeon corridors                                                                                                                     //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateTorches(GameObject torchParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition     = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPosition    = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        torchParent.transform.parent = transform;

        // Calculate torch positions
        Vector3 torchBottomLeftPosition  = new Vector3(bottomLeftPosition.x  + 0.2f, 0.0f, bottomLeftPosition.z  + 0.2f);
        Vector3 torchBottomRightPosition = new Vector3(bottomRightPosition.x - 0.2f, 0.0f, bottomRightPosition.z + 0.2f);
        Vector3 torchTopLeftPosition     = new Vector3(topLeftPosition.x     + 0.2f, 0.0f, topLeftPosition.z     - 0.2f);
        Vector3 torchTopRightPosition    = new Vector3(topRightPosition.x    - 0.2f, 0.0f, topRightPosition.z    - 0.2f);

        // Create Torch Game objects in cornor of each room
        GameObject torchBottomLeft  = Instantiate(torch1, torchBottomLeftPosition,  Quaternion.identity, torchParent.transform);
        GameObject torchBottomRight = Instantiate(torch1, torchBottomRightPosition, Quaternion.identity, torchParent.transform);
        GameObject torchTopLeft     = Instantiate(torch1, torchTopLeftPosition,     Quaternion.identity, torchParent.transform);
        GameObject torchTopRight    = Instantiate(torch1, torchTopRightPosition,    Quaternion.identity, torchParent.transform);

    } // End of CreateTorches function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateBarrels                                                                                                                         //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room a set of barrels. A barrel is placed in a random position in the room, not too close to             //
    // the walls.                                                                                                                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateBarrels(GameObject barrelParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition     = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPosition    = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        barrelParent.transform.parent = transform;

        // Calculate dimensions of room
        float widthOfRoom  = Mathf.Abs(bottomLeftPosition.x - bottomRightPosition.x);
        float lengthOfRoom = Mathf.Abs(bottomLeftPosition.z - topLeftPosition.z);

        // Check if room is not small corridor where the object may potentionally block any enemy pathfinding
        if (widthOfRoom > 7f || lengthOfRoom > 7f)
        {
            // Calcualte new position of barrel
            Vector3 barrelPosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.0f,
                                                 UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));

            // Create barrel
            GameObject barrel = Instantiate(barrelSet1, barrelPosition, Quaternion.identity, barrelParent.transform);

            // Add barrel to unwalkable layer, used in pathfinding
            barrel.layer = 9;
 
        }

    } // End of CreateBarrels function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateCages                                                                                                                           //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room a prison cage. A cage is placed in a random position in the room, not too close to                  //
    // the walls.                                                                                                                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateCages(GameObject CageParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition     = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPosition    = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        CageParent.transform.parent = transform;

        // Calculate dimensions of room
        float widthOfRoom  = topRightPosition.x - bottomLeftPosition.x;
        float lengthOfRoom = topRightPosition.y - bottomLeftPosition.y;

        // Check if room is not small corridor where the object may potentionally block any enemy pathfinding
        if (widthOfRoom >= 5.0f || lengthOfRoom >= 5.0f)
        {
            // Calcualte new position of Cage
            Vector3 cagePosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.0f,
                                                 UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));

            // Create Cage
            GameObject cage = Instantiate(cage1, cagePosition, Quaternion.identity, CageParent.transform);

            // Add cage to unwalkable layer, used in pathfinding
            cage.layer = 9;
        }

    } // End of CreateCages funtion

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateCrates                                                                                                                          //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room a set of wodden crates. Crates are placed in a random position in the room, not too close to        //
    // the walls.                                                                                                                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateCrates(GameObject CrateParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y - 50.0f);
        Vector3 topRightPosition = new Vector3(topRightCorner.x - 50.0f, yPos, topRightCorner.y - 50.0f);
        Vector3 bottomLeftPosition = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        CrateParent.transform.parent = transform;

        // Calculate dimensions of room
        float widthOfRoom = topRightPosition.x - bottomLeftPosition.x;
        float lengthOfRoom = topRightPosition.y - bottomLeftPosition.y;

        // Check if room is not small corridor where the object may potentionally block any enemy pathfinding
        if (widthOfRoom > 6.0f || lengthOfRoom > 6.0f)
        {
            // Calcualte new position of Cage
            Vector3 cratePosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.0f,
                                                 UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));

            // Create Cage
            GameObject crate = Instantiate(crate1, cratePosition, Quaternion.identity, CrateParent.transform);

            // Add cage to unwalkable layer, used in pathfinding
            crate.layer = 9;
        }

    } // End of CreateCages funtion

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateSoliderEnemy                                                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room a solider enemy. The solider starts in an idle position and can be aggrod if approached too close.  //
    // Solider can attack using his sword in close combat and can be killed where he dies and despawns.                                      //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateSoliderEnemy(GameObject enemyParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition     = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPosition    = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        enemyParent.transform.parent = transform;
        
        // Calculate dimensions of room
        float widthOfRoom  = topRightPosition.x - bottomLeftPosition.x;
        float lengthOfRoom = topRightPosition.y - bottomLeftPosition.y;

        // Check if room is not small corridor, only want enemies spawning in the main rooms
        if (widthOfRoom >= 5.0f || lengthOfRoom >= 5.0f)
        {
            // Calcualte new random position of enemy
            Vector3 enemyPosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.0f,
                                                UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));
        
            // Instantiate the enemy
            GameObject enemyInstance = Instantiate(enemy1, enemyPosition, Quaternion.identity, enemyParent.transform);

            totalNumberOfEnemies++;

            // Set the target of the enemy as the player
            enemyInstance.GetComponent<Enemy>().target       = player.transform;
            // Set the player as the unit to be attacked
            enemyInstance.GetComponent<EnemyAttack>().target = player.transform;
        }

    } // End of CreateSoliderEnemy funtion

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateMageEnemy                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room a Mage enemy. Like the solider enemy, the solider starts in an idle position and can be aggrod if   //
    // approached too close. Mage can attack using his sword in close combat and can be killed where he dies and despawns. Possible future   //
    // work may include ranged combat that allows the Mage to attack with ranged spells.                                                     //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateMageEnemy(GameObject enemyParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition     = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 topRightPosition    = new Vector3(topRightCorner.x   - 50.0f, yPos, topRightCorner.y   - 50.0f);
        Vector3 bottomLeftPosition  = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x   - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        enemyParent.transform.parent = transform;

        // Calculate dimensions of room
        float widthOfRoom  = topRightPosition.x - bottomLeftPosition.x;
        float lengthOfRoom = topRightPosition.y - bottomLeftPosition.y;

        // Check if room is not small corridor, only want enemies spawning in the main rooms
        if (widthOfRoom >= 5.0f || lengthOfRoom >= 5.0f)
        {
            // Calcualte new random position of enemy
            Vector3 enemyPosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.0f,
                                                 UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));

            // Instantiate the enemy
            GameObject enemyInstance = Instantiate(enemy2, enemyPosition, Quaternion.identity, enemyParent.transform);

            totalNumberOfEnemies++;

            // Set the target of the enemy as the player
            enemyInstance.GetComponent<Enemy>().target = player.transform;
            // Set the player as the unit to be attacked
            enemyInstance.GetComponent<EnemyAttack>().target = player.transform;
        }

    } // End of CreateMageEnemy funtion

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateCollectableGems                                                                                                                 //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room with gems for the player to collect. Gems are placed in a random position in the room,              //
    // currently 2 per room but an option can be added for the user to choose.                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateCollectableGems(GameObject GemParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y - 50.0f);
        Vector3 topRightPosition = new Vector3(topRightCorner.x - 50.0f, yPos, topRightCorner.y - 50.0f);
        Vector3 bottomLeftPosition = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        GemParent.transform.parent = transform;

        // Calculate dimensions of room
        float widthOfRoom = topRightPosition.x - bottomLeftPosition.x;
        float lengthOfRoom = topRightPosition.y - bottomLeftPosition.y;

        // Check if room is not small corridor where the object may potentionally block any enemy pathfinding
        if (widthOfRoom > 6.0f || lengthOfRoom > 6.0f)
        {
            // Calcualte new position of gem
            Vector3 gemPosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.5f,
                                                 UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));

            // Create gem
            GameObject gem = Instantiate(gem1, gemPosition, Quaternion.Euler(90.0f, 0.0f, 0.0f), GemParent.transform);

            totalNumberOfGems++;

            // Add gem to unwalkable layer, used in pathfinding
            gem.layer = 9;
        }

    } // End of CreateCollectableGems funtion

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CreateCollectableAmmoBooks                                                                                                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates each room with books for the player to collect. Books are placed in a random position in the room,            //
    // currently 1 per room. Books increase the players staff ammo by a set amount that can be changed in the inspector.                     //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CreateCollectableAmmoBooks(GameObject ammoParent, Vector2 bottomLeftCorner, float yPos, Vector2 topRightCorner)
    {
        // Calculate the corners of the room
        Vector3 topLeftPosition = new Vector3(bottomLeftCorner.x - 50.0f, yPos, topRightCorner.y - 50.0f);
        Vector3 topRightPosition = new Vector3(topRightCorner.x - 50.0f, yPos, topRightCorner.y - 50.0f);
        Vector3 bottomLeftPosition = new Vector3(bottomLeftCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);
        Vector3 bottomRightPosition = new Vector3(topRightCorner.x - 50.0f, yPos, bottomLeftCorner.y - 50.0f);

        // Create new group for inspector
        ammoParent.transform.parent = transform;

        // Calculate dimensions of room
        float widthOfRoom = topRightPosition.x - bottomLeftPosition.x;
        float lengthOfRoom = topRightPosition.y - bottomLeftPosition.y;

        // Check if room is not small corridor where the object may potentionally block any enemy pathfinding
        if (widthOfRoom > 6.0f || lengthOfRoom > 6.0f)
        {
            // Calcualte new position of ammo
            Vector3 ammoPosition = new Vector3(UnityEngine.Random.Range(bottomLeftPosition.x + 1.0f, bottomRightPosition.x - 1.0f), 0.5f,
                                                 UnityEngine.Random.Range(bottomLeftPosition.z + 1.0f, topLeftPosition.z - 1.0f));

            // Create ammo
            GameObject ammo = Instantiate(ammoBook1, ammoPosition, Quaternion.Euler(90.0f, 0.0f, 0.0f), ammoParent.transform);

            // Add ammo to unwalkable layer, used in pathfinding
            ammo.layer = 9;
        }

    } // End of CreateCollectableAmmoBooks funtion


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // AddPositionOfWallToList                                                                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that populates the walls and doors list with the positions of the walls or doors                                             //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void AddPositionOfWallToList(Vector3 positionOfWall, List<Vector3Int> listOfWalls, List<Vector3Int> listOfDoors)
    {
        // Calulate position
        Vector3Int position = Vector3Int.CeilToInt(positionOfWall);

        // If it already contains the position, is is a door so add it to the doors list
        if (listOfWalls.Contains(position))
        {
            listOfDoors.Add(position);
            listOfWalls.Remove(position);
        }
        else // else add it to the walls list
        {
            listOfWalls.Add(position);
        }

    } // End of AddPositionOfWallToList function

} // End of DungeonMaker class
