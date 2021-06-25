using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Range(0, 10)] float movementSpeed;
    [SerializeField] [Range(0, 100)] float jumpForce;
    [SerializeField] float slowDownMovementSpeed;
    [SerializeField] InputAction movement;
    [SerializeField] InputAction jump;
    [SerializeField] InputAction crouch;

    Rigidbody rigidBody;
    float zMovement = 0f;
    float xMovement = 0f;
    bool isGrounded;
    private void OnEnable()
    {
        movement.Enable();
        jump.Enable();
        crouch.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        jump.Enable();
        crouch.Enable();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            isGrounded = false;
        }
    }
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        isGrounded = true;
    }

    void Update()
    {
        ProcessInput();
    }
    private void ProcessInput()
    {
        float xValue = movement.ReadValue<Vector2>().x;
        float zValue = movement.ReadValue<Vector2>().y;
        float jumpValue = (jump.ReadValue<float>() > 0.5 && isGrounded) ? 1 : 0;

        if (Mathf.Abs(xValue) > 0.5)
        {
            xValue /= Mathf.Abs(xValue);
            xMovement = xValue * Time.deltaTime * movementSpeed;
        }
        else if (Mathf.Abs(xMovement) > Mathf.Epsilon) { }
        {
            bool isPositive = xMovement > 0;
            xMovement -= xMovement * Time.deltaTime * slowDownMovementSpeed / Mathf.Abs(xMovement);
            if (xMovement > 0 != isPositive)
            {
                xMovement = 0f;
            }
        }
        if (Mathf.Abs(zValue) > 0.5)
        {
            zValue /= Mathf.Abs(zValue);
            zMovement = zValue * Time.deltaTime * movementSpeed;
        }
        else if (Mathf.Abs(zMovement) > Mathf.Epsilon)
        {
            zMovement -= zMovement * Time.deltaTime * slowDownMovementSpeed / Mathf.Abs(zMovement);
            zMovement = Mathf.Clamp(zMovement, 0, Mathf.Infinity);
        }

        float yForce = jumpValue * Time.deltaTime * jumpForce;

        Vector3 force = new Vector3(0, yForce, 0);

        if (isGrounded && Mathf.Abs(jumpValue) > Mathf.Epsilon) rigidBody.AddRelativeForce(force);

        if (Mathf.Abs(zMovement) > Mathf.Epsilon) transform.Translate(0, 0, zMovement);

        if (Mathf.Abs(xMovement) > Mathf.Epsilon) transform.Translate(xMovement, 0, 0);
    }
}
