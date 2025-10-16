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

    private GameObject[] taggedActionBtnObjects; // All the buttons tagged with ActionBtn
    private GameObject[] taggedNumberBtnObjects; // All the buttons tagged with NumberBtn
    private float buttonsTotal; // Used to hold the sum of the value of all pressed buttons in the current session
    public float runningTotal; // USed to hold the running total of all values added.
    private string totalString; // The value used to pass a string of a number to functions for display
    private string frontTotalString; // The value used to pass a string of a number for display in the front total window
    private string runningTotalString; // The value of the running total converted to a string

    // Variables to use for displaying running totals values in the front display
    public TextMeshPro displayTotalAmount;
    public TextMeshPro displayTotalAmount_Col1; // 1000000 column
    public TextMeshPro displayTotalAmount_Col2; // 100000 column
    public TextMeshPro displayTotalAmount_Col3; // 10000 column
    public TextMeshPro displayTotalAmount_Col4; // 1000 column
    public TextMeshPro displayTotalAmount_Col5; // 100 column
    public TextMeshPro displayTotalAmount_Col6; // 10 column
    public TextMeshPro displayTotalAmount_Col7; // 1 column
    public TextMeshPro displayTotalAmount_Col_Fract; // 1/4 1/2 3/4 - NOTE: Not displaying these for now...

    // A list of buttons that have been pressed to use for resetting values after PullHandle() has been run
    public List<ButtonManager> changedButtons;

    // A list of values that have been entered by the user. This is used to record each individual float entered by the user,
    // to allow a list to be displayed at the top and a running total to be calculated.
    public List<float> valuesEntered; 


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
     * have been pressed. We do this by looking at the public variable from the ButtonManager script which is 
     * assigned to each button finalAction, & which is only set when an action button is pressed. 
     * We will use this later to select a relevant function to call, to carry out that action.
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
     * 
     * Returns: the combined value of the values assigned to all of the pressed buttons for a single number entry
     */
    private float GetButtonsTotal()
    {

        // Initialise value
        buttonsTotal = 0.00f;

        // Loop through each GameObject that is tagged as a number button so we can get the value assigned to each button
        foreach (GameObject go in taggedNumberBtnObjects)
        {

            // Get the script component that contains the value variables for each GameObject
            ButtonManager script = go.GetComponent<ButtonManager>();

            if (script != null)
            {
                //UnityEngine.Debug.Log("In GetButtonsTotal(), " + go.name + " - script.finalPressedValue returned: " + script.finalPressedValue);

                // If the button was pressed:  
                if (script.finalPressedValue) {

                    // Add the button's value to the running session total
                    UnityEngine.Debug.Log("In GetButtonsTotal(), " + go.name + " - script.finalValue returned: " + script.finalValue);
                    buttonsTotal += script.finalValue;

                    // and save the names of the GameObjects for later when we want to reset the button for the next round
                    changedButtons.Add(script);

                }
            }
        }

        UnityEngine.Debug.Log("Finally, GetButtonsTotal() returned buttonsTotal=" + buttonsTotal.ToString());

        return buttonsTotal;

    }

    /*
     * Users enter a number & pull the handle to finalise the number entry. The buttons then reset, allowing the user to enter a new number.
     * This function adds each number to the previous total to create a new total
     * 
     */
    private float UpdateRunningTotal(List<float> valuesEntered, string action)
    {

        UnityEngine.Debug.Log("updateRunningTotal start.............................");
        string listString = string.Join(", ", valuesEntered);

        //UnityEngine.Debug.Log("action = " + action + ", buttonsTotal = " + buttonsTotal.ToString() + ", runningTotal = " + runningTotal.ToString());
        UnityEngine.Debug.Log("action = " + action + ", valuesEntered of " + listString);

        // Initialise variable
        runningTotal = 0.00f;

        if (action is null)
        {

            /*
            UnityEngine.Debug.Log("action is null so adding buttonsTotal of " + buttonsTotal.ToString() + " runningTotal of: " + runningTotal.ToString());
            runningTotal += buttonsTotal;
            UnityEngine.Debug.Log("RunningTotal is now: " + runningTotal.ToString());
            */
            UnityEngine.Debug.Log("action is null so adding valuesEntered of " + listString);

            foreach (float value in valuesEntered)
            {
                runningTotal += value;
            }


            return runningTotal;
        }

        UnityEngine.Debug.Log("updateRunningTotal - nothing to change.............................");
        return runningTotal;

    }

    /*
     * 
     * 
     */
    public void PullHandle()
    {
        UnityEngine.Debug.Log("Starting PullHandle: Oh luck be my lady tonight, sings Frank Sinatra...");

        // Initialise variable value each time the function is called
        //buttonsTotal = 0;

        //Get total for the number buttons that have been pressed for this specific number entry.
        buttonsTotal = GetButtonsTotal();
        UnityEngine.Debug.Log("In PullHandle, GetSessionTotal returned: " + buttonsTotal.ToString());

        // Add value to public list of entered values in HandleManager - we can total these later
        valuesEntered.Add(buttonsTotal);

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

                // Where do we store/get running total from? Do we put this in a game object?
                UnityEngine.Debug.Log("In PullHandle, doing ELSE action...");
                //DoActionNumber();

                //runningTotal = UpdateRunningTotal(buttonsTotal, runningTotal, "");
                runningTotal = UpdateRunningTotal(valuesEntered, "");
                DisplayResult(buttonsTotal, runningTotal);

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
        UnityEngine.Debug.Log("Start DoActionNumber");
        //DisplayResult();

        return;

    }


    /*
     * Create result for display on display GameObjects
     * 
     * This updates the value of the public Text Mesh Pro variables declared at the top of this script & 
     * that have been assigned a GameObject via the inspector
     */
    private void DisplayResult(float buttonsTotal, float runningTotal)
    {
        UnityEngine.Debug.Log("Start DoActionNumber using buttonsTotal: " + buttonsTotal.ToString());


        // Only update total displays if there is a value to display. 
        if (buttonsTotal > 0)
        {

            // Convert the number to a string
            totalString = buttonsTotal.ToString();

            // Convert running total to a string
            runningTotalString = runningTotal.ToString();

            // Assign value to top number display
            displayTotalAmount.text = totalString;


            // Display on front individual number display
            // Pad with zeros on the left to avoid out of bounds issues below
            // Getting each individual value from a number of variable length requiring left-padding is tricky,
            // so there are some float to string to char to string conversions required.
            frontTotalString = runningTotalString.PadLeft(7, '0');

            char num1 = frontTotalString[0];// 1000000 column
            char num2 = frontTotalString[1];// 100000 column 
            char num3 = frontTotalString[2];// 10000 column 
            char num4 = frontTotalString[3];// 1000 column 
            char num5 = frontTotalString[4];// 100 column 
            char num6 = frontTotalString[5];// 10 column 
            char num7 = frontTotalString[6];// 1 column 

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

            // Need to work out how to deal with fractions so leaving as a dash for now...
            displayTotalAmount_Col_Fract.text = "-"; // 1/4 1/2 3/4

        }

        return;
    }

}


