using System.Collections.Generic;
using UnityEngine;

public class InteractableButtonsCaller : MonoBehaviour
{
    public List<GameObject> buttonsToPress; // List of all the buttons we want to press
    private InteractableGeneral buttonPress; // The interactible general script of the object in our list

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject go in buttonsToPress)
        {
            if (go != null)
            {
                Debug.Log("GameObject is: " + go.name);

                // Get the script attached to the button GameObject
                buttonPress = go.GetComponent<InteractableGeneral>();

                if (buttonPress != null)
                {
                    // Invoke the interaction events (i.e. press the button & make the sound)
                    buttonPress.onPrimaryInteract.Invoke();
                    Debug.Log("buttonPress is: " + buttonPress.name);
                }
                else
                {
                    Debug.LogError("Can't find me no script, pal");
                }
            }
        }
    }
            // Update is called once per frame
            void Update()
    {
        
    }
}
