//*******************************************************************************************************************************************//
//  File:   PathfindingRequestManager.cs                                                                                                     //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   A* path request manager class                                                                                                    //
//                                                                                                                                           //
//  Notes:  Struct that takes all the path finding requests and spreads them out over multiple frames. This speeds up the process            //
//          when the program is requesting multiple paths at once                                                                            //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Runtime.ExceptionServices;

public class PathfindingRequestManager : MonoBehaviour
{
	//***************************************************************************************************************************************//
	//	Private Variables                                                                                                                    //
	//***************************************************************************************************************************************//

	Queue<ResultOfPath> pathResults = new Queue<ResultOfPath>();  // path request queue
	static PathfindingRequestManager requestManagerInstance;  // Static to access information from the static method
	PathfindingAStar pathfindingAStar;                        // Reference to the pathfinding class

	//***************************************************************************************************************************************//
	//	Awake Function - Called once when program is first started                                                                           //
	//***************************************************************************************************************************************//

	void Awake()
	{
		requestManagerInstance = this;                         // Set to this instance
		pathfindingAStar = GetComponent<PathfindingAStar>();   // Create pathfinding reference
	}

	//***************************************************************************************************************************************//
	//	Update Function - Called once per frame                                                                                              //
	//***************************************************************************************************************************************//

	void Update()
    {
        if (pathResults.Count > 0)
        {
			int numOfItemsInQueue = pathResults.Count;
			
			lock(pathResults)
            {
				for (int i = 0; i < numOfItemsInQueue; i++)
                {
					ResultOfPath pathResult = pathResults.Dequeue();
					pathResult.callback(pathResult.validPath, pathResult.pathSucess);
                }
            }
        }

    } // End of Update Function

	//***************************************************************************************************************************************//
	//	Class functions                                                                                                                      //
	//***************************************************************************************************************************************//

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// RequestValidPath                                                                                                                      //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function to request a path for a enemy. This is called from the enemy class. The request path class                                                           //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public static void RequestValidPath(RequestPath pathRequest)
	{
		// Start a new thread
		// The pathfinding code now runs on a seperate thread
		ThreadStart startThread = delegate
		{
			requestManagerInstance.pathfindingAStar.FindValidPath(pathRequest, requestManagerInstance.CompleteProcessingValidPath);
		};

		startThread.Invoke();

	} // End of RequestValidPath function

	//---------------------------------------------------------------------------------------------------------------------------------------//
	// CompleteProcessingValidPath                                                                                                           //
	// --------------------------------------------------------------------------------------------------------------------------------------//
	// Function that is called by the pathfinding script when it has finished finding the path for the enemy                                 //
	//                                                                                                                                       //
	// --------------------------------------------------------------------------------------------------------------------------------------//

	public void CompleteProcessingValidPath(ResultOfPath pathResult)
	{
		lock(pathResults)
        {
			pathResults.Enqueue(pathResult);

		}

	} // End of CompleteProcessingValidPath function

} // End of PathfindingRequestManager clas

//*******************************************************************************************************************************************//
//	Structures                                                                                                                               //
//*******************************************************************************************************************************************//

public struct ResultOfPath
{
	//***************************************************************************************************************************************//
	//	Struct public Variables                                                                                                              //
	//***************************************************************************************************************************************//

	public Vector3[] validPath;
	public bool pathSucess;
	public Action<Vector3[], bool> callback;

	//***************************************************************************************************************************************//
	//	Struct Constructor                                                                                                                   //
	//***************************************************************************************************************************************//

	public ResultOfPath(Vector3[] _validPath, bool _pathSuccess, Action<Vector3[], bool> _callback)
	{
		this.validPath  = _validPath;
		this.pathSucess = _pathSuccess;
		this.callback  = _callback;
	}

} // End of ResultOfPath Struct



public struct RequestPath
{
	//***************************************************************************************************************************************//
	//	Struct public Variables                                                                                                              //
	//***************************************************************************************************************************************//

	public Vector3           startPosOfPath;  // Position of the start of the path
	public Vector3           endPosOfPath;    // Position of the end of the path
	public Action<Vector3[], bool> callback;  // callback

	//***************************************************************************************************************************************//
	//	Struct Constructor                                                                                                                   //
	//***************************************************************************************************************************************//

	// Set variables
	public RequestPath(Vector3 _startPosOfPath, Vector3 _endPosOfPath, Action<Vector3[], bool> _callback)
	{
		startPosOfPath = _startPosOfPath;
		endPosOfPath   = _endPosOfPath;
		callback       = _callback;
	}

} // End of RequestPath struct