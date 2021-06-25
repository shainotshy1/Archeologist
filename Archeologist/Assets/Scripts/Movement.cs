using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] InputAction movement;
    [SerializeField] InputAction jump;
    [SerializeField] GameObject stairObject;
    [SerializeField] float stairSeperatorDistance;
    [SerializeField] float stairSeperatorHeight;
    [SerializeField] bool freezeMovement;

    Rigidbody rigidBody;
    List<GameObject> pastStairs = new List<GameObject>();
    bool isGrounded;
    float currentStairPositionX = 0f;
    float currentStairPositionY = 0f;
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
        Vector3 pos = new Vector3(0, 0, transform.position.z);
        pastStairs.Add(Instantiate(stairObject, pos, Quaternion.identity));
    }

    private void Update()
    {
        ProcessInput();
        CreateNewStairs();
        if (transform.position.y <= -50)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIndex);
        }
    }
    private Vector2 CreateNewStair(float deltaX,float deltaY)
    {
        float newX = currentStairPositionX + deltaX;
        float newY = currentStairPositionY - deltaY;

        currentStairPositionX = newX;
        currentStairPositionY = newY;

        for(int i = 0; i < pastStairs.Count; i++)
        {
            if (pastStairs[i].transform.position.x == newX)
            {
                RemoveStair(i,false);
                break;
            }
        }

        Vector3 newPosition = new Vector3(newX, newY, transform.position.z);
        pastStairs.Add(Instantiate(stairObject, newPosition, Quaternion.identity));
        newPosition = new Vector3(newX+deltaX, newY-deltaY, transform.position.z);
        pastStairs.Add(Instantiate(stairObject, newPosition, Quaternion.identity));

        return new Vector2(newX, newY);
    }
    private void CreateNewStairs()
    {
        if (transform.position.x > currentStairPositionX + stairSeperatorDistance/2)
        {
            CreateNewStair(stairSeperatorDistance, stairSeperatorHeight);
            //CreateNewStair(stairSeperatorDistance, stairSeperatorHeight);
        }
        else if(transform.position.x < currentStairPositionX - stairSeperatorDistance/2)
        {
            CreateNewStair(-stairSeperatorDistance, -stairSeperatorHeight);
            //CreateNewStair(-stairSeperatorDistance, -stairSeperatorHeight);
        }

        for (int i = 0; i < pastStairs.Count; i++)
        {
            if (Mathf.Abs(pastStairs[i].transform.position.x - currentStairPositionX) > stairSeperatorDistance)
            {
                RemoveStair(i,true);
                i--;
            }
        }
    }
    private void RemoveStair(int index,bool useAnimation)
    {
        DropStair stairDrop = pastStairs[index].GetComponent<DropStair>();
        stairDrop.Drop(useAnimation);
        pastStairs.RemoveAt(index);
    }
    private void ProcessInput()
    {
        float horizontalValue = horizontalValue = movement.ReadValue<float>();
        if (freezeMovement) horizontalValue = 1f;
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
