using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//*******************************************************************************************************************************************//
//  File:   Enemy.cs                                                                                                                         //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Enemy Class                                                                                                                      //
//                                                                                                                                           //
//  Notes:  Main enemy class. Deals with pathfinding, and states                                                                             //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class Enemy : MonoBehaviour
{
	//***************************************************************************************************************************************//
	//	Constants                                                                                                                            //
	//***************************************************************************************************************************************//

	const float minPathUpdateTime = .2f;         // Minimum amount of time that has to pass between each path request
	const float pathUpdateMoveThreshold = .5f;

	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	enum enemyState { idle, following, Attacking, Dead }; // Current state of the enemy

	float     distanceToPlayer;  // Total distance bwtween the player and the enemy
	float     chaseRange = 6.0f; // Agro range with the player
	bool      agroSoundplayed;   // Has the agro sound been played?
	bool      deathSoundplayed;  // has the death sound been played?
	Rigidbody rb;
	PathfindingPath path; // Final path the enemy follows when chasing the player

	enemyState  currentState; // Current state of the enemy
	EnemyHealth health;       // Enemies health
	Collider    collider1;    // enmey Collider

	bool onetime = false;

	//***************************************************************************************************************************************//
	//	Public Variables                                                                                                                     //
	//***************************************************************************************************************************************//

	public Transform target;                // Target position of the player
	public float speed = 20;                // Enemy speed
	public float turnSpeed = 3;
	public float turnDst = 5;               // Distance from the waypoint in the pathfinding path in which the enemy starts to turn
	public float stoppingDst = 10;          // How far from the player the enemy starts to slow down
	public float rotationTurnSpeed = 5.0f;

	public GameObject player;               // Reference to the player

	public AudioSource audioSource;        // Audio clips for the enemy
	public AudioClip death1;
	public AudioClip death2;
	public AudioClip death3;

	public AudioClip agro1;
	public AudioClip agro2;
	public AudioClip agro3;

	//***************************************************************************************************************************************//
	//	Start Function - Called once at the start                                                                                            //
	//***************************************************************************************************************************************//

	private void Start()
	{
		currentState = enemyState.idle; // Set starting state


		rb        = GetComponent<Rigidbody>();     // Get components
		health    = GetComponent<EnemyHealth>();
		collider1 = GetComponent<Collider>();


		agroSoundplayed  = false;    // Set starting sounds
		deathSoundplayed = false;

	}

	//***************************************************************************************************************************************//
	//	Update Function - Called once per frame                                                                                              //
	//***************************************************************************************************************************************//


	private void Update()
	{
		// Check for death
		if (health.IsDead())
		{
			currentState = enemyState.Dead;
		}

		// Calculate the distance between the player and the enemy
		distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

		// Switch statment to deal with the current enemy state
		switch (currentState)
		{
			// IDLE
			case enemyState.idle:
			{
				if (distanceToPlayer < chaseRange) // Is the player within range of agro?
				{
						if (!onetime) // Only start the coroutine once in the update loop
						{
							StartCoroutine(UpdatePath());
							onetime = true;
						}

					    currentState = enemyState.following; // set state to following 
					}

					break;
				}
			// FOLLOWING
			case enemyState.following:
			{
				if (!agroSoundplayed)
				{
					// Play the agro sound
					int ranNum = UnityEngine.Random.Range(1, 3);

					// Can be one of 3 random sounds
					if (ranNum == 1) audioSource.PlayOneShot(agro1);
					else if (ranNum == 2) audioSource.PlayOneShot(agro2);
					else if (ranNum == 3) audioSource.PlayOneShot(agro3);

					agroSoundplayed = true;
				}

				// Activate animations
				GetComponent<Animator>().SetBool("Attack", false);
				GetComponent<Animator>().SetTrigger("Move");

				// Check if enemy is within attack distance
				if (distanceToPlayer <= 2.0f)
				{
					// Set state to attack
					currentState = enemyState.Attacking;
				}

				break;

			}
			// ATTACKING
			case enemyState.Attacking:
			{
				// Set attack animation
				GetComponent<Animator>().SetBool("Attack", true);

				// Check if player has moved away from the enemy
				if (distanceToPlayer > 2.0f)
				{
					// Change state to following
					currentState = enemyState.following;
				}


				break;
			}
			// DEAD
			case enemyState.Dead:
			{
				// Play death sound
				if (!deathSoundplayed)
				{
					audioSource.PlayOneShot(death2);
					deathSoundplayed = true;
				}

				// Stop the pathfinidng
				if (onetime)
				{
					StopCoroutine(UpdatePath());
					onetime = false;
				}

				// Deactivate enemies collider
				collider1.enabled = false;

				// Delete the enemy from the world
				StartCoroutine(DeleteEnemy());

				break;
			}

		}

	}

	//***************************************************************************************************************************************//
	//	IEnumerators                                                                                                                         //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// DeleteEnemy                                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to delete the enemy from the world when dead                                                                                 //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	IEnumerator DeleteEnemy()
    {
		yield return new WaitForSeconds(4.0f);

		Destroy(gameObject);
    }

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// UpdatePath                                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to update the path of the enemy once the target position has actually started moving.                                        //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	IEnumerator UpdatePath()
	{
		
		if (Time.timeSinceLevelLoad < .3f)
		{
			yield return new WaitForSeconds(.3f);
		}

		// Request a new path wether or not the target has moved or not to start
		PathfindingRequestManager.RequestValidPath(new RequestPath(transform.position, target.position, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target.position;

		while (true)
		{
			// Add a min amount of time that has to pass between each path request
			yield return new WaitForSeconds(minPathUpdateTime);

			// Has the target moved?
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
			{
				// Request new path
				PathfindingRequestManager.RequestValidPath(new RequestPath(transform.position, target.position, OnPathFound));
				targetPosOld = target.position;
			}

		}

	} // End of UpdatePath IEnumerator

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// FollowPath                                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Coroutine that is started once a path is found between the player and an enemy.                                                       //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	IEnumerator FollowPath()
	{
		
		bool  followingPath  = true;   // Is the enemy following the path?
		int   pathIndex      = 0;      // Set the index start of the path
		float speedPercent   = 1;      // Percentages of the speed of the enemy

		// Start with the enemy facing the first lookpoint in the path
		transform.LookAt(path.lookPositions[0]);

		// While loop that runs while we are following the path
		while (followingPath) 
		{
			// Get 2D position of the enemy
			Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

			// Check if the enemy has passed the next turn boundary
			// This uses a while loop to do this check more than once per frame
			while (path.turnBoundaries[pathIndex].HasCrossedTheLine(pos2D))
			{
				// Has the enemy reached the last line?
				if (pathIndex == path.indexOfFinalFinishLine)
				{
					followingPath = false;
					break;
				}
				else
				{
					// Increment the path index
					pathIndex++;
				}
			}

			if (followingPath) // Is the enemy still following the path?
			{
				// Check if we need to start slowing down
				if (pathIndex >= path.slowMovementIndex && stoppingDst > 0)
				{
					// Slow down the enemy when it gets to the final line
					speedPercent = Mathf.Clamp01(path.turnBoundaries[path.indexOfFinalFinishLine].CalculateDistanceFromPointToLine(pos2D) / stoppingDst);
					
					// if moving to slow, stop the enemy
					if (speedPercent < 0.01f)
					{
						followingPath = false;
					}
				}

				// Rotate the unit a small amount towards the lookpoint
				Quaternion targetRotation = Quaternion.LookRotation(path.lookPositions[pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

				// Move the enemy a small amount along the path
				transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
			}

			yield return null; // move to next frame

		}
	}

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// OnPathFound                                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that is actioned when a valid path is found between the enemy and the player                                                 //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
	{

		if (!health.IsDead()) // Is the enemy alive?
		{


			if (pathSuccessful) // Has a path been found?
			{
				// Create new path
				path = new PathfindingPath(waypoints, transform.position, turnDst, stoppingDst);
				// Stop Coroutine if it has already been started
				StopCoroutine("FollowPath");
				// Start Coroutine
				StartCoroutine("FollowPath");
			}

		}

	} // End of OnPathFound function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// FaceTarget                                                                                                                            //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to face the enemy (NOT USED)                                                                                                                                      //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	private void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationTurnSpeed);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// OnDamageTaken                                                                                                                         //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to death with damage from the player (NOT USED)                                                                              //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	private void OnDamageTaken()
	{
		StartCoroutine(UpdatePath());
		currentState = enemyState.following;

	}

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// OnDrawGizmos                                                                                                                          //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to draw a line between the enemy and the player in the Scene view. Also draws a sphere around the enemies to show chase      //
	// range                                                                                                                                 //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void OnDrawGizmos()
	{
		if (path != null)
		{
			path.DrawWithGizmos();  
		}

		// Draw a sphere to show the chase range of the enemies
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, chaseRange); 
	}

} // End of enemy class