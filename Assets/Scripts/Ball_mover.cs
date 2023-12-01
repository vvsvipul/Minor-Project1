using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball_mover : MonoBehaviour
{
    [SerializeField] float initialSpeed=10f;
    [SerializeField] Text playerScore;
    [SerializeField] Text AIScore;
    
    Rigidbody rb;
    
    [SerializeField] GameObject player;
    [SerializeField] GameObject ai;


    float xDirection = 0, yDirection = 0f, zDirection = -2;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("StartBall", 2f);
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity,initialSpeed);
    }
    private void StartBall()
    {
        rb.velocity = new Vector3(-1f,0f,0f) * (initialSpeed);
    }
    private void ResetBall()
    {
        rb.velocity = new Vector3(0f,0f, 0f);
        transform.position = new Vector3(8f,0.4f, 0f);
        Invoke("StartBall", 2f);

    }
    private void PlayerBounce(Transform myObject)
    {
        Vector3 ballPos = transform.position;
        Vector3 playerPos = myObject.position;


        if (transform.position.x > 8)
        {
            xDirection = -1;
        }
        else
        {
            xDirection = 1;
        }
        zDirection = (ballPos.z - playerPos.z)*1.5f/ myObject.GetComponent<Collider>().bounds.size.z;
        if (zDirection == 0)
        {
            zDirection = 0.25f;
        }
        rb.velocity = new Vector3(xDirection,yDirection,zDirection)*(initialSpeed);
    
    
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "AI" || collision.gameObject.name == "Player")
        {
            Debug.Log("Collsion occured with " + collision.gameObject.name);
            PlayerBounce(collision.transform);
        }
        else if(collision.gameObject.name == "Top Bar" || collision.gameObject.name == "Bottom Bar")
        {
            rb.velocity= new Vector3(xDirection, yDirection, -zDirection) * (initialSpeed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (transform.position.x > 8)
        {
            ResetBall();
            playerScore.text = (int.Parse(playerScore.text) + 1).ToString();
        }
        else if (transform.position.x < 0)
        {
            ResetBall();
            AIScore.text = (int.Parse(AIScore.text) + 1).ToString();
        }

        if (int.Parse(AIScore.text) >= 11 || int.Parse(playerScore.text) >= 11)
        {
            AIScore.text = (0).ToString();
            playerScore.text = (0).ToString();
        }
    }
}
