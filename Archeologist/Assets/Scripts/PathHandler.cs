using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PathHandler : MonoBehaviour
{
    [SerializeField] InputAction movement;
    [SerializeField] GameObject newPath;
    [SerializeField] GameObject startPath;
    [SerializeField] float pathSeperatorDistance;
    [SerializeField] float pathSeperatorHeight;
    [SerializeField] int pathsInfrontAmount;
    [SerializeField] float jumpForce;
    [SerializeField] InputAction jump;
    [SerializeField] float movementDistance;
    [SerializeField] float horizontalSpeed;

    enum Position
    {
        Left, Middle, Right
    }
    Vector3 movementDirection = new Vector3(0, 0, 1);
    bool isGrounded;
    Position setPosition;
    Rigidbody rigidBody;
    Queue<GameObject> platforms = new Queue<GameObject>();
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
        movement.Disable();
        jump.Disable();
    }
    private void Awake()
    {
        transform.rotation = Quaternion.identity;
    }
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        setPosition = Position.Middle;

        Vector3 newPosition = new Vector3(0, 0, -7.5f);
        platforms.Enqueue(Instantiate(startPath, newPosition, Quaternion.identity));

        for (int i = 1; i <= pathsInfrontAmount*2; i++)
        {
            newPosition = new Vector3(0, 0, pathSeperatorDistance*i);
            platforms.Enqueue(Instantiate(newPath, newPosition, Quaternion.identity));
        }
    }
    private void Update()
    {
        ProcessInput();
        CreatePath();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    private void CreatePath()
    {
        if (platforms.Peek().transform.position.z < transform.position.z - 4*pathSeperatorDistance)
        {
            Vector3 newPosition = new Vector3(0, 0, pathSeperatorDistance * pathsInfrontAmount);

            platforms.Dequeue().GetComponent<PlatformHandler>().RemovePlatform();
            platforms.Enqueue(Instantiate(newPath,newPosition,Quaternion.identity));
        }
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
            if(movementDirection.x == 1||movementDirection.x == -1)
            {
                xPos = transform.position.x;
            }
            else if(movementDirection.z == 1||movementDirection.z == -1)
            {
                zPos = transform.position.z;
            }

            Vector3 endPosition = new Vector3(xPos, transform.position.y, zPos);
            transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * horizontalSpeed);
        }
        if (position == Position.Left)
        {
            Vector3 endPosition = new Vector3(transform.position.x*movementDirection.x-movementDistance*movementDirection.z, transform.position.y, transform.position.z*movementDirection.z+movementDistance*movementDirection.x);
            transform.position = Vector3.Lerp(transform.position, endPosition, Time.deltaTime * horizontalSpeed);
        }
        else if (position == Position.Right)
        {
            Vector3 endPosition = new Vector3(transform.position.x*movementDirection.x+movementDistance*movementDirection.z, transform.position.y, transform.position.z*movementDirection.z-movementDistance*movementDirection.x);
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
