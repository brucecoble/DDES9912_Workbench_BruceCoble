using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using TMPro;
using System.Linq;

public class HandleManager : MonoBehaviour
{
    private string action;

    private GameObject[] taggedActionBtnObjects; // All the buttons tagged with ActionBtn
    private GameObject[] taggedNumberBtnObjects; // All the buttons tagged with NumberBtn
    private GameObject taggedHandleObject; // Handle Graphic GameObject

    private float buttonsTotal; // Used to hold the sum of the value of all pressed buttons in the current session
    public float runningTotal; // USed to hold the running total of all values added.
    private string totalString; // The value used to pass a string of a number to functions for display
    private string frontTotalString; // The value used to pass a string of a number for display in the front total window
    private string runningTotalString; // The value of the running total converted to a string

    // Variables to use for displaying running totals values in the front display assigned on HandleRig GameObject
    public TextMeshPro displayTotalAmount;
    public TextMeshPro displayPaperPrintout;
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

    // A string of all the values entered to be used for the top display (i.e. the paper printout)
    public string paperPrintoutString;
    public List<string> paperPrintoutValues; // Try a list

    // Also create a new object to avoid a multithreading error generated while looping through & adding to the valueEntered list
    private readonly object listlock = new object();

    // Also create a new object to avoid a multithreading error generated while looping through & adding to the paperPrintoutValues list
    private readonly object listlock2 = new object();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Debug.Log("HandleManager script starting");

        // Find all GameObjects with an ActionBtn tag
        taggedActionBtnObjects = GameObject.FindGameObjectsWithTag("ActionBtn");

        // Find all GameObjects with an ActionBtn tag
        taggedNumberBtnObjects = GameObject.FindGameObjectsWithTag("NumberBtn");

        // Find Handle GameObject with Handle tag
        taggedHandleObject = GameObject.FindWithTag("Handle");
        

        // Initialise values that we will add to as the user interacts with the GameObjects
        //valuesEntered = null;
        //paperPrintoutString = "";

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

