using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using TMPro;

public class HandleManager : MonoBehaviour
{
    private string action;

    //public string targetActionVariableName = "finalAction"; // Name of the Action variable to check (this will be like "Total", etc)
    //public string targetActionValue = "Total"; // Value to match
    //public string targetPressedVariableName = "finalPressedValue"; // Name of the Pressed Value variable to check (this will be true or false)
    //public string targetPressedValue = "true"; // Value to match
    private GameObject[] taggedActionBtnObjects;
    private GameObject[] taggedNumberBtnObjects;
    private float sessionTotal; // Used to hold the sum of the value of all pressed buttons
    private string totalString; // The value used to pass a string of a number to functions for display

    // Variables to use for displaying totals values
    public TextMeshPro displayTotalAmount;
    public TextMeshPro displayTotalAmount_Col1; // 1000000
    public TextMeshPro displayTotalAmount_Col2; // 100000 
    public TextMeshPro displayTotalAmount_Col3; // 10000
    public TextMeshPro displayTotalAmount_Col4; // 1000
    public TextMeshPro displayTotalAmount_Col5; // 100
    public TextMeshPro displayTotalAmount_Col6; // 10
    public TextMeshPro displayTotalAmount_Col7; // 1
    public TextMeshPro displayTotalAmount_Col_Fract; // 1/4 1/2 3/4

    public List<ButtonManager> changedButtons; // A list of buttons that have been pressed to use for resetting values after PullHandle() has been run


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Debug.Log("HandleManager script starting");

        // Find all GameObjects with an ActionBtn tag
        taggedActionBtnObjects = GameObject.FindGameObjectsWithTag("ActionBtn");

