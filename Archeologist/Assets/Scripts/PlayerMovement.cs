using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField][Range(0,100)] float movementSpeed;
    [SerializeField] [Range(0,100)] float jumpForce;

    Rigidbody rigidBody;
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void ProcessInput()
    {

    }

    private void Update()
    {
        ProcessInput();
    }
}
