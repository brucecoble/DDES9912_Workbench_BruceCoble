using UnityEngine;

public class Week2_Spin : MonoBehaviour
{
    public float ySpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, ySpeed * Time.deltaTime, 0);
    }
}
