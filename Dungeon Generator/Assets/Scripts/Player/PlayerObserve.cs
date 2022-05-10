using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   PlayerObserve.cs                                                                                                                 //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Player Observe class                                                                                                             //
//                                                                                                                                           //
//  Notes:  Functionality to allow the player to look around in first person mode                                                            //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class PlayerObserve : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Constants                                                                                                                            //
    //***************************************************************************************************************************************//

    private const float Y_AXIS_ROTATION_LIMIT = 90.0f;
    private const float LOOK_UP_CLAMP_VALUE = 270.0f;
    private const float LOOK_DOWN_CLAMP_VALUE = 90.0f;


    //***************************************************************************************************************************************//
    //	Serialized Private Variables                                                                                                         //
    //***************************************************************************************************************************************//

    [SerializeField] private Transform playerTransform = null; // Transform of the player
    [SerializeField] private string mouseHorizontalInputName = null; // Name of X mouse control defined in the Input manager
    [SerializeField] private string mouseVerticalInputName = null; // Name of Y mouse control defined in the Input manager
    [SerializeField] private float mouseResponsiveness = 0.0f; // Sensitivity of the mouse movement

    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private float xAxisLimit = 0.0f; // Clamp value that will stop the player being able to look 360 degrees in a vertical motion

    //***************************************************************************************************************************************//
    //	Awake Function - Called once before any start functions                                                                              //
    //***************************************************************************************************************************************//

    private void Awake()
    {
        xAxisLimit = 0.0f; // Initalise clamp value
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor at start

    } // End of Awake function

    //***************************************************************************************************************************************//
    //	Update Function - Called once per frame                                                                                              //
    //***************************************************************************************************************************************//

    private void Update()
    {
        RotateCameraOnPlayerMouseInput();

    } // End of update function

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // RotateCameraOnPlayerMouseInput                                                                                                        //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to rotate the camera attached to the player body                                                                             //
    // depending on the inputs of the mouse from the user                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void RotateCameraOnPlayerMouseInput()
    {
        // Get the mouse movements and store them in float variables
        // Responsive time can be modified in the inspector to alter the sensativity of the mouse
        float mouseMovementX = Input.GetAxis(mouseHorizontalInputName) * mouseResponsiveness * Time.deltaTime;
        float mouseMovementY = Input.GetAxis(mouseVerticalInputName) * mouseResponsiveness * Time.deltaTime;

        // Get the Y mouse movement value and store it in a variable
        xAxisLimit += mouseMovementY;

        // Check to see if the user is looking up or down and limit thier look movement to stop them
        // rotating 360 degrees upside down

        if (xAxisLimit > Y_AXIS_ROTATION_LIMIT) // Looking directly up?
        {
            xAxisLimit = Y_AXIS_ROTATION_LIMIT;
            mouseMovementY = 0.0f;
            LimitXAxisRotation(LOOK_UP_CLAMP_VALUE);
        }
        else if (xAxisLimit < -Y_AXIS_ROTATION_LIMIT) // Looking directly down?
        {
            xAxisLimit = -Y_AXIS_ROTATION_LIMIT;
            mouseMovementY = 0.0f;
            LimitXAxisRotation(LOOK_DOWN_CLAMP_VALUE);

        } // End of if statment

        // Rotate the camera attached to the player up and down depending on the mouses up and down movement
        // The player itself does not need to rotate when looking up and down, only the camera

        transform.Rotate(Vector3.left * mouseMovementY);

        // Rotate the player body left and right depending on the mouses left and right movement
        // The whole players body rotates and the camera will rotate as well becuase it is attached to the player

        playerTransform.Rotate(Vector3.up * mouseMovementX);

    } // End of RotateCameraOnPlayerMouseInput function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // LimitXAxisRotation                                                                                                                    //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to limit the X axis of the mouse to a specific value                                                                         //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void LimitXAxisRotation(float rotationValue)
    {
        // Get the euler rotation of the camera and store it in a variable
        Vector3 eulerRoationOfCamera = transform.eulerAngles;

        // Set the x axis rotation to the value passed into the function
        eulerRoationOfCamera.x = rotationValue;

        // Set the rotation of the transform back to the euler rotation
        transform.eulerAngles = eulerRoationOfCamera;

    } // End of LimitXAxisRotation function

} // End of PlayerObserve class

