using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HandleManager : MonoBehaviour
{
    private string action;

    public string targetActionVariableName = "finalAction"; // Name of the Action variable to check (this will be like "Total", etc)
    public string targetActionValue = "Total"; // Value to match
    public string targetPressedVariableName = "finalPressedValue"; // Name of the Pressed Value variable to check (this will be true or false)
    public string targetPressedValue = "true"; // Value to match
    private GameObject[] taggedActionBtnObjects;
    private GameObject[] taggedNumberBtnObjects;
    private float sessionTotal; // Used to hold the sum of the value of all pressed buttons


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

        foreach (GameObject go in taggedNumberBtnObjects)
        {
            // Initialise value
            sessionTotal = 0.00f;

            // Get the script component that contains the variables
            ButtonManager script = go.GetComponent<ButtonManager>();

            if (script != null)
            {
                // If the button was pressed, add the button's value to the running session total
                if (script.finalPressedValue == true) { sessionTotal += script.finalValue; }
            }
        }
        UnityEngine.Debug.Log("GetSessionTotal returned" + sessionTotal.ToString());
        return sessionTotal;

    }

    public void PullHandle()
    {
        UnityEngine.Debug.Log("Starting PullHandle: Oh luck be my lady tonight, sings Frank Sinatra...");

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
        return;
    }

}


