using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void BoxJump()
    {
        SceneManager.LoadScene("bxj");
    }
    public void BoxPush()
    {
        SceneManager.LoadScene("bxp");
    }
    public void MainM()
    {
        SceneManager.LoadScene("a");
    }
    public void Apple()
    {
        SceneManager.LoadScene("bxf");
    }
    public void Ping()
    {
        SceneManager.LoadScene("ping");
    }
}
