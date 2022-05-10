using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//*******************************************************************************************************************************************//
//  File:   BSPUISlderTextBoxes.cs                                                                                                           //
//                                                                                                                                           //
//  Author: Mark Ince                                                                                                                        //
//                                                                                                                                           //
//  Date:   03/11/2020                                                                                                                       //
//                                                                                                                                           //
//  Info:   Slider text boxes class                                                                                                          //
//                                                                                                                                           //
//  Notes:  Updates the text boxes when the sliders are used to modify the dungeon generation settings                                       //
//                                                                                                                                           //
//*******************************************************************************************************************************************//


public class BSPUISlderTextBoxes : MonoBehaviour
{
    //***************************************************************************************************************************************//
    //	Public Variables                                                                                                                     //
    //***************************************************************************************************************************************//

    public Text dungeonWidthText;
    public Slider dungeonWidthTextSlider;

    public Text dungeonLengthText;
    public Slider dungeonLengthTextSlider;

    public Text minWidthOfRoomsText;
    public Slider minWidthOfRoomsSlider;

    public Text minLengthOfRoomsText;
    public Slider minLengthOfRoomsSlider;

    public Text maxNumberOfRoomsText;
    public Slider maxNumberOfRoomsSlider;

    public Text widthOfCorridorText;
    public Slider widthOfCorridorSlider;

    public Text bottomCornerRoomOffsetText;
    public Slider bottomCornerRoomOffsetSlider;

    public Text topCornerRoomOffsetText;
    public Slider topCornerRoomOffsetSlider;

    public Text roomOffsetText;
    public Slider roomOffsetSlider;

    //***************************************************************************************************************************************//
    //	Update Function                                                                                                                      //
    //***************************************************************************************************************************************//

    void Update()
    {

        dungeonWidthText.text           = dungeonWidthTextSlider.value.ToString();
        dungeonLengthText.text          = dungeonLengthTextSlider.value.ToString();
        minWidthOfRoomsText.text        = minWidthOfRoomsSlider.value.ToString();
        minLengthOfRoomsText.text       = minLengthOfRoomsSlider.value.ToString();
        maxNumberOfRoomsText.text       = maxNumberOfRoomsSlider.value.ToString();
        widthOfCorridorText.text        = widthOfCorridorSlider.value.ToString();
        bottomCornerRoomOffsetText.text = bottomCornerRoomOffsetSlider.value.ToString("0.00");
        topCornerRoomOffsetText.text    = topCornerRoomOffsetSlider.value.ToString("0.00");
        roomOffsetText.text             = roomOffsetSlider.value.ToString();
    }

}
