using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_script : MonoBehaviour
{
    [SerializeField] GameObject ball;
    [SerializeField] float speed = 5f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.transform.position.y > transform.position.y)
        {
            rb.velocity = new Vector3(0f,0f, speed);
        }
        else
        {
            rb.velocity = new Vector3(0f,0f, -speed);
        }
    }
}
