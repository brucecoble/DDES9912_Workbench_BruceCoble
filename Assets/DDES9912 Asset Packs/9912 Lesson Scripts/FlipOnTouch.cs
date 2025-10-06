using UnityEngine;

public class FlipOnTouch : MonoBehaviour
{
    public Rigidbody flipper;
    public float spinForce;
    public float pushForce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject + " collided with: " + collision.gameObject.name);
        flipper.AddTorque(0, 0, spinForce);

        //adds an extra boost to the colliding object when it hits
        Rigidbody otherRbody = collision.rigidbody;
        if(otherRbody != null )
            otherRbody.AddForce(collision.rigidbody.linearVelocity.normalized * pushForce);
    }
}
