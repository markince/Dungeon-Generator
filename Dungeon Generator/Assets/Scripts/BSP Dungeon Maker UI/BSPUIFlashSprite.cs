using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*******************************************************************************************************************************************//
//  File:   BSPUIFlashSprite.cs                                                                                                              //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Flash Sprite Class                                                                                                               //
//                                                                                                                                           //
//  Notes:  Function that flashes a sprite on the screen for a few seconds                                                                   //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class BSPUIFlashSprite : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Private Variables                                                                                                                    //
    //***************************************************************************************************************************************//

    private Material _mat;
    private Color[] _colors = { Color.yellow, Color.red };
    private float _flashSpeed = 0.1f;
    private float _lengthOfTimeToFlash = 1f;

    //***************************************************************************************************************************************//
    //	Awake Function                                                                                                                       //
    //***************************************************************************************************************************************//

    public void Awake()
    {
        this._mat = GetComponent<SpriteRenderer>().material;
    }

    //***************************************************************************************************************************************//
    //	Start Function                                                                                                                       //
    //***************************************************************************************************************************************//

    void Start()
    {
        StartCoroutine(Flash(this._lengthOfTimeToFlash, this._flashSpeed));
    }

    //***************************************************************************************************************************************//
    //	IEnumerators                                                                                                                         //
    //***************************************************************************************************************************************//

    //---------------------------------------------------------------------------------------------------------------------------------------//
    // Flash                                                                                                                                 //
    // --------------------------------------------------------------------------------------------------------------------------------------//
    // Function that flashes a sprite on the screen for a few seconds                                                                        //
    //                                                                                                                                       //
    // --------------------------------------------------------------------------------------------------------------------------------------//

    IEnumerator Flash(float time, float intervalTime)
    {
        float elapsedTime = 0f;
        int index = 0;
        while (elapsedTime < time)
        {
            _mat.color = _colors[index % 2];

            elapsedTime += Time.deltaTime;
            index++;
            yield return new WaitForSeconds(intervalTime);
        }
    }

}
