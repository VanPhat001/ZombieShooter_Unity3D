using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public float GetBestScore()
    {
        return PlayerPrefs.GetFloat("BestScore", 0);
    }

    public float UpdateBestScore(float score)
    {
        float currentBestScore = GetBestScore();
        if (score > currentBestScore)
        {
            PlayerPrefs.SetFloat("BestScore", score);
            currentBestScore = score;
        }

        return currentBestScore;
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
