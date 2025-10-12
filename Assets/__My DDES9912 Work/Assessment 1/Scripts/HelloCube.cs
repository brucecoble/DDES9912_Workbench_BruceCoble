using UnityEngine;

public class HelloCube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hello cube");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("The update log");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bang Bang");
    }
}
