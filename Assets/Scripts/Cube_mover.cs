using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_mover : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]float speed=-1f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(speed, 0f, 0f);
    }

    void Update()
    {
        if (transform.position.x < -3)
        {
            transform.position = new Vector3(20, transform.position.y, transform.position.z);
        }

    }
}
