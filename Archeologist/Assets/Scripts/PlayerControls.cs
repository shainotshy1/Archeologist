using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] float jumpForce;
    [SerializeField] InputAction jump;
    [SerializeField] float movementDistance;
    [SerializeField] float horizontalSpeed;

    enum Position
    {
        Left,Middle,Right
    }

    bool isGrounded;
    Position setPosition;
    Rigidbody rigidBody;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    private void OnEnable()
    {
        jump.Enable();
    }
    private void OnDisable()
    {
        jump.Disable();
    }
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        setPosition = Position.Middle;
    }
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        float verticalValue = (jump.ReadValue<float>() > 0.5) ? jumpForce : 0;

        if (Mathf.Abs(verticalValue) > Mathf.Epsilon && isGrounded)
        {
            StartCoroutine(JumpRoutine(verticalValue));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (setPosition == Position.Middle)
            {
                setPosition = Position.Left;
            }
            else if (setPosition == Position.Right)
            {
                setPosition = Position.Middle;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (setPosition == Position.Middle)
            {
                setPosition = Position.Right;
            }
            if (setPosition == Position.Left)
            {
                setPosition = Position.Middle;
            }
        }

        GoToPosition(setPosition);
    }
    private void GoToPosition(Position position)
    {
        if (position == Position.Middle)
        {
            Vector3 endPosition = new Vector3(0, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * horizontalSpeed);
        }
        if(position == Position.Left)
        {
            Vector3 endPosition = new Vector3(-movementDistance, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * horizontalSpeed);
        }
        else if(position == Position.Right)
        {
            Vector3 endPosition = new Vector3(movementDistance, transform.position.y,transform.position.z);
            transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * horizontalSpeed);
        }
    }
    IEnumerator JumpRoutine(float verticalValue)
    {
        rigidBody.ResetInertiaTensor();
        rigidBody.ResetCenterOfMass();
        rigidBody.AddRelativeForce(0, verticalValue * Time.deltaTime, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        isGrounded = false;
    }
}
