using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour
{
    [SerializeField] float jumpForce;
    [SerializeField] InputAction movement;
    [SerializeField] InputAction jump;
    [SerializeField] float horizontalSpeed;
    [SerializeField] bool enableCollisions;

    public static float playerMovementDistance;
    public static bool collisionsEnabled;

    enum Position
    {
        Left, Middle, Right
    }
    Vector3 movementDirection = new Vector3(0, 0, 1);
    bool isGrounded;
    Position setPosition;
    Rigidbody rigidBody;
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        setPosition = Position.Middle;
    }
    private void OnEnable()
    {
        movement.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        jump.Disable();
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
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
            float xPos = 0f;
            float zPos = 0f;
            if (movementDirection.x == 1 || movementDirection.x == -1)
            {
                xPos = transform.localPosition.x;
            }
            else if (movementDirection.z == 1 || movementDirection.z == -1)
            {
                zPos = transform.localPosition.z;
            }

            Vector3 endPosition = new Vector3(xPos, transform.localPosition.y, zPos);
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, Time.deltaTime * horizontalSpeed);
        }
        if (position == Position.Left)
        {
            Vector3 endPosition = new Vector3(transform.localPosition.x * movementDirection.x - playerMovementDistance * movementDirection.z, transform.localPosition.y, transform.localPosition.z * movementDirection.z + playerMovementDistance * movementDirection.x);
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, Time.deltaTime * horizontalSpeed);
        }
        else if (position == Position.Right)
        {
            Vector3 endPosition = new Vector3(transform.localPosition.x * movementDirection.x + playerMovementDistance * movementDirection.z, transform.localPosition.y, transform.localPosition.z * movementDirection.z - playerMovementDistance * movementDirection.x);
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, Time.deltaTime * horizontalSpeed);
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

    // Update is called once per frame
    void Update()
    {
        collisionsEnabled = enableCollisions;
        ProcessInput();
        if (transform.localPosition.y < -10)
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }
    }
}
