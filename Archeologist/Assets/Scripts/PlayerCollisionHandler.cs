using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCollisionHandler : MonoBehaviour
{
    [HideInInspector] public bool playerGrounded = true;

    ScoreHandler scoreHandler;
    private void Start()
    {
        scoreHandler = new ScoreHandler();
    }
    private void OnCollisionStay(Collision collision)
    {
        playerGrounded = collision.gameObject.tag == "Ground";
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            playerGrounded = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            PlayerControls playerControls = FindObjectOfType<PlayerControls>();
            StartCoroutine(playerControls.ReloadScene());
            scoreHandler.ResetScore();
        }
    }
    private void Update()
    {
        scoreHandler.DisplayScore();
    }
}
