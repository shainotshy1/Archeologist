using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] float jumpForce;
    [SerializeField] InputAction movement;
    [SerializeField] InputAction jump;
    [SerializeField] float horizontalSpeed;
    [SerializeField] bool enableCollisions;

    public static float playerMovementDistance = 0f;
    public static bool collisionsEnabled;

    enum Position
    {
        Left, Middle, Right,Idle
    }
    Vector3 movementDirection = new Vector3(1, 0, 0);
    Vector3 futurePosition = new Vector3(0, 0, 0);
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
            //StartCoroutine(JumpRoutine(verticalValue));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (setPosition == Position.Middle)
            {
                futurePosition = new Vector3(0, 0, -1);
                setPosition = Position.Left;
            }
            else if (setPosition == Position.Right)
            {
                futurePosition = new Vector3(0, 0, 0);
                setPosition = Position.Middle;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (setPosition == Position.Middle)
            {
                futurePosition = new Vector3(0, 0, 1);
                setPosition = Position.Right;
            }
            if (setPosition == Position.Left)
            {
                futurePosition = new Vector3(0, 0, 0);
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
        foreach (Transform child in transform)
        {
            foreach(Transform grandChild in child)
            {
                if (grandChild.gameObject.tag == "Body")
                {
                    Vector3 endPosition = new Vector3(newX, grandChild.transform.localPosition.y, newZ);
                    grandChild.transform.localPosition = Vector3.Lerp(grandChild.transform.localPosition, endPosition, Time.deltaTime * horizontalSpeed);
                }
            }
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
