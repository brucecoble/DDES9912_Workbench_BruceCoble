using UnityEngine;
using UnityEngine.AI;


public class ButtonPusher : MonoBehaviour
{
    public InteractableGeneral subject;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        subject = other.GetComponent<InteractableGeneral>();

        if (subject != null)
        {
            subject.onPrimaryInteract.Invoke();
        }
    }
}