        // Find all GameObjects with an ActionBtn tag
        taggedNumberBtnObjects = GameObject.FindGameObjectsWithTag("NumberBtn");

    }

    /*
     * Some buttons specify an action that should take place. This function looks for the action buttons that 
     * have been pressed. We do this by looking at the public variable finalAction, which is only set when an 
     * action button is pressed. We will use this later to select a relevant function to call, to carry out 
     * that action.
     */
    private string GetActionValue()
    {

        foreach (GameObject go in taggedActionBtnObjects)
        {
            // Set default value
            action = "";

            // Get the script component that contains the variable
            ButtonManager script = go.GetComponent<ButtonManager>();

            if (script != null)
            {
                // Check if the variable's value matches the target
                if (script.finalAction == "Total") { action = "Total"; }
                else if (script.finalAction == "SubTotal") { action = "SubTotal"; }
                else if (script.finalAction == "NonAdd") { action = "NonAdd"; }
                else if (script.finalAction == "1Qtr") { action = "1Qtr"; }
                else if (script.finalAction == "1Half") { action = "1Half"; }
                else if (script.finalAction == "3Qtr") { action = "3Qtr"; }
                else if (script.finalAction == "Repeat") { action = "Repeat"; }
                else { action = "Number"; }
            }           
        }
        UnityEngine.Debug.Log("GetActionValue returned: " + action);
        return action;

    }

    /*
     * The number buttons will have a value saved to the finalValue variable when the button is pressed. We need 
     * to get all of the number buttons that are pressed and add their final values to get the value that has been 
     * entered by the user each time. This will need to be saved for display separately. First, we find all the 
     * GameObjects tagged with NumberBtn, then look through those to find ones where finalPressedValue is true, 
     * then we get the finalValue value and add it to the total count for each pull of the handle.
     */
    private float GetSessionTotal()
    {

        // Initialise value
        sessionTotal = 0.00f;

        // Loop through each GameObject that is tagged as a number button so we can get the value assigned to each button
        foreach (GameObject go in taggedNumberBtnObjects)
        {

            // Get the script component that contains the value variables for each GameObject
            ButtonManager script = go.GetComponent<ButtonManager>();

            if (script != null)
            {
                //UnityEngine.Debug.Log("In GetSessionTotal(), " + go.name + " - script.finalPressedValue returned: " + script.finalPressedValue);

                // If the button was pressed:  
                if (script.finalPressedValue) {

                    // Add the button's value to the running session total
                    UnityEngine.Debug.Log("In GetSessionTotal(), " + go.name + " - script.finalValue returned: " + script.finalValue);
                    sessionTotal += script.finalValue;

                    // and save the names of the GameObjects for later when we want to reset the button for the next round
                    changedButtons.Add(script);

                }
            }
        }

        UnityEngine.Debug.Log("Finally, GetSessionTotal() returned sessionTotal=" + sessionTotal.ToString());

        return sessionTotal;

    }

    public void PullHandle()
    {
        UnityEngine.Debug.Log("Starting PullHandle: Oh luck be my lady tonight, sings Frank Sinatra...");

        // Initialise variable value each time the function is called
        sessionTotal = 0;

        //Get session running total for the number buttons that have been pressed
        sessionTotal = GetSessionTotal();
        UnityEngine.Debug.Log("In PullHandle, GetSessionTotal returned: " + sessionTotal.ToString());

        // Now get the action buttons that have been pressed
        action = GetActionValue();
        UnityEngine.Debug.Log("In PullHandle, GetActionValue returned: " + action);

        if (action != "")
        {
            // Do actions
            if (action == "Total") {

                DoActionTotal();
                
            } else if (action == "SubTotal") {

                DoActionSubTotal();

            } else if (action == "NonAdd") {

                DoActionNonAdd(); 

            } else if (action == "1Qtr") {

                DoAction1Qtr(); 

            } else if (action == "1Half") {

                DoAction1Half(); 

            } else if (action == "3Qtr") {

                DoAction3Qtr(); 

            } else if (action == "Repeat") {

                DoActionRepeat(); 

            } else {

                DoActionNumber();
                
            }

        }

    }

    /*
     * Set of actions to carry out when the handle has been released.
     */
    public void ReleaseHandle()
    {
        // Reset the values of variables for each button GameObject that has been pressed so it can be pressed again
        foreach (ButtonManager script in changedButtons)
        {
            //UnityEngine.Debug.Log("In ReleaseHandle(), script.finalValue returned: " + script.finalValue);

            UnityEngine.Debug.Log("Resetting button " + script + " to original position");

            // Move button back up to original not-pressed position
            script.transform.localPosition = new Vector3(script.localPositionX, script.localPositionY, script.localPositionZ);
            // Change state of button back to not-pressed
            script.buttonPressed = false;
            // Set the numerical value of the button to be the value to zero
            script.finalValue = 0;
            // Set the action value of the button to be not active
            script.finalAction = "Inactive";
            // Set the final Button Pressed value to be the value specified in the object's inspector window
            script.finalPressedValue = script.buttonPressed;

        }
    }

    private void DoActionTotal()
    {
        UnityEngine.Debug.Log("Start DoActionTotal");
        return;
    }

    private void DoActionSubTotal()
    {
        UnityEngine.Debug.Log("Start DoActionSubTotal");
        return;
    }
    private void DoActionNonAdd()
    {
        UnityEngine.Debug.Log("Start DoActionNonAdd");
        return;
    }
    private void DoAction1Qtr()
    {
        UnityEngine.Debug.Log("Start DoAction1Qtr");
        return;
    }
    private void DoAction1Half()
    {
        UnityEngine.Debug.Log("Start DoAction1Half");
        return;
    }
    private void DoAction3Qtr()
    {
        UnityEngine.Debug.Log("Start DoAction3Qtr");
        return;
    }
    private void DoActionRepeat()
    {
        UnityEngine.Debug.Log("Start DoActionRepeat");
        return;
    }
    private void DoActionNumber()
    {
        UnityEngine.Debug.Log("Start DoActionNumber using sessionTotal: " + sessionTotal.ToString());


        // Only update total displays if there is a value to display. 
        if (sessionTotal > 0)
        {

            // Convert the number to a string
            totalString = sessionTotal.ToString(); ;

            // Assign value to top number display
            displayTotalAmount.text = totalString;


            // Display on front individual number display
            // Pad with zeros on the left to avoid out of bounds issues below
            totalString = totalString.PadLeft(7, '0');

            char num1 = totalString[0];// 1000000 
            char num2 = totalString[1];// 100000 
            char num3 = totalString[2];// 10000 
            char num4 = totalString[3];// 1000 
            char num5 = totalString[4];// 100 
            char num6 = totalString[5];// 10 
            char num7 = totalString[6];// 1 

            if (num1 != '\0') { displayTotalAmount_Col1.text = num1.ToString(); } else { displayTotalAmount_Col1.text = "0"; }
            ; // 1000000
            if (num2 != '\0') { displayTotalAmount_Col2.text = num2.ToString(); } else { displayTotalAmount_Col2.text = "0"; }
            ; // 100000
            if (num3 != '\0') { displayTotalAmount_Col3.text = num3.ToString(); } else { displayTotalAmount_Col3.text = "0"; }
            ; // 10000
            if (num4 != '\0') { displayTotalAmount_Col4.text = num4.ToString(); } else { displayTotalAmount_Col4.text = "0"; }
            ; // 1000
            if (num5 != '\0') { displayTotalAmount_Col5.text = num5.ToString(); } else { displayTotalAmount_Col5.text = "0"; }
            ; // 100
            if (num6 != '\0') { displayTotalAmount_Col6.text = num6.ToString(); } else { displayTotalAmount_Col6.text = "0"; }
            ; // 10
            if (num7 != '\0') { displayTotalAmount_Col7.text = num7.ToString(); } else { displayTotalAmount_Col7.text = "0"; }
            ; // 1

            // Need to work out how to deal with fractions
            displayTotalAmount_Col_Fract.text = "-"; // 1/4 1/2 3/4
            
        }

        return;
    }

}


