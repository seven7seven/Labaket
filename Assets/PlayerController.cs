using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;
    private Oscillator osc;

    private float prevVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        osc = GetComponent<Oscillator>();

        osc.PlayScale("C", new int[] { 2, 1, 2, 2, 1, 2, 2 });
    }

    private float timer = 0.0f; // begins at this value
    private float timerMax = 0.1f; // event occurs at this value

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        Debug.Log(prevVelocity);

        // If velocity increases
        if (prevVelocity < rb.velocity.magnitude - 0.01f)
        {
            osc.PlayNote();
            timer = 0.0f;
        }

        if (timer >= timerMax)
        {
            osc.Stop();
        }

        prevVelocity = rb.velocity.magnitude;

        rb.AddForce(movement * speed);
    }
}