using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************************************************************//
//  File:   CAUISliderTextBoxes.cs                                                                                                           //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:                                                                                                                                    //
//                                                                                                                                           //
//  Notes:  Updates the text boxes when the sliders are used to modify the dungeon generation settings                                       //
//                                                                                                                                           //
//*******************************************************************************************************************************************//

public class CAUISliderTextBoxes : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//


    public Text   dungeonWidthText;
    public Slider dungeonWidthTextSlider;
    public Text   dungeonLengthText;
    public Slider dungeonLengthTextSlider;
    public Text   wallFillText;
    public Slider wallFillTextSlider;

    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    private void Update()
    {
        dungeonWidthText.text  = dungeonWidthTextSlider.value.ToString();
        dungeonLengthText.text = dungeonLengthTextSlider.value.ToString();
        wallFillText.text      = wallFillTextSlider.value.ToString();
    }


}
