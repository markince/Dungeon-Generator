using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   CameraHeadBob.cs                                                                                                                 //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:                                                                                                                                    //
//                                                                                                                                           //
//  Notes:                                                                                                                                   //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class PlayerCameraHeadBob : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private KeyCode playerRunKey = KeyCode.LeftShift; // Key player can hold down to make the player move faster (run)
    private float timer = 0.0f;
    float bobbingSpeed = 0.18f;
    float midpoint = 2.0f;

    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    [SerializeField] float bobbingAmount = 0.2f;

    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cSharpConversion = transform.localPosition;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.y = midpoint + translateChange;
        }
        else
        {
            cSharpConversion.y = midpoint;
        }

        transform.localPosition = cSharpConversion;

        if (Input.GetKey(playerRunKey)) // Has the run key been pressed?
        {
            bobbingSpeed = 0.025f;

        }
        else
        {
            bobbingSpeed = 0.015f;

        }

    }

}

