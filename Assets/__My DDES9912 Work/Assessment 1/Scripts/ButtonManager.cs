using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    // Script Variables
    float localPositionX;
    float localPositionY;
    float localPositionZ;
    bool buttonPressed = false;
    float moveDistance = 0.04f;

    // Public Variables
    public int buttonValue = 0;
    public bool isActionNumber = false;
    public bool isActionTotal = false;
    public bool isActionSubtotal = false;
    public bool isActionNonadd = false;
    public bool isActionQtr = false;
    public bool isActionHalf = false;
    public bool isAction3Qtr = false;
    public bool isActionRepeat = false;
    public string action = "Undefined";
    public int finalValue = 0;
    public string finalAction = "Inactive";
    public bool finalPressedValue = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start Button Manager");

        // Get the local X position of the button
        localPositionX = transform.localPosition.x;
        // Get the local X position of the button
        localPositionY = transform.localPosition.y;
        // Get the local X position of the button
        localPositionZ = transform.localPosition.z;
        // Print the value to the console
        //Debug.Log("Local Position: X: " + localPositionX + " Y: " + localPositionY + " Z: " + localPositionZ);
        //Get the button action value based on the value specified in the object's inspector window
        action = setButtonActionValue();
        //Debug.Log("pressed action is: " + action);
    }

    private string setButtonActionValue()
    {
        if (isActionTotal) { action = "Total"; }
        else if (isActionSubtotal) { action = "SubTotal"; }
        else if (isActionNonadd) { action = "NonAdd"; }
        else if (isActionQtr) { action = "1Qtr"; }
        else if (isActionHalf) { action = "1Half"; }
        else if (isAction3Qtr) { action = "3Qtr"; }
        else if (isActionRepeat) { action = "Repeat"; }
        else { action = "Number"; }

        return action;
    }

    public void PressButton()
    {
        Debug.Log("Pressing button either up or down");

        if (buttonPressed == false) {
            Debug.Log("Pressing button down");

            // Move button down on the y-axis to appear to be pressed down
            transform.localPosition = new Vector3(localPositionX, localPositionY - moveDistance, localPositionZ);
            // Change the state of the button to pressed
            buttonPressed = true;
            // Set the numerical value of the button to be the value specified in the object's inspector window
            finalValue = buttonValue;
            // Set the action value of the button to be the value specified in the object's inspector window
            finalAction = action;
            // Set the final Button Pressed value to be the value specified in the object's inspector window
            finalPressedValue = buttonPressed;

        } else
        {
            Debug.Log("Resetting button to original position");

            // Move button back up to original not-pressed position
            transform.localPosition = new Vector3(localPositionX, localPositionY, localPositionZ);
            // Change state of button back to not-pressed
            buttonPressed = false;
            // Set the numerical value of the button to be the value to zero
            finalValue = 0;
            // Set the action value of the button to be not active
            finalAction = "Inactive";
            // Set the final Button Pressed value to be the value specified in the object's inspector window
            finalPressedValue = buttonPressed;
        }
        
    }

}

