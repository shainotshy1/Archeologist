using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] InputAction movement;
    [SerializeField] InputAction jump;

    Rigidbody rigidBody;
    bool isGrounded;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    private void OnEnable()
    {
        movement.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        movement.Enable();
        jump.Enable();
    }
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        float horizontalValue = movement.ReadValue<float>();
        float verticalValue = (jump.ReadValue<float>() > 0.5) ? jumpForce : 0;

        float xDelta = horizontalValue / Mathf.Abs(horizontalValue)*Time.deltaTime*movementSpeed;
        if (Mathf.Abs(xDelta) > Mathf.Epsilon)
        {
            transform.Translate(xDelta, 0, 0);
        }

        if (Mathf.Abs(verticalValue) > Mathf.Epsilon&&isGrounded)
        {
            StartCoroutine(JumpRoutine(verticalValue));
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
