using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class Capsule_mover : MonoBehaviour
{
    [SerializeField] float game_speed = 0f;
    [SerializeField] Text disp_text;
    Vector3[] cube_pos = { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
    Vector3 player_pos = new Vector3(0, 0, 0);
    Rigidbody rb;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] GameObject[] cubes;
    public static Capsule_mover Instance;
    float[,] Q_table = { { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f } , { 1.0f, 1.0f } , { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f }, { 1.0f, 1.0f } };
    [SerializeField] int saveThreshold =250;

    [SerializeField]bool perfectAI = false;
    float fp = 0;

    float expl_val = 1.0f;
    float exploration_decay = 0.005f;
    float learning_rate = 0.001f;
    float gamma = 0.3f;

    bool should_update = false;
    int action = 0;
    int x = 0;

    int score = 0;
    int max_score = 0;
    int episodes = 0;
    float average_score = 0f;
    int total_score = 0;

    string relativePath = "abc.json";
    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        Time.timeScale = game_speed;
        rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(0, jumpForce, 0);
        player_pos = transform.position;
        for (int i = 0; i < cubes.Length; i++)
        {
            cube_pos[i] = cubes[i].gameObject.transform.position;
        }
        if (perfectAI)
        {
            string path = "C:\\Users\\Vipul\\MARL\\Assets\\Scripts\\" + relativePath;
            Q_table = JsonConvert.DeserializeObject<float[,]>(File.ReadAllText(path));
            expl_val = 0;
        }

    }
    int Env_state()
    {
        float dist = 1000f;
        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i].gameObject.transform.position.x > 0)
                dist = Math.Min(dist, Vector3.Distance(transform.position, cubes[i].gameObject.transform.position));
        }
        return 1 + (int)dist * 10 / 4;
    }
    void Update()
    {
        if (rb.velocity.y != 0f)
            return;
        if (fp > 0.075f)
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
            Q_table[x, action] = Q_table[x, action] * (1 - learning_rate) + learning_rate * ((action*110 -10) * gamma * Math.Max(Q_table[x - 1, 0], Q_table[x - 1, 1]));
        }
        float a = UnityEngine.Random.value;
        action = 0;
        x = Env_state();
        Debug.Log("Value of a: "+a+"Value of expl: "+expl_val);
        if (a > expl_val)
        {
            Debug.Log("value of x is : " + x+"\n");
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
            //Debug.Log("Hello");
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
            Jump();
        }
        max_score = Math.Max(score, max_score);
        //Debug.Log("Current score: " + score + " Max Score: " + max_score);
        //Debug.Log("Average Score: " + average_score);
        should_update = true;
        disp_text.text = "Score: " + score + "\n"+"Max Score: " + max_score + "\n" + "Episodes: "+episodes;

    }
    private void OnTriggerEnter(Collider other)
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Q_table[x, action] = Q_table[x, action] * (1 - learning_rate) + learning_rate * (-100 + gamma * Math.Max(Q_table[x - 1, 0], Q_table[x - 1, 1]));
        Reset_capsule();
        total_score += score;
        average_score = total_score / episodes;
        score = 0;
        should_update = false;
        if (episodes == saveThreshold)
        {
            SaveToFile();
        }
    }
    private void Reset_capsule()
    {
        expl_val -= exploration_decay;
        expl_val = Math.Max(0f,expl_val);
        episodes++;
        transform.position = player_pos;
        for(int i = 0; i < cubes.Length; i++)
        {
            cubes[i].gameObject.transform.position = cube_pos[i];
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(0, jumpForce, 0);
    }

    private void SaveToFile()
    {
        string path = "C:\\Users\\Vipul\\MARL\\Assets\\Scripts\\" + relativePath;
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
                Debug.Log("Error"+e);
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
                Debug.Log("Error"+e);
            }
        }
    }
}
