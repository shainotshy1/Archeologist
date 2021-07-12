using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandler
{
    static float score = 0;

    TextMeshProUGUI scoreBoard = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
    public void ChangeScore(int delta)
    {
        score += delta/2.0f;
    }
    public void ResetScore()
    {
        Debug.Log($"Previous Score: {score}");
        score = 0;
    }
    public void DisplayScore()
    {
        if (scoreBoard != null)
        {
            scoreBoard.text = $"Score: {score}";
        }
        else
        {
            Debug.Log("No Score Board");
        }
    }
}
