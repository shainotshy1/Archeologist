using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    Canvas pauseCanvas;
    Canvas gameOverCanvas;
    private void Awake()
    {
        pauseCanvas = GameObject.FindGameObjectWithTag("Pause").GetComponent<Canvas>();
        gameOverCanvas = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Canvas>();
        pauseCanvas.enabled = false;
    }
    public void Pause()
    {
        if (!gameOverCanvas.enabled)
        {
            pauseCanvas.enabled = true;
            PathHandler.pathRunning = false;
        }
    }
    public void Resume()
    {
        pauseCanvas.enabled = false;
        PathHandler.pathRunning = true;
    }
    public void LoadMainMenu()
    {

    }
    public void Replay()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
