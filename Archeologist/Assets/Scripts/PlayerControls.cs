using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    public static float playerMovementDistance = 0f;

    [SerializeField] float jumpInitialVelocity;
    [SerializeField] float horizontalSpeed;
    [SerializeField] InputAction movement;

    enum Position
    {
        Left, Middle, Right, Idle
    }
    bool isGrounded;
    Vector3 movementDirection = new Vector3(1, 0, 0);
    Position setPosition;
    Transform bodyTransform;
    Rigidbody rigidBody;
    Canvas canvas;
    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("GameOver").GetComponent<Canvas>();
        canvas.enabled = false;
    }
    private void Start()
    {
        setPosition = Position.Middle;

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
    private void ProcessInput()
    {

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rigidBody.velocity = Vector3.up*jumpInitialVelocity;
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
    public IEnumerator PlayerDeath()
    {
        yield return new WaitForEndOfFrame();//temporary till end sequence is added
        PathHandler.pathRunning = false;
        canvas.enabled = true;
        rigidBody.velocity = Vector3.zero;
        rigidBody.useGravity = false;
        rigidBody.gameObject.GetComponent<SphereCollider>().enabled = false;
    }
    void Update()
    {
        isGrounded = rigidBody.GetComponent<PlayerCollisionHandler>().playerGrounded;
        if (transform.localPosition.y < -10)
        {
            StartCoroutine(PlayerDeath());
        }

        ProcessInput();
    }
}