using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class InteractableButtonsCaller : MonoBehaviour
{
    private InteractableGeneral buttonPress; // The interactible general script of the object in our list
    private AudioSource audioSource; // For the sound effects
    public float delaytime = 3f; // Set the delay time to use between actions

    public BossWalk bossWalk;

    [Header("Data")]
    [Tooltip("JSON in References folder with a 'buttons' array of { id:string, action:string, button_value:int }")]
    public TextAsset jsonFile;

    [Tooltip("Tuple of action & numbers to use to find the button object in the JSON")]
    public List<(string action, int button_value)> whatToPress = new List<(string, int)>();

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

    [System.Serializable]
    public class ButtonData
    {
        public string id;           // string
        public string action;       // string
        public int button_value;    // int
        public string audio;        // string
    }

    [System.Serializable]
    public class ButtonsList
    {
        public List<ButtonData> buttons;
    }

    // Indexes
    private Dictionary<int, ButtonData> _byValueNonZero;
    private Dictionary<string, ButtonData> _zeroByAction;

    void Awake()
    {

        // Populate the list
        whatToPress.Add(("number",   30000));
        whatToPress.Add(("number",    2000));
        whatToPress.Add(("number",      10));
        whatToPress.Add(("addnumber",   0));
        whatToPress.Add(("number",   20000));
        whatToPress.Add(("number",    3000));
        whatToPress.Add(("number",      30));
        whatToPress.Add(("addnumber",   0));
        whatToPress.Add(("number", 1000000));
        whatToPress.Add(("number",  200000));
        whatToPress.Add(("number",   10000));
        whatToPress.Add(("total",        0));

        whatToPress.Add(("number",   20000));
        whatToPress.Add(("number",    3000));
        whatToPress.Add(("number",      30));
        whatToPress.Add(("subtotal",     0));
        whatToPress.Add(("number",      10));
        whatToPress.Add(("total",        0));

        whatToPress.Add(("number", 1000000));
        whatToPress.Add(("number",  200000));
        whatToPress.Add(("number",   10000));
        whatToPress.Add(("nonadd",       0));
        whatToPress.Add(("number",   30000));
        whatToPress.Add(("total",        0));

        var list = JsonUtility.FromJson<ButtonsList>(jsonFile.text);

        _byValueNonZero = new Dictionary<int, ButtonData>();
        _zeroByAction = new Dictionary<string, ButtonData>(System.StringComparer.OrdinalIgnoreCase);

        foreach (var b in list.buttons)
        {
            if (b == null) continue;

            if (b.button_value != 0)
            {
                // If you expect uniqueness for non-zero values:
                if (_byValueNonZero.ContainsKey(b.button_value))
                {
                    Debug.LogWarning($"Duplicate non-zero button_value {b.button_value} for id={b.id}. Overwriting previous entry.");
                }
                _byValueNonZero[b.button_value] = b;
            }
            else
            {
                // For zero values, index by action
                if (string.IsNullOrWhiteSpace(b.action))
                {
                    Debug.LogWarning($"Zero button_value record missing action (id={b.id}). Skipping.");
                    continue;
                }
                if (_zeroByAction.ContainsKey(b.action))
                {
                    Debug.LogWarning($"Duplicate zero-value action '{b.action}' encountered. Overwriting previous entry.");
                }
                _zeroByAction[b.action] = b;
            }
        }
    }

    /// <summary>
    /// Find by non-zero button_value. Returns true if found.
    /// </summary>
    public bool TryGetByButtonValue(int buttonValue, out ButtonData data)
    {
        if (buttonValue == 0)
        {
            data = null;
            return false; // Force callers to use TryGetZeroByAction for zero.
        }
        return _byValueNonZero.TryGetValue(buttonValue, out data);
    }

    /// <summary>
    /// Find a zero-value record by action (case-insensitive). Returns true if found.
    /// </summary>
    public bool TryGetZeroByAction(string action, out ButtonData data)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            data = null;
            return false;
        }
        return _zeroByAction.TryGetValue(action.Trim(), out data);
    }

    /// <summary>
    /// Convenience method: if buttonValue != 0, search by value; if 0, search by action.
    /// Returns true if found.
    /// </summary>
    public bool TryFind(int buttonValue, string actionForZero, out ButtonData data)
    {
        if (buttonValue != 0)
        {
            return TryGetByButtonValue(buttonValue, out data);
        }
        return TryGetZeroByAction(actionForZero, out data);
    }


    private IEnumerator Start()
    {
        // Step 1: Loop through the list of number buttons and press each one
        yield return StartCoroutine(PressEachButton());

        // Make sure the typist stops typing at the end.
        bossWalk.TypistStopTyping();
    }

    IEnumerator PressEachButton()
    {
        // Make The Boss talk each time a button is pressed
        bossWalk.TypistTyping();
        bossWalk.BossTalking();

        yield return new WaitForSeconds(delaytime);

        // Make boss walk to a random spot
        bossWalk.MoveToRandomPosition();


        // Loo through tuples of instructions
        foreach (var t in whatToPress)
        {
            bool _is_total = false;

            // Make The Boss talk each time a button is pressed
            bossWalk.TypistTyping();
            bossWalk.BossTalking();

            if (t.action == "number")
            {
                // Do number action
                // Non-zero button_value → find by value (because values are unique)
                if (TryGetByButtonValue(t.button_value, out var nonZero))
                {
                    Debug.Log($"Found by value 42 → id={nonZero.id}, action={nonZero.action}, button_value={nonZero.button_value}");

                    // Play the boss audio for the selected number
                    GameObject sfxNumberGo = GameObject.Find(nonZero.audio);
                    audioSource = sfxNumberGo.GetComponent<AudioSource>();
                    audioSource.Play();
                    yield return new WaitForSeconds(delaytime);

                    var go = GameObject.Find(nonZero.id); // okay for setup; avoid every-frame usage
                    if (go != null)
                    {
                        ButtonManager script = go.GetComponent<ButtonManager>();
                        script.PressButton();

                        // Find a GameObject named "ButtonPress" in the scene
                        GameObject sfxGo = GameObject.Find("ButtonPress");
                        audioSource = sfxGo.GetComponent<AudioSource>();
                        audioSource.Play();
                    }
                    else
                    {
                        Debug.LogWarning($"Record found for value {t.button_value} → id \"{nonZero.id}\", but no GameObject with that name exists in the scene.");
                    }
                    yield return new WaitForSeconds(delaytime);
                }

                yield return new WaitForSeconds(delaytime);


            }
            else
            {
                // Do command action (total, subtotal or non add)
                // Zero button_value → find by action (because the button_value is zero)
                if (TryGetZeroByAction(t.action, out var zeroByAction))
                {
                    Debug.Log($"Found zero-value by action → id={zeroByAction.id}, value={zeroByAction.button_value}, button_value={zeroByAction.button_value}");



                    // Play the boss audio for the selected action command
                    GameObject sfxNumberGo = GameObject.Find(zeroByAction.audio);
                    audioSource = sfxNumberGo.GetComponent<AudioSource>();
                    audioSource.Play();
                    yield return new WaitForSeconds(delaytime * 2);

                    var go = GameObject.Find(zeroByAction.id); // okay for setup; avoid every-frame usage
                    if (go != null)
                    {
                        if (zeroByAction.action == "addnumber")
                        {
                            // We are only adding a number, so just pull the handle
                            // Pause then pull the handle
                            yield return new WaitForSeconds(delaytime);
                            yield return StartCoroutine(PullTheHandle());

                            // Pause then rlease the handle
                            yield return new WaitForSeconds(delaytime);
                            yield return StartCoroutine(ReleaseTheHandle());
                        }
                        else
                        {
                            // We are doing an action command that is not "addnumber"
                            ButtonManager script = go.GetComponent<ButtonManager>();
                            script.PressButton();

                            // Find an SFX GameObject named "ButtonPress" in the scene and play the sound
                            GameObject sfxGo = GameObject.Find("ButtonPress");
                            audioSource = sfxGo.GetComponent<AudioSource>();
                            audioSource.Play();

                            if (zeroByAction.action == "total")
                            {
                                // Set this to true so we can trigger an extra audio line 
                                _is_total = true;

                                // Make boss walk to a random spot
                                //bossWalk.MoveToRandomPosition();

                            }
                        }
                        yield return new WaitForSeconds(delaytime);
                    }
                    else
                    {
                        Debug.LogWarning($"Record found for value {t.button_value} → id \"{zeroByAction.id}\", but no GameObject with that name exists in the scene.");
                    }
                }

                // Pause then pull the handle
                yield return new WaitForSeconds(delaytime);
                yield return StartCoroutine(PullTheHandle());

                // Pause then release the handle
                yield return new WaitForSeconds(delaytime);
                yield return StartCoroutine(ReleaseTheHandle());

                // Finished adding a number so move the boss to a new point and start a new number
                if (_is_total == true)
                {
                    // Make boss walk to a random spot
                    bossWalk.MoveToRandomPosition();

                    // Play the boss audio for the letsgoagain action command
                    GameObject sfxLetsGoAgain = GameObject.Find("letsgoagain");
                    audioSource = sfxLetsGoAgain.GetComponent<AudioSource>();
                    audioSource.Play();
                    yield return new WaitForSeconds(delaytime * 3);

                    _is_total = false;
                }

            }

            // Pause then move to the next button preess (if there is one)
            yield return new WaitForSeconds(delaytime);
        }
        
    }

    IEnumerator PullTheHandle()
    {
        bossWalk.TypistTyping();

        // Find a GameObject named "HandleRig" (the main handle) in the scene & pull the handle
        GameObject handleRig = GameObject.Find("HandleRig");

        HandleManager script = handleRig.GetComponent<HandleManager>();
        script.PullHandle();

        // Find a SFX GameObject named "HandlePull" in the scene & play the sound
        GameObject sfxGo = GameObject.Find("HandlePull");
        audioSource = sfxGo.GetComponent<AudioSource>();
        audioSource.Play();

        yield return null;

    }

    IEnumerator ReleaseTheHandle()
    {
        // Find a GameObject named "HandleRig" (the main handle) in the scene & release the handle
        GameObject handleRig = GameObject.Find("HandleRig");

        HandleManager script = handleRig.GetComponent<HandleManager>();
        script.ReleaseHandle();

        // Find a SFX GameObject named "HandlePull" in the scene & play the sound
        GameObject sfxGo = GameObject.Find("HandlePull");
        audioSource = sfxGo.GetComponent<AudioSource>();
        audioSource.Play();

        yield return null;

    }

}
