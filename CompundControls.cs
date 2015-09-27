using UnityEngine;
using System.Collections;

public class CompundControls : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static float LabelSlider(Rect screenRect, float sliderValue, float sliderMaxValue, string labelText)
    {
        GUI.Label(screenRect, labelText);

        screenRect.x += screenRect.width;

        sliderValue = GUI.HorizontalSlider(screenRect, sliderValue, 0.0f, sliderMaxValue);
        return sliderValue; 
    }
    public static float VerticleLabelSlider(Rect screenRect, float sliderValue, float sliderMaxValue, string labelText)
    {
        GUI.Label(screenRect, labelText);

        screenRect.x += screenRect.width;

        sliderValue = GUI.VerticalSlider(screenRect, sliderValue, 0.0f, sliderMaxValue);
        return sliderValue;
    }
}
