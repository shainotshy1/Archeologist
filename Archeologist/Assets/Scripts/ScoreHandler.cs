using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandler
{
    static float score;

    TextMeshProUGUI scoreBoard = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
    public ScoreHandler(int _score)
    {
        score = _score;
    }
    public ScoreHandler()
    {

    }
    public void ChangeScore(int delta)
    {
        score += delta;
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
