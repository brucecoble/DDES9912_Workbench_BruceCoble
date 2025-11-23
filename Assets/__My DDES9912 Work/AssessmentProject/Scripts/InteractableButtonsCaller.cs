using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Audio;

public class InteractableButtonsCaller : MonoBehaviour
{
    public List<GameObject> buttonsToPress; // List of all the buttons we want to press
    private InteractableGeneral buttonPress; // The interactible general script of the object in our list
    private AudioSource audioSource; // For the sound effects
    public float delaytime = 0.5f; // Set the delay time to use between actions

    private IEnumerator Start()
    {
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
