using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject man1;
    [SerializeField] GameObject man2;
    [SerializeField] GameObject box;
    [SerializeField] int gameSpeed;
    [SerializeField] Text disp;

    float[,] states = { { -1.6f, 0.7f, -3f }, { 0f, 0.7f, -3f }, { 1.6f, 0.7f, -3f }, { 3f, 0.7f, -1.6f }, { 3f, 0.7f, 0f }, { 3f, 0.7f, 1.6f }, { 1.6f, 0.7f, 3f }, { 0f, 0.7f, 3f }, { -1.6f, 0.7f, 3f }, { -3f, 0.7f, 1.6f }, { -3f, 0.7f, 0f }, { -3f, 0.7f, -1.6f } };

    Rigidbody rb1;
    Rigidbody rb2;
    float force = 300f;

    float[] Q_Table1 = new float[12];
    float[,] Q_Table2 = new float[12, 12];

    float[] forceVector1 = new float[3];
    float[] forceVector2 = new float[3];
    float expl_val = 1.0f;
    float exploration_decay = 0.005f;
    float learning_rate = 0.1f;
    float gamma = 0.8f;

    public int action = 0;
    public int action2 = 0;

    float fp = 0;
    float fp2 = 0;
    int episodes = 0;
    int wins = 0;
    bool firstIteration = true;
    float fpsu = 0f;
    public bool takeDecision = false;

    Vector3 initialPositionMan1;
    Vector3 initialPositionMan2;

    void Start()
    {
        Time.timeScale = gameSpeed;
        initialPositionMan1 = man1.transform.position;
        initialPositionMan2 = man2.transform.position;
        rb1 = man1.GetComponent<Rigidbody>();
        rb2 = man2.GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        if (box.transform.position.z >= 4.5f)
        {
            wins++;
            Q_Table1[action] += 10f;
            Q_Table2[action,action2] += 10f;
            Debug.Log("Reward1");
            goto a;

        }


        if (fp2 >= 0.05f)
        {
            rb1.velocity = new Vector3(0f, 0f, 0f);
            rb2.velocity = new Vector3(0f, 0f, 0f);
            rb1.AddForce(forceVector1[0], forceVector1[1], forceVector1[2], ForceMode.Force);
            rb2.AddForce(forceVector2[0], forceVector2[1], forceVector2[2], ForceMode.Force);
            fp2= 0;
        }
        else
        {
            fp2 += Time.deltaTime;
        }


        if (fp <= fpsu)
        {
            fp += Time.deltaTime;
            return;
        }

    a:
        
        fpsu = 3f;
        fp = 0;
        box.transform.position = new Vector3(0f, 1f, 0f);
        box.transform.eulerAngles = new Vector3(0f, 0f, 0f);

        firstIteration = false;


        //Man1

        man1.transform.position = initialPositionMan1;
        float a = UnityEngine.Random.value;
        action = 0;
        if (a > expl_val)
        {
            action = Array.IndexOf(Q_Table1, Q_Table1.Max());
        }
        else
        {
            action = ((int)(UnityEngine.Random.value * 1000000)) % 12;
        }

        if (action < 3)
        {
            man1.transform.position = new Vector3(states[action,0], states[action, 1], states[action, 2]);
            forceVector1[0] = 0f;
            forceVector1[1] = 0f;
            forceVector1[2] = force;
        }
        else if (action < 6)
        {
            man1.transform.position = new Vector3(states[action, 0], states[action, 1], states[action, 2]);
            forceVector1[0] = -force;
            forceVector1[1] = 0f;
            forceVector1[2] = 0f;
        }
        else if (action < 9)
        {
            man1.transform.position = new Vector3(states[action, 0], states[action, 1], states[action, 2]);
            forceVector1[0] = 0f;
            forceVector1[1] = 0f;
            forceVector1[2] = -force;
        }
        else
        {
            forceVector1[0] = force;
            forceVector1[1] = 0f;
            forceVector1[2] = 0f;
        }


        //Man2

        man2.transform.position = initialPositionMan2;
        a = UnityEngine.Random.value;
        if (a > expl_val)
        {
            action2 = RowMax(Q_Table2, action);
        }
        else
        {
            action2 = ((int)(UnityEngine.Random.value * 1000000)) % 12;
        }

        if (action2 < 3)
        {
            man2.transform.position = new Vector3(states[action2, 0], states[action2, 1], states[action2, 2]);
            forceVector2[0] = 0f;
            forceVector2[1] = 0f;
            forceVector2[2] = force;
        }
        else if (action2 < 6)
        {
            man2.transform.position = new Vector3(states[action2, 0], states[action2, 1], states[action2, 2]);
            forceVector2[0] = -force;
            forceVector2[1] = 0f;
            forceVector2[2] = 0f;
        }
        else if (action2 < 9)
        {
            man2.transform.position = new Vector3(states[action2, 0], states[action2, 1], states[action2, 2]);
            forceVector2[0] = 0f;
            forceVector2[1] = 0f;
            forceVector2[2] = -force;
        }
        else
        {
            forceVector2[0] = force;
            forceVector2[1] = 0f;
            forceVector2[2] = 0f;
        }

        episodes++;
        disp.text = "Episode: " + episodes+"\n"+"Wins: "+wins;
        expl_val -= exploration_decay;
    }

    int RowMax(float[,] arr, int x)
    {
        int maxI = 0;
        for (int i = 0; i < 12; i++)
        {
            if (arr[x, i] > arr[x, maxI])
            {
                maxI = i;
            }
        }
        return maxI;
    }


}
