using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   PlayerMovement.cs                                                                                                                //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Player Movement class                                                                                                            //
//                                                                                                                                           //
//  Notes:  Functionality to allow the player to move around in the world in first person mode                                               //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class PlayerMovement : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Constants                                                                                                                            //
    //***************************************************************************************************************************************//

    private const float CLAMP_MAG_MAX_LENGTH = 1.0f;
    private const int CHAR_CONTROLLER_HEIGHT_MODIFIER = 2;
    private const float CHAR_CONTROLLER_SLOPE_LIMIT_MAX = 90.0f;
    private const float CHAR_CONTROLLER_SLOPE_LIMIT_MIN = 45.0f;

    //***************************************************************************************************************************************//
    //	Serialized Private Variables                                                                                                         //
    //***************************************************************************************************************************************//

    [SerializeField] private string mouseHorizontalInputName = null; // Name of X mouse control defined in the Input manager
    [SerializeField] private string mouseVerticalInputName = null; // Name of Y mouse control defined in the Input manager
    [SerializeField] private float playerWalkSpeed = 0.0f; // Walk speed of player
    [SerializeField] private float playerRunSpeed = 0.0f; // Run speed of player
    [SerializeField] private float buildUpRunSpeed = 0.0f; // Build up speed amount when player runs
    [SerializeField] private float forceAppliedOnSlope = 0.0f; // Multiplier for the downward force applied to the player when on a slope
    [SerializeField] private float lengthOfSlopeForceRay = 0.0f; // Multiplier for the length of the ray that is shot down from the player
                                                                 // to check if they are on a slope
    [SerializeField] private float jumpCurveMultiplier = 0.0f; // Multiplier for the animation curve

    [SerializeField] private AnimationCurve jumpDescendCurveValue = null;  // Animation curve that is used to specifiy how we jump

    [SerializeField] private KeyCode playerRunKey = KeyCode.LeftShift; // Key player can hold down to make the player move faster (run)
    [SerializeField] private KeyCode playerJumpKey = KeyCode.Space;     // Key player can hold down to make the player jump in the air

    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private CharacterController characterController; // Reference to the character controller
    private float movementSpeed;                     // Overall player move speed;
    private bool isPlayerJumping;                    // Is the player jumping in the air?

    //***************************************************************************************************************************************//
    //	Awake Function - Called once before any start functions                                                                              //
    //***************************************************************************************************************************************//

    private void Awake()
    {
        // Set a reference to the character controller
        characterController = GetComponent<CharacterController>();

    } // End of awake funtion

    //***************************************************************************************************************************************//
    //	Update Function - Called once per frame                                                                                              //
    //***************************************************************************************************************************************//

    private void Update()
    {
        MovePlayer();

    } // End of update function

    //***************************************************************************************************************************************//
    //	Class functions                                                                                                                      //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // MovePlayer                                                                                                                            //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to move the player around the world depending on the inputs                                                                  //
    // of the keyboard keys pressed by the user                                                                                              //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    private void MovePlayer()
    {
        // Get the mouse movements and store them in float variables
        float horizontalMouseInput = Input.GetAxis(mouseHorizontalInputName);
        float verticalMouseInput = Input.GetAxis(mouseVerticalInputName);

        // Get the forward vector of the player and multiply it by the vertical mouse 
        // input and store it in a variable
        Vector3 forwardMovement = transform.forward * verticalMouseInput;

        // Get the right vector of the player and multiply it by the horizontal mouse 
        // input and store it in a variable
        Vector3 rightMovement = transform.right * horizontalMouseInput;

        // Move the character controller using the above values
        // no need to use Time.Deltatime as simple move automatically applies that for us
        // Clamp magnitune stops the player moving too quickly when moving diagnoly
        // Clamp magnitude is similar to mathf.Clamp but is only used for a Vector3, can only specify a max value
        characterController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, CLAMP_MAG_MAX_LENGTH) * movementSpeed);

        // Check if player is on a slope using OnSlope function
        // is the player moving in any direction and on a slope?
        if ((verticalMouseInput != 0 || horizontalMouseInput != 0) && OnSlope())
        {
            // Apply the extra downward force. This stops the player from jittering when moving down a slope surface
            characterController.Move(Vector3.down * characterController.height / CHAR_CONTROLLER_HEIGHT_MODIFIER *
                                     forceAppliedOnSlope * Time.deltaTime);

        } // End of if statment

        // Set the approprate speed depending if the player is walking or running
        SetMovementSpeed();

        // Check for jumping
        CheckIfJumpKeyPressed();

    } // End of MovePlayer function


    //---------------------------------------------------------------------------------------------------------------------------------------//
    // SetMovementSpeed                                                                                                                      //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to set the appropriate movement of the player (running or walking)                                                           //
    // Game uses a key modifier to allow the player to run (shift)                                                                           //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void SetMovementSpeed()
    {
        if (Input.GetKey(playerRunKey)) // Has the run key been pressed?
        {
            // Lerp the movement speed towards our run speed
            movementSpeed = Mathf.Lerp(movementSpeed, playerRunSpeed, Time.deltaTime * buildUpRunSpeed);
        }
        else
        {
            // If the run key has been released, lerp towards the walk speed
            movementSpeed = Mathf.Lerp(movementSpeed, playerWalkSpeed, Time.deltaTime * buildUpRunSpeed);

        } // End of if statment

    } // End of SetMovementSpeed function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // OnSlope                                                                                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function to test if the character controller is on a slope                                                                            //
    // This is used to prevent the player from jittering when moving down a sloped surface                                                   //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private bool OnSlope()
    {
        if (isPlayerJumping) // We know that the player is not on a slope if they are jumping
        {
            return false;
        }
        else // Player on a slope?
        {
            RaycastHit groundHit; // Stores the information on the surface that has been hit by the ray

            // Check the ray, send in the origion of the ray, direction the ray is shot in, store the hit information in groundHit
            // and the length of the ray (character controller halfed * the length of the slope ray
            if (Physics.Raycast(transform.position, Vector3.down, out groundHit, characterController.height
                                / CHAR_CONTROLLER_HEIGHT_MODIFIER * lengthOfSlopeForceRay))
            {
                // Check the normal of the surface that has been hit. if it is not equal to
                // Vector3.up then we know we are on a slope surface
                if (groundHit.normal != Vector3.up)
                {
                    return true; // Player is on a slope surface

                } // End of if statment

            } // End of if statment

        } // End of if statment

        return false; // Player is not on a slope surface

    } // End of OnSlope function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // CheckIfJumpKeyPressed                                                                                                                               //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that checks if the user has pressed the jump key and makes the player jump in the air                                        //
    // Uses an IEnumerator co-Routine so that it can be dependant of the frametime                                                           //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private void CheckIfJumpKeyPressed()
    {
        // Check if user has pressed the jump key and is already not jumping
        if (Input.GetKeyDown(playerJumpKey) && !isPlayerJumping)
        {
            // player is jumping
            isPlayerJumping = true;

            // Start jump co-routine
            StartCoroutine(ActionJump());

        }  // End of if statment

    } // End of CheckIfJumpKeyPressed function

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // ActionJump (IEnumerator)                                                                                                              //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // IEnumerator used in CheckIfJumpKeyPressed function to make the player jump in the air                                                 //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    private IEnumerator ActionJump()
    {
        // Initalise a variable that keeps track of the time the player is in the air
        float timePlayerIsinAir = 0.0f;

        // Set the character controllers slope limit to a value. This stops the player from jittering 
        // in the air when they jump while against an object such as a wall
        characterController.slopeLimit = CHAR_CONTROLLER_SLOPE_LIMIT_MAX;

        do
        {
            // Initalise a variable that calculates the jump force
            // This is calcualted using the animation curve and is evaluated at a certain point on the curve
            // Player time in the air is passed into the aniamtion curve
            float playerJumpForce = jumpDescendCurveValue.Evaluate(timePlayerIsinAir);

            // Call the player controler move function to move the player using tghe above calculated values
            // Multiplyed by time.deltatime as this is not done in the above function
            characterController.Move(Vector3.up * playerJumpForce * jumpCurveMultiplier * Time.deltaTime);

            // Increment the time in air counter
            timePlayerIsinAir += Time.deltaTime;

            yield return null;

            // While the character is not on the ground
            // Collisionsflags checks to see if the user has hit a celing when jumping and stops the player
            // from jumping through it
        } while (!characterController.isGrounded && characterController.collisionFlags != CollisionFlags.Above); // End of Do-While loop


        characterController.slopeLimit = CHAR_CONTROLLER_SLOPE_LIMIT_MIN;

        // Reset jumping to false
        isPlayerJumping = false;

    } // End of ActionJump IEnumerator

} // End of PlayerMovement class

