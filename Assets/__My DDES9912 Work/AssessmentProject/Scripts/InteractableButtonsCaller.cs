using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class InteractableButtonsCaller : MonoBehaviour
{
    //public TextAsset jsonFile; // Assign your JSON file (as TextAsset) in the Inspector
    //public List<GameObject> buttonsToPress; // List of all the buttons we want to press
    //public List<float> numbersToPress; // List of all the numbers to press
    private InteractableGeneral buttonPress; // The interactible general script of the object in our list
    private AudioSource audioSource; // For the sound effects
    public float delaytime = 0.5f; // Set the delay time to use between actions


    [Header("Data")]
    [Tooltip("JSON with a 'buttons' array of { id:string, value:int }")]
    public TextAsset jsonFile;

    [Tooltip("Numbers you want to match to 'value' in the JSON")]
    public List<int> numbersToMatch = new List<int> { 10000, 1000, 100 };

    // --- JSON models (renamed as requested) ---
    [Serializable]
    public class Buttons
    {
        public string id;   // GameObject name
        public string action;   // GameObject action
        public int button_value;   // Number to match
    }

    [Serializable]
    public class ButtonsList
    {
        public List<Buttons> buttons;
    }


    // value -> GameObject name
    private Dictionary<int, string> valueToName;

    // Optionally cache found GameObjects too
    private Dictionary<int, GameObject> valueToGO;



    // Command script from The Boss
    // "Add these numbers"
    // 10500, 349, 250000
    // Now give me the total
    // value=10000, action="number"
    // value=500, action="number"
    // Pull Handle to add
    // value=300, action="number"
    // value=40, action="number"
    // value=9, action="number"
    // Pull Handle to add
    // value=200000, action="number"
    // value=50000, action="number"
    // value=0, action="total"
    // Pull Handle to run total
    // "Nice work. Let's do another one"


    /*

    [Serializable]
    public class MyItem // Represents a single item in the list
    {
        public string id;
        public string action;
        public int value;
    }

    [Serializable]
    public class MyItemList // Holds the list of items
    {
        public List<MyItem> buttons; // The name of this field must match the JSON array key
    }
    */


    private IEnumerator Start()
    {

        /*
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            MyItemList data = JsonUtility.FromJson<MyItemList>(jsonString);

            // Accessing items from the list
            if (data != null && data.buttons != null && data.buttons.Count > 0)
            {
                // Select the first item
                MyItem firstItem = data.buttons[0];
                Debug.Log($"First item: ID={firstItem.id}, Value={firstItem.value}");

                // Select an item by index (e.g., the third item)
                if (data.buttons.Count > 2)
                {
                    MyItem thirdItem = data.buttons[2];
                    Debug.Log($"Third item: ID={thirdItem.id}, Value={thirdItem.value}");
                }

                // Iterate through the list to find a specific item
                foreach (MyItem item in data.buttons)
                {
                    if (item.id == "ButtonBlack_Col2_Row1")
                    {
                        Debug.Log($"Found specific item: ID={item.id}, Value={item.value}");
                        break; // Exit the loop once found
                    }
                }
            }
        }
        else
        {
            Debug.LogError("JSON file not assigned!");
        }
        */

        if (jsonFile == null)
        {
            Debug.LogError("No JSON file assigned.");
            //return;
        }

        var list = JsonUtility.FromJson<ButtonsList>(jsonFile.text);
        if (list == null || list.buttons == null)
        {
            Debug.LogError("JSON couldn't be parsed or has no 'buttons' array.");
            //return;
        }

        valueToName = new Dictionary<int, string>(list.buttons.Count);
        foreach (var b in list.buttons)
        {
            // last-one-wins; change if you want to warn on duplicates
            valueToName[b.button_value] = b.id;
        }


        //Debug.Log("Starting CapsuleTriggerReturn");

        // Step 1: Loop through the list of number buttons and press each one
        yield return StartCoroutine(PressEachButton());

        // Step 2: A little pause before we pull the handle
        yield return new WaitForSeconds(delaytime);

        // Step 3: Pull the handle
        yield return StartCoroutine(PullTheHandle());

        // Step 4: A little pause before we let go
        yield return new WaitForSeconds(delaytime);

        // Step 5: Mow we release the handle
        yield return StartCoroutine(ReleaseTheHandle());
    }
    /*
    IEnumerator PressEachButton()
    {
        foreach (GameObject go in buttonsToPress)
        {
            if (go != null)
            {

                ButtonManager script = go.GetComponent<ButtonManager>();
                script.PressButton();

                // Find a GameObject named "MyObject" in the scene
                GameObject sfxGo = GameObject.Find("ButtonPress");
                audioSource = sfxGo.GetComponent<AudioSource>();
                audioSource.Play();

                yield return new WaitForSeconds(delaytime);

            }
        }
    }
    */

    /*

    GameObject FindButtonWithValue(float value)
    {
        GameObject[] numberButtons = GameObject.FindGameObjectsWithTag("NumberBtn");

        foreach (GameObject btn in numberButtons)
        {
            ButtonManager buttonScript = btn.GetComponent<ButtonManager>();
            if (buttonScript != null && Mathf.Approximately(buttonScript.buttonValue, value))
            {
                return btn;
            }
        }

        return null; // Not found
    }


    IEnumerator PressEachButton()
    {
        foreach (float targetValue in numbersToPress)
        {
            GameObject go = FindButtonWithValue(targetValue);

            if (go != null)
            {
                Debug.Log("Found GameObject: " + go.name);
                ButtonManager script = go.GetComponent<ButtonManager>();
                script.PressButton();

                // Find a GameObject named "MyObject" in the scene
                GameObject sfxGo = GameObject.Find("ButtonPress");
                audioSource = sfxGo.GetComponent<AudioSource>();
                audioSource.Play();

            }
            else
            {
                Debug.Log("No GameObject found with value " + targetValue);
            }

            yield return new WaitForSeconds(delaytime);

        }
    }
    */

    IEnumerator PressEachButton()
    {
        valueToGO = new Dictionary<int, GameObject>(numbersToMatch.Count);

        foreach (var number in numbersToMatch)
        {
            if (!valueToName.TryGetValue(number, out var goName) || string.IsNullOrEmpty(goName))
            {
                Debug.LogWarning($"No record for value {number}.");
                continue;
            }

            var go = GameObject.Find(goName); // okay for setup; avoid every-frame usage
            if (go != null)
            {
                valueToGO[number] = go;
                Debug.Log($"Matched value {number} → id \"{goName}\" → GameObject found: {go.name}");
                // TODO: use 'go' as needed
                ButtonManager script = go.GetComponent<ButtonManager>();
                script.PressButton();

                // Find a GameObject named "MyObject" in the scene
                GameObject sfxGo = GameObject.Find("ButtonPress");
                audioSource = sfxGo.GetComponent<AudioSource>();
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Record found for value {number} → id \"{goName}\", but no GameObject with that name exists in the scene.");
            }
        }
        yield return new WaitForSeconds(delaytime);
        /*
        foreach (float targetValue in numbersToPress)
        {

            // Accessing items from the JSON data list
            if (data != null && data.buttons != null && data.buttons.Count > 0)
            {

                // Iterate through the list to find a specific item
                foreach (MyItem item in data.buttons)
                {
                    if (item.value == targetValue)
                    {
                        Debug.Log($"Found specific item: ID={item.id}, Value={item.value}");
                        GameObject go = GameObject.Find(item.id);

                        if (go != null)
                        {
                            Debug.Log("Found GameObject: " + go.name);
                            ButtonManager script = go.GetComponent<ButtonManager>();
                            script.PressButton();

                            // Find a GameObject named "MyObject" in the scene
                            GameObject sfxGo = GameObject.Find("ButtonPress");
                            audioSource = sfxGo.GetComponent<AudioSource>();
                            audioSource.Play();

                        }
                        else
                        {
                            Debug.Log("No GameObject found with value " + targetValue);
                        }

                        break; // Exit the loop once found

                    }
                }
            }


            yield return new WaitForSeconds(delaytime);

        }
        */
    }

    IEnumerator PullTheHandle()
    {
        // Find a GameObject named "MyObject" in the scene
        GameObject handleRig = GameObject.Find("Handle Rig");

        HandleManager script = handleRig.GetComponent<HandleManager>();
        script.PullHandle();

        // Find a GameObject named "MyObject" in the scene
        GameObject sfxGo = GameObject.Find("HandlePull");
        audioSource = sfxGo.GetComponent<AudioSource>();
        audioSource.Play();

        yield return null;

    }

    IEnumerator ReleaseTheHandle()
    {
        // Find a GameObject named "MyObject" in the scene
        GameObject handleRig = GameObject.Find("Handle Rig");

        HandleManager script = handleRig.GetComponent<HandleManager>();
        script.ReleaseHandle();

        // Find a GameObject named "MyObject" in the scene
        GameObject sfxGo = GameObject.Find("HandlePull");
        audioSource = sfxGo.GetComponent<AudioSource>();
        audioSource.Play();

        yield return null;

    }

}
