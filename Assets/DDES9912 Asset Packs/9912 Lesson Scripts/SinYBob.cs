using UnityEngine;

public class SinYBob : MonoBehaviour
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
        sinValue = Mathf.Sin(alpha * Mathf.Deg2Rad);

        sinOffset.y = sinValue * rangeFactor;

        transform.localPosition = startPosition + sinOffset;

        alpha += bobSpeed * Time.deltaTime;
    }
}
