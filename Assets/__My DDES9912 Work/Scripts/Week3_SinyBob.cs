using UnityEngine;

public class Week3_SinyBob : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 sinOffset;
    public float alpha;
    public float sinValue;
    public float rangeFactor;
    public float bobSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //sinValue = Mathf.Sin(alpha); // Continuously returning a value between -1 & 1 geogebra.org
        
        // Mathf takes rads so if people are entering degrees, then convert this:
        sinValue = Mathf.Sin(alpha * Mathf.Deg2Rad); // Continuously returning a value between -1 & 1 geogebra.org

        // Cannot directly do transform.position.y = sinValue - Unity security prevents that, thus need to use a Vector3 value
        sinOffset.y = sinValue * rangeFactor;

        // This will place the object in the wrong spot - the global position - we want the local position
        // transform.position = sinOffset; 
        transform.localPosition = startPosition + sinOffset;

        alpha += bobSpeed * Time.deltaTime;

    }
}
