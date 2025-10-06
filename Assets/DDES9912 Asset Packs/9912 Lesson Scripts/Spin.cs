using UnityEngine;

public class Spin : MonoBehaviour
{
    public float ySpeed;
    public float yAcceleration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, ySpeed * Time.deltaTime, 0);

        ySpeed += yAcceleration * Time.deltaTime;
    }

    public void Stop()
    {
        ySpeed = 0;
        yAcceleration = 0;
    }

    public void SetSpeed(float newSpeed)
    {
        ySpeed = newSpeed;
    }

    public void SetAcceleration(float newAcceleration)
    {
        yAcceleration = newAcceleration;   
    }
}
