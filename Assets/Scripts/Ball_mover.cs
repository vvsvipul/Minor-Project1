using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball_mover : MonoBehaviour
{
    [SerializeField] float initialSpeed=10f;
    [SerializeField] float speedIncrease=0.25f;
    [SerializeField] Text playerScore;
    [SerializeField] Text AIScore;

    int hitCounter;
    Rigidbody2D rb;

    [SerializeField] GameObject player;
    [SerializeField] GameObject ai;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("StartBall", 2f);
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity,initialSpeed);
    }
    private void StartBall()
    {
        rb.velocity = new Vector2(-1, 0) * (initialSpeed );
    }
    private void ResetBall()
    {
        rb.velocity = new Vector2(0, 0);
        transform.position = new Vector2(0, 0);
        hitCounter = 0;
        Invoke("StartBall", 2f);

    }
    private void PlayerBounce(Transform myObject)
    {
        hitCounter++;
        Vector2 ballPos = transform.position;
        Vector2 playerPos = myObject.position;

        float xDirection, yDirection;
        if (transform.position.x > 0)
        {
            xDirection = -1;
        }
        else
        {
            xDirection = 1;
        }
        yDirection = (ballPos.y - playerPos.y) / myObject.GetComponent<Collider2D>().bounds.size.y;
        if (yDirection == 0)
        {
            yDirection = 0.25f;
        }
        rb.velocity = new Vector2(xDirection,yDirection)*(initialSpeed);
    
    
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "AI" || collision.gameObject.name == "Player")
        {
            PlayerBounce(collision.transform);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.x>0)
        {
            ResetBall();
            playerScore.text = (int.Parse(playerScore.text) + 1).ToString();
        }
        else if(transform.position.x < 0)
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
