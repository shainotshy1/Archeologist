using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    public static float playerMovementDistance = 0f;
    public static bool collisionsEnabled;

    [SerializeField] float initalJumpHeight;
    [SerializeField] InputAction movement;
    [SerializeField] float horizontalSpeed;
    [SerializeField] bool _collisionsEnabled;
    [SerializeField] float gravity;

    enum Position
    {
        Left, Middle, Right,Idle
    }
    Vector3 movementDirection = new Vector3(1, 0, 0);
    bool isGrounded;
    float yDelta = 0f;
    float idleHeight;
    Position setPosition;
    Transform bodyTransform;
    Rigidbody rigidBody;
    private void Start()
    {
        setPosition = Position.Middle;
        isGrounded = true;

        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.gameObject.tag == "JumpCenter")
                {
                    bodyTransform = grandChild;
                    rigidBody = grandChild.GetComponent<Rigidbody>();
                }
            }
        }

        idleHeight = bodyTransform.position.y;
        rigidBody.useGravity = true;
    }
    private void OnEnable()
    {
        movement.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
    }
    private void CheckGroundedStatus()
    {
        if (PlayerCollisionHandler.playerGrounded)
        {
            isGrounded = true;
            yDelta = 0;
            rigidBody.useGravity = true;
        }
    }
    private void ProcessInput()
    {

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(PlayerJump());
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
                xPos = 0;
            }
            else if (movementDirection.z == 1 || movementDirection.z == -1)
            {
                zPos = 0;
            }

            MoveChild(xPos,zPos);
        }
        if (position == Position.Left)
        {
            MoveChild(- playerMovementDistance * movementDirection.z, playerMovementDistance * movementDirection.x);
        }
        else if (position == Position.Right)
        {
            MoveChild(playerMovementDistance * movementDirection.z, -playerMovementDistance * movementDirection.x);
        }
    }
    private void MoveChild(float newX,float newZ)
    {
        Vector3 endPosition = new Vector3(newX, bodyTransform.transform.localPosition.y, newZ);
        bodyTransform.transform.localPosition = Vector3.Lerp(bodyTransform.transform.localPosition, endPosition, Time.deltaTime * horizontalSpeed);
    }
    IEnumerator PlayerJump()
    {
        rigidBody.useGravity = false;
        isGrounded = false;
        yDelta = initalJumpHeight;

        while (!isGrounded)
        {
            Vector3 newPos = new Vector3(bodyTransform.position.x, bodyTransform.position.y + yDelta, bodyTransform.position.z);
            bodyTransform.position = newPos;

            if (!isGrounded && !rigidBody.useGravity&&bodyTransform.position.y>idleHeight)
            {
                yDelta -= gravity * Time.deltaTime;
            }
            else
            {
                yDelta = 0;
            }
            CheckGroundedStatus();
            yield return new WaitForEndOfFrame();
        }
    }
    void Update()
    {
        collisionsEnabled = _collisionsEnabled;
        ProcessInput();
        if (transform.localPosition.y < -10)
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);
        }
    }
}