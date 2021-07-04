using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    public static float pathSeperatorDistance;

    [SerializeField] float movementSpeed;
    [SerializeField] GameObject straightPath;
    [SerializeField] GameObject rightTurn;
    [SerializeField] GameObject leftTurn;
    [SerializeField] GameObject forkPath;
    [SerializeField] int pathsInfrontAmount;
    [SerializeField] int pathsInBackAmount;
    [SerializeField] int minPlatformsBetweenTurns;
    [SerializeField] float rotationRadius;

    Vector3 currentDirection;
    Vector3 nextDirection;
    Vector3 nextPlatformPositioning;
    List<GameObject> platforms = new List<GameObject>();
    System.Random random = new System.Random();
    float targetAngle;
    float currentAngle;
    float currentSpeed;
    float _turnPlatformShift;
    int platformsSinceLastTurn = 0;
    bool directionSet;
    TurnType currentTurnType;
    Transform playerTransform;
    private void Start()
    {
        _turnPlatformShift = 0f;
        directionSet = true;
        nextPlatformPositioning = Vector3.zero;
        currentDirection = Vector3.forward;
        nextDirection = currentDirection;
        playerTransform = FindObjectOfType<PlayerControls>().GetComponent<Transform>();
        currentSpeed = movementSpeed;
        targetAngle = currentAngle = 0f;
        BoxCollider boxCollider = straightPath.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            pathSeperatorDistance = boxCollider.size.z;
        }
        else
        {
            pathSeperatorDistance = 0;
        }

        for (int i = -pathsInBackAmount; i <= pathsInfrontAmount; i++)
        {
            Vector3 newPosition = new Vector3(0, 0, pathSeperatorDistance*i);
            GameObject addedPath = Instantiate(straightPath, newPosition, Quaternion.Euler(0, transform.eulerAngles.y,0), transform);
            addedPath.GetComponent<PlatformHandler>().turnType = TurnType.Straight;
            platforms.Add(addedPath);
        }
    }
    private void Update()
    {
        if(currentSpeed != 0) currentSpeed = movementSpeed;
        TurnPlayer();
        CreatePath();
        MovePaths();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void TurnPlayer()
    {
        currentTurnType = platforms[pathsInBackAmount].GetComponent<PlatformHandler>().turnType;
        if (currentTurnType != TurnType.Straight && currentSpeed != 0)
        {
            float distance = PathPlayerDistance(pathsInBackAmount);
            if (distance >= 0)
            {
                directionSet = false;
                currentSpeed = 0;
            }
        }
        if (currentSpeed == 0)
        {
            if (!directionSet)
            {
                targetAngle = (currentTurnType == TurnType.Left) ? currentAngle - 90f : currentAngle + 90f;
                ChangePlayerRotationAxis(currentTurnType);
                directionSet = true;
            }
            if ((currentTurnType == TurnType.Left && currentAngle > targetAngle)|| (currentTurnType == TurnType.Right && currentAngle < targetAngle))
            {
                float angleChange = Time.deltaTime * movementSpeed*180/(Mathf.PI*rotationRadius)*(targetAngle-currentAngle)/Mathf.Abs(targetAngle-currentAngle);
                currentAngle += angleChange;
                playerTransform.Rotate(new Vector3(0, 1, 0), angleChange);
            }
            else
            {
                currentDirection = nextDirection;
                currentAngle = targetAngle;
                currentSpeed = movementSpeed;
                nextPlatformPositioning = Vector3.zero;
            }
        }
    }

    private void ChangePlayerRotationAxis(TurnType turn)
    {
        if(turn == TurnType.Straight)
        {
            foreach (Transform child in playerTransform)
            {
                if (child.gameObject.tag == "Player")
                {
                    child.transform.localPosition = new Vector3(0, child.transform.localPosition.y, child.transform.localPosition.z);
                }
            }
            playerTransform.localPosition = new Vector3(0, playerTransform.localPosition.y, playerTransform.localPosition.z);
        }
        else
        {
            float newPosVal = (turn == TurnType.Right) ? rotationRadius : -rotationRadius;
            foreach (Transform child in playerTransform)
            {
                if (child.gameObject.tag == "Player")
                {
                    child.transform.localPosition = new Vector3( - newPosVal * currentDirection.z, child.transform.localPosition.y, - newPosVal * currentDirection.x);
                }
            }
            playerTransform.localPosition = new Vector3( newPosVal*currentDirection.z, playerTransform.localPosition.y, newPosVal *currentDirection.x);
        }
    }

    private void MovePaths()
    {
        float xChange = -Time.deltaTime * currentSpeed * currentDirection.x;
        float zChange = -Time.deltaTime * currentSpeed * currentDirection.z;

        for (int i = 0; i < platforms.Count; i++)
        {
            float y = platforms[i].transform.localPosition.y;
            float x = platforms[i].transform.localPosition.x;
            float z = platforms[i].transform.localPosition.z;

            Vector3 newPos = new Vector3(x+xChange, y, z+zChange);
            platforms[i].transform.localPosition = newPos;
            
            if (!platforms[i].activeInHierarchy) platforms[i].SetActive(true);
        }
    }
    private float PathPlayerDistance(int pathIndex)
    {
        float pathX = platforms[pathIndex].transform.position.x;
        float pathZ = platforms[pathIndex].transform.position.z;
        float playerX = playerTransform.transform.position.x;
        float playerZ = playerTransform.transform.position.z;

        return playerX * currentDirection.x + playerZ * currentDirection.z - pathX * currentDirection.x - pathZ * currentDirection.z;
    }
    private void CreatePath()
    {
        float distance = PathPlayerDistance(0);
        if (distance>pathsInBackAmount*pathSeperatorDistance&&currentSpeed!=0)
        {
            platforms[0].GetComponent<PlatformHandler>().RemovePlatform();
            platforms.RemoveAt(0);

            int randomVal = (int)(random.NextDouble() * 100);
            GameObject pathType;
            TurnType turnType;
            float turnFactor = 0f;

            if (randomVal < 6 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
            {
                pathType = leftTurn;
                turnType = TurnType.Left;
                platformsSinceLastTurn = 0;
                turnFactor = 1f;
            }
            else if (randomVal < 12 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
            {
                pathType = rightTurn;
                turnType = TurnType.Right;
                platformsSinceLastTurn = 0;
                turnFactor = 1f;
            }
            /*else if (randomVal < 9 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
            {
                pathType = forkPath;
                turnType = TurnType.Fork;
                platformsSinceLastTurn = 0;
            }*/
            else
            {
                pathType = straightPath;
                turnType = TurnType.Straight;
                platformsSinceLastTurn++;
            }

            Vector3 newPosition = new Vector3(pathSeperatorDistance*nextDirection.x + platforms[platforms.Count - 1].transform.localPosition.x + _turnPlatformShift * nextPlatformPositioning.x, 0, pathSeperatorDistance*nextDirection.z + platforms[platforms.Count - 1].transform.localPosition.z + _turnPlatformShift * nextPlatformPositioning.z);
            GameObject addedPath = Instantiate(pathType, Vector3.zero, Quaternion.identity, transform);

            addedPath.transform.localPosition = newPosition;
            addedPath.GetComponent<PlatformHandler>().turnType = turnType;
            addedPath.transform.rotation = Quaternion.Euler(0, playerTransform.eulerAngles.y*turnFactor+nextDirection.x*90*(turnFactor-1), 0);
            if(turnType == TurnType.Straight)
            {
                addedPath.GetComponent<PlatformHandler>().GenerateObstacle(transform.eulerAngles.y);
            }
            addedPath.SetActive(false);
            platforms.Add(addedPath);

            ChangeDirection(turnType);
        }
    }
    private void ChangeDirection(TurnType turn)
    {
        if(turn == TurnType.Left)
        {
            if (currentDirection == Vector3.right)
            {
                nextDirection = Vector3.forward;
            }
            else if(currentDirection == Vector3.left)
            {
                nextDirection = Vector3.back;
            }
            else if (currentDirection == Vector3.forward)
            {
                nextDirection = Vector3.left;
            }
            else if (currentDirection == Vector3.back)
            {
                nextDirection = Vector3.right;
            }

            nextPlatformPositioning = currentDirection + nextDirection;

            _turnPlatformShift = rotationRadius;
        }
        else if(turn == TurnType.Right)
        {
            if (currentDirection == Vector3.right)
            {
                nextDirection = Vector3.back;
            }
            else if (currentDirection == Vector3.left)
            {
                nextDirection = Vector3.forward;
            }
            else if (currentDirection == Vector3.forward)
            {
                nextDirection = Vector3.right;
            }
            else if (currentDirection == Vector3.back)
            {
                nextDirection = Vector3.left;
            }

            nextPlatformPositioning = currentDirection + nextDirection;

            _turnPlatformShift = rotationRadius;
        }
        else
        {
            _turnPlatformShift = 0f;
        }
    }
}
