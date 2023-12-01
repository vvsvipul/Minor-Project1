using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class capS : MonoBehaviour
{
    float[,] Q_table=new float[220,2];
    Rigidbody rb;
    [SerializeField] float speed=0f;
    [SerializeField] TextMeshProUGUI disp;
    [SerializeField] int gameSpeed = 0;

    float expl_val = 1.0f;
    float exploration_decay = 0.005f;
    float learning_rate = 0.005f;
    float gamma = 0.3f;

    bool should_update = false;
    int action = 0;
    int x = 0;

    int score = 0;
    int bestScore = 0;
    int episodes = 0;
    float average_score = 0f;
    int total_score = 0;

    Vector3 playerPos;

    float pos = 0f;
    float fp = 0f;
    int EnvState(float x)
    {
        return (int)x*10+110;
    }
    void Start()
    {
        Time.timeScale = gameSpeed;
        rb = GetComponent<Rigidbody>();
        playerPos = transform.position;
    }

    void Update()
    {
        if (fp > 0.1f)
        {
            fp = 0;
        }
        else
        {
            fp = fp + Time.deltaTime;
            return;
        }
        if (should_update)
        {
            score++;
            if (score >= 100)
            {
                ResetCapsule();
            }
            if (transform.position.x <= -10f)
            {
                Q_table[x, action] = Q_table[x, action] * (1 - learning_rate) + learning_rate * (-100 );
                ResetCapsule();
            }
            Q_table[x, action] = Q_table[x, action] * (1 - learning_rate) + learning_rate * (-2 + gamma * Math.Max(Q_table[EnvState(transform.position.x), 0], Q_table[EnvState(transform.position.x), 1]));
        }
        float a = UnityEngine.Random.value;
        action = 0;
        x = EnvState(pos);
        pos = transform.position.x;

        Debug.Log("Value of a: " + a + "Value of expl: " + expl_val);
        if (a > expl_val)
        {
            Debug.Log("value of x is : " + x + "\n");
            if (Q_table[x, 0] >= Q_table[x, 1])
            {
                action = 0;
            }
            else
            {
                action = 1;
            }
        }
        else
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                action = 1;
            }
            else
            {
                action = 0;
            }
        }
        if (action == 1)
        {
            rb.velocity = new Vector3(speed,0f,0f);
        }
        else
        {
            rb.velocity = new Vector3(-speed, 0f, 0f);
        }
        disp.text = "Score: " + score + "\nEpisodes: " + episodes;
        should_update = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        Q_table[x, action] = Q_table[x, action] * (1 - learning_rate) + learning_rate * (+100 );
        ResetCapsule();
        total_score += score;
        average_score = total_score / episodes;
        should_update = false;
    }
    private void ResetCapsule()
    {
        expl_val -= exploration_decay;
        expl_val = Math.Max(0f, expl_val);
        episodes++;
        score = 0;
        transform.position = playerPos;
    }
}