        // Now loop through each GameObject that is tagged as an Action button so we can add them to the list of changedButtons for resetting later
        foreach (GameObject go in taggedActionBtnObjects)
        {

            // Get the script component that contains the value variables for each GameObject
            ButtonManager script = go.GetComponent<ButtonManager>();

            if (script != null)
            {
                //UnityEngine.Debug.Log("In GetButtonsTotal(), " + go.name + " - script.finalPressedValue returned: " + script.finalPressedValue);

                // If the button was pressed:  
                if (script.finalPressedValue)
                {

                    // Add the button's value to the running session total
                    UnityEngine.Debug.Log("In GetButtonsTotal(), add Action Button to changedButtons: " + go.name);
                   
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

        UnityEngine.Debug.Log("updateRunningTotal() start.............................");
        string listString = string.Join(", ", valuesEntered);

        //UnityEngine.Debug.Log("action = " + action + ", buttonsTotal = " + buttonsTotal.ToString() + ", runningTotal = " + runningTotal.ToString());
        UnityEngine.Debug.Log("UpdateRunningTotal() - action = " + action + ", valuesEntered of " + listString);

        // Initialise variable
        runningTotal = 0.00f;

        if (action == "Number")
        {

            UnityEngine.Debug.Log("UpdateRunningTotal() - action is Number so adding valuesEntered of " + listString);

            foreach (float value in valuesEntered)
            {
                runningTotal += value;
            }

            UnityEngine.Debug.Log("updateRunningTotal() - action==Number returned: " + runningTotal.ToString() );
            return runningTotal;

        }

        UnityEngine.Debug.Log("updateRunningTotal() - nothing to change.............................");
        return runningTotal;

    }


    /*
     * Users enter a number & pull the handle to finalise the number entry. The buttons then reset, allowing the user to enter a new number.
     * This function adds each number to the previous total to create a new total
     * 
     */
    private string FormatPaperPrintout(List<string> paperPrintoutValues, float runningTotal, string action)
    {

        UnityEngine.Debug.Log("FormatPaperPrintout() start.............................");
        string listString = string.Join(", ", paperPrintoutValues);

        //UnityEngine.Debug.Log("action = " + action + ", buttonsTotal = " + buttonsTotal.ToString() + ", runningTotal = " + runningTotal.ToString());
        UnityEngine.Debug.Log("FormatPaperPrintout() - action = " + action + ", valuesEntered of " + listString);

        // Initialise variable
        paperPrintoutString = "";

        if (action == "Number")
        {

            UnityEngine.Debug.Log("FormatPaperPrintout() - action is Number so adding valuesEntered of " + listString);

            //paperPrintoutValues = string.Join("\n", paperPrintoutValues);
            
            foreach (string value in paperPrintoutValues)
            {
                paperPrintoutString += value + '\n';
            }
            
            UnityEngine.Debug.Log("FormatPaperPrintout() - action==Number returned: " + paperPrintoutString );

            return paperPrintoutString;

        } else if (action == "Total")
        {

            UnityEngine.Debug.Log("FormatPaperPrintout() - action is Total so adding valuesEntered of " + listString + " plus total");

            //paperPrintoutValues = string.Join("\n", paperPrintoutValues);

            foreach (string value in paperPrintoutValues)
            {
                paperPrintoutString += value + '\n';
            }

            // Now add total line
            paperPrintoutString += "---------" + '\n' + runningTotal.ToString();


            UnityEngine.Debug.Log("FormatPaperPrintout() - action==Total returned: " + paperPrintoutString);

            return paperPrintoutString;

        }

        UnityEngine.Debug.Log("FormatPaperPrintout() - nothing to change.............................");

        return paperPrintoutString;

    }

    private void MoveHandle(GameObject handleGraphic, string direction)
    {
        float tiltAngle = 0.0f;
        //bool hasMoved = false; // Use this to ensure the release does not trigger if the pull handle did not trigger in the first place

        // Set the amount to move
        if (direction == "Forward")
        {
            tiltAngle = 80.0f;
        }
        else
        {
            tiltAngle = -80.0f;
        }

        if (handleGraphic != null)
        {
            handleGraphic.transform.Rotate(Vector3.up, tiltAngle);
        }

    }
    

    /*
     * 
     * 
     */
    public void PullHandle()
    {
        UnityEngine.Debug.Log("Starting PullHandle: Oh luck be my lady tonight, sings Frank Sinatra...");

        // Initialise variable value each time the function is called
        buttonsTotal = 0;

        //Get total for the number buttons that have been pressed for this specific number entry.
        buttonsTotal = GetButtonsTotal();
        UnityEngine.Debug.Log("In PullHandle, GetSessionTotal returned: " + buttonsTotal.ToString());

        // Safely add value to public list of entered float values in HandleManager (avoiding multithreading error)
        lock (listlock)
        {
            valuesEntered.Add(buttonsTotal);
        }        
        
        // Safely add value to public list of entered string values in HandleManager (avoiding multithreading error)
        
        lock (listlock2)
        {
            // We don't want to print zeros
            if (buttonsTotal > 0)
            {
                paperPrintoutValues.Add(buttonsTotal.ToString());
            }
            
        }


        // Add buttons total to our paper printout display string
        //string newString = buttonsTotal.ToString();
        //UnityEngine.Debug.Log("In PullHandle, paperPrintoutString is currently: " + paperPrintoutString);
        //paperPrintoutString += string.Join("\n\r\n\r", newString);
        //UnityEngine.Debug.Log("In PullHandle, paperPrintoutString is now: " + paperPrintoutString);

        // Now get the action buttons that have been pressed
        action = GetActionValue();
        UnityEngine.Debug.Log("In PullHandle, GetActionValue returned: " + action);

        if (action != "")
        {
            // Turn the HandleGraphic GameObject but only if there is an action (i.e. only if a button has been pressed)
            MoveHandle(taggedHandleObject, "Forward");

            // Do actions
            if (action == "Total") {

                // This prints the total at the bottom of the top display in red with a "T" next to it
                // and resets the front display to zero (you would normally rip the printed paper off 
                // from the top typewriter display section & start a new addition task).

                // Where do we store/get running total from? Do we put this in a game object?
                UnityEngine.Debug.Log("In PullHandle, doing Total action...");

                //runningTotal = UpdateRunningTotal(buttonsTotal, runningTotal, "");
                runningTotal = UpdateRunningTotal(valuesEntered, action);
                UnityEngine.Debug.Log("In PullHandle, paperPrintoutString is currently: " + paperPrintoutString);

                paperPrintoutString = FormatPaperPrintout(paperPrintoutValues, runningTotal, action);
                UnityEngine.Debug.Log("In PullHandle, paperPrintoutString is now: " + paperPrintoutString);

                // Send outputs to the display object value variables
                DisplayResult(buttonsTotal, runningTotal, paperPrintoutString);

                //DoActionTotal();
                
            } else if (action == "SubTotal") {

                // This prints the subtotal value at the bottom of the top display in red with a "T" next to it
                // and resets the front display to zero (you would normally rip the printed paper off 
                // from the top typewriter display section & start a new addition task).

                DoActionSubTotal();

            } else if (action == "NonAdd") {

                // This prints the number at the bottom of the top display in black with an "N" next to it,
                // but does not add it to the running total at the front

                DoActionNonAdd(); 

            } else if (action == "1Qtr") {

                DoAction1Qtr(); 

            } else if (action == "1Half") {

                DoAction1Half(); 

            } else if (action == "3Qtr") {

                DoAction3Qtr(); 

            } else if (action == "Repeat") {

                // This keeps the Repeat button and any pressed number buttons remaining pressed until the
                // Repeat button is unpressed. This allows the user to keep pulling the handle and adding
                // the same number

                DoActionRepeat(); 

            } else if (action == "Number") {

                // Where do we store/get running total from? Do we put this in a game object?
                UnityEngine.Debug.Log("In PullHandle, doing Number action...");
                //DoActionNumber();

                //runningTotal = UpdateRunningTotal(buttonsTotal, runningTotal, "");
                runningTotal = UpdateRunningTotal(valuesEntered, action);
                UnityEngine.Debug.Log("In PullHandle, paperPrintoutString is currently: " + paperPrintoutString);

                paperPrintoutString = FormatPaperPrintout(paperPrintoutValues, runningTotal, action);
                UnityEngine.Debug.Log("In PullHandle, paperPrintoutString is now: " + paperPrintoutString);

                // Send outputs to the display object value variables
                DisplayResult(buttonsTotal, runningTotal, paperPrintoutString);

            } else {

                UnityEngine.Debug.Log("In PullHandle actions section, doing ELSE...");

            }

        }

    }

    /*
     * Set of actions to carry out when the handle has been released.
     */
    public void ReleaseHandle()
    {

        // Now get the action buttons that have been pressed
        action = GetActionValue();
        UnityEngine.Debug.Log("In PullHandle, GetActionValue returned: " + action);

        if (action != null)
        {
            // Turn the HandleGraphic GameObject
            MoveHandle(taggedHandleObject, "Backward");
        }

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

        // Set action to null so user cannot pull the handle
        action = null;

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
    private void DisplayResult(float buttonsTotal, float runningTotal, string paperPrintoutString)
    {
        UnityEngine.Debug.Log("Start DisplayResult() using buttonsTotal: " + buttonsTotal.ToString() + ", runningTotal: " + runningTotal.ToString());
        UnityEngine.Debug.Log("Start DisplayResult() using paperPrintoutString: " + paperPrintoutString);

                // Only update total displays if there is a value to display. 
        if (buttonsTotal > 0)
        {

            // Convert the number to a string
            totalString = buttonsTotal.ToString();

            // Convert running total to a string
            runningTotalString = runningTotal.ToString();

            // Assign value to top number display
            displayTotalAmount.text = totalString;  

            // Assign value to top paper printout display
            displayPaperPrintout.text = paperPrintoutString;


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


