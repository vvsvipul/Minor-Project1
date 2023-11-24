using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ManScript : MonoBehaviour
{
    [SerializeField] GameObject box;
    [SerializeField] float gameSpeed=0f;

    Rigidbody rb;
    float force =60f;
    
    float[] Q_Table = new float[12];


    float[] forceVector = new float[3];
    float expl_val = 1.0f;
    float exploration_decay = 0.05f;
    float learning_rate = 0.1f;
    float gamma = 0.8f;

    public int action = 0;

    float fp = 0;
    int episodes = 0;
    bool firstIteration = true;
    float fpsu = 0f;
    public bool takeDecision = false;

    Vector3 initialPosition;
    void Start()
    {
        Time.timeScale = gameSpeed;
        //GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 5f);
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
       
    }
    void Update()
    {
        if (box.transform.position.z >= 4.5f)
        {
            Q_Table[action] += 10f;
            Debug.Log("Reward1");
            goto a;

        }
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.AddForce(forceVector[0], forceVector[1], forceVector[2], ForceMode.Force);
        if (fp <= fpsu)
        {
            takeDecision = true;
            fp += Time.deltaTime;
            return;
        }

        takeDecision = false;
        a:

        fpsu = 10f;
        fp = 0;
        box.transform.position = new Vector3(0f, 1f, 0f);
        box.transform.eulerAngles = new Vector3(0f,0f,0f); 

        firstIteration = false;
        transform.position = initialPosition;

        
        float a = UnityEngine.Random.value;
        action = 0;
        if (a > expl_val)
        {
            //Debug.Log("a>expl_val");
            action = Array.IndexOf(Q_Table, Q_Table.Max());
        }
        else
        {
            //Debug.Log("a<expl_val");
            action = ((int)(UnityEngine.Random.value*1000000))%12;
        }

        if(action < 3)
        {
            //Debug.Log("action<3");
            forceVector[0] = 0f;
            forceVector[1] = 0f;
            forceVector[2] = force;
        }
        else if (action<6)
        {
            //Debug.Log("action<6");
            forceVector[0] = -force;
            forceVector[1] = 0f;
            forceVector[2] = 0f;
        }
        else if(action<9){
            //Debug.Log("action<9");
            forceVector[0] = 0f;
            forceVector[1] = 0f;
            forceVector[2] = -force;
        }
        else
        {
            //Debug.Log("action<12");
            forceVector[0] = force;
            forceVector[1] = 0f;
            forceVector[2] = 0f;
        }
        expl_val -= exploration_decay;
    }


}
