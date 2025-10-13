using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HandleManager : MonoBehaviour
{
    private string action;

    public string targetActionVariableName = "finalAction"; // Name of the Action variable to check (this will be like "Total", etc)
    public string targetActionValue = "Total"; // Value to match
    public string targetPressedVariableName = "finalPressedValue"; // Name of the Pressed Value variable to check (this will be true or false)
    public string targetPressedValue = "true"; // Value to match
    private GameObject[] taggedActionBtnObjects;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Debug.Log("HandleManager script starting");

        // Find all GameObjects with an ActionBtn tag
        taggedActionBtnObjects = GameObject.FindGameObjectsWithTag("ActionBtn");

        // Find all GameObjects with an ActionBtn tag
        //GameObject[] taggedNumberBtnObjects = GameObject.FindGameObjectsWithTag("NumberBtn");

    }

    private string GetActionValue()
    {

        foreach (GameObject go in taggedActionBtnObjects)
        {

            // Get the script component that contains the variable
            ButtonManager script = go.GetComponent<ButtonManager>();

            if (script != null)
            {
                // Check if the variable's value matches the target
                if (script.finalAction == "Total")
                {
                    UnityEngine.Debug.Log("Found GameObject with Total variable: " + go.name);
                    // Do something with the found GameObject
                }
            }
            
        }

        action = "";

        return action;
    }

    public void PullHandle()
    {
        UnityEngine.Debug.Log("Oh luck be my lady tonight, sings Frank Sinatra...");

        action = GetActionValue();
    }

}