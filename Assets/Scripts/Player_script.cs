using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Analytics;
using Newtonsoft.Json;

public class Player_script : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] bool isAI;
    [SerializeField] GameObject ball;
    [SerializeField] int gameSpeed = 0;
    [SerializeField] bool isPerfect = false;
    
    string relativePath = "pingQ.json";

    int state = 0;

    float[,] Q_table=new float[3,3];

    private Rigidbody rb;
    private Vector3 playerMove;


    float expl_val = 1.0f;
    float exploration_decay = 0.05f;
    float learning_rate = 0.01f;
    float gamma = 0.5f;


    int action = 0;
    //bool shouldUpdate = false;

    float fp = 0;
    int episodes = 0;
  
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = gameSpeed;
        if (isPerfect)
        {
            expl_val = 0;
            LoadFromFile();
            
        }
    }

    void Update()
    {
        if (isAI)
        {
            AIControl();
        }
        else
        {
            PlayerControl();
            if (episodes == 240000)
            {
                SaveToFile();
            }
        }
    }
    private void PlayerControl()

    {
        Time.timeScale = gameSpeed;
        if ((ball.transform.position.x == 8) && (ball.GetComponent<Rigidbody>().velocity.z == 0))
            return;
        if (fp < 0.1f)
        {
            fp += Time.deltaTime;
            return;
        }
        fp = 0;
        int newState = State();
        
        Debug.Log("Episdes: " + episodes);

        float reward = Reward();
        Debug.Log("Reward: "+(reward));
        Q_table[state, action] = Q_table[state, action] * (1 - learning_rate) + learning_rate * (reward + gamma * Mathf.Max(Q_table[newState, 2], Mathf.Max(Q_table[newState, 0], Q_table[newState, 1])));
        episodes++;
        if (episodes % 100 == 0)
            expl_val -= exploration_decay;
        state = newState;
        float a = UnityEngine.Random.value;
        action = 0;

        Debug.Log("Value of a: " + a + "Value of expl: " + expl_val);

        if (a > expl_val)
        {
            if (Q_table[state, 0] >= Q_table[state,1])
            {
                if (Q_table[state, 0] >= Q_table[state, 2])
                {
                    action = 0;
                }
                else
                {
                    action = 2;
                }
            }
            else
            {
                if (Q_table[state, 1] >= Q_table[state, 2])
                {
                    action = 1;
                }
                else
                {
                    action = 2;
                }
            }
        }
        else
        {
            if (UnityEngine.Random.value > 0.33f)
            {
                action = 0;
            }
            else if(UnityEngine.Random.value > 0.66f)
            {
                action = 1;
            }
            else
            {
                action = 2;
            }
        }

        if (action == 0)
        {
            playerMove = new Vector3(0f,0f, 0f);
        }
        else if(action == 1)
        {
            playerMove = new Vector3(0f,0f ,1f);
        }
        else
        {
            playerMove = new Vector3(0f, 0f,-1f);
        }
        if(!isAI)
            rb.velocity = playerMove * movementSpeed;

    }
    private void AIControl()
    {
        if (ball.transform.position.z > transform.position.z + 0.5f)
        {
            playerMove = new Vector3(0, 0,1);
        }
        else if (ball.transform.position.z < transform.position.z - 0.5f)
        {
            playerMove = new Vector3(0, 0,-1);
        }
        else
        {
            playerMove = new Vector3(0, 0,0);
        }
    }
    private void FixedUpdate()
    {
        if(isAI)
            rb.velocity = playerMove * movementSpeed;
    }
    int State()
    {
        if (Math.Abs(transform.position.z - ball.transform.position.z)<1f)
        {
            return 0;
        }
        else if (transform.position.z > ball.transform.position.z)
        {
            return 1;
        }
        else {
            return 2;
        }
    }
    private float Reward()
    {
        //Debug.Log("Reward: " + (5 - Math.Abs(ball.transform.position.z - transform.position.z)));
        //if (Math.Abs(ball.transform.position.z - transform.position.z) > 1)
            return (7 - Math.Abs(ball.transform.position.z - transform.position.z));
        //else
            //return 3;
    }
    private void SaveToFile()
    {
        string path = "C:\\Users\\Vipul\\Minor1\\Assets\\Scripts\\" + relativePath;
        if (File.Exists(path))
        {
            try
            {
                Debug.Log("Data exists. Deleting old file and writing a new one: " + path);
                File.Delete(path);
                using FileStream stream = File.Create(path);
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Q_table));
                stream.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Error" + e);
            }

        }
        else
        {
            try
            {
                Debug.Log("Data exists. Deleting old file and writing a new one");
                using FileStream stream = File.Create(path);
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Q_table));
                stream.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Error" + e);
            }
        }
    }
    private void LoadFromFile()
    {
        string path = "C:\\Users\\Vipul\\Minor1\\Assets\\Scripts\\" + relativePath;
        Q_table = JsonConvert.DeserializeObject<float[,]>(File.ReadAllText(path));
        expl_val = 0;
    }
}
