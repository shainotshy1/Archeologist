using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [HideInInspector] public bool playerGrounded = true;
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
}
