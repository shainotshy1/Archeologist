using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathHandler : MonoBehaviour
{
    public static float pathSeperatorDistance;

    [SerializeField] float movementSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float platformAlignSpeed;
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
    Vector3 playerPosition;
    Vector3 alignedPosition;
    List<GameObject> platforms = new List<GameObject>();
    System.Random random = new System.Random();
    float targetAngle;
    float currentAngle;
    float currentSpeed;
    float _turnPlatformShift;
    public int distance = 0;
    int platformsSinceLastTurn = 0;
    bool directionSet;
    TurnType currentTurnType;
    Transform playerTransform;
    TextMeshProUGUI distanceBoard;
    private void Start()
    {
        _turnPlatformShift = 0f;
        directionSet = true;
        nextPlatformPositioning = Vector3.zero;
        currentDirection = Vector3.forward;
        alignedPosition = Vector3.zero;
        nextDirection = currentDirection;
        playerTransform = FindObjectOfType<PlayerControls>().GetComponent<Transform>();
        playerPosition = playerTransform.position;
        currentSpeed = movementSpeed;
        targetAngle = currentAngle = 0f;

        distanceBoard = GameObject.FindGameObjectWithTag("Distance").GetComponent<TextMeshProUGUI>();

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
        PathAlignWithPlayer();
        DisplayDistance();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (movementSpeed < maxSpeed)
        {
            movementSpeed += Time.deltaTime * acceleration;
        }
        else
        {
            movementSpeed = maxSpeed;
        }
    }
    private void DisplayDistance()
    {
        distanceBoard.text = $"Distance: {distance} m";
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
            float newPosVal = (currentTurnType == TurnType.Right) ? rotationRadius : -rotationRadius;

            if (!directionSet)
            {
                targetAngle = (currentTurnType == TurnType.Left) ? currentAngle - 90f : currentAngle + 90f;
                playerPosition = playerTransform.position;
                directionSet = true;
            }
            if ((currentTurnType == TurnType.Left && currentAngle > targetAngle)|| (currentTurnType == TurnType.Right && currentAngle < targetAngle))
            {
                float angleChange = Time.deltaTime * movementSpeed*180/(Mathf.PI*rotationRadius)*(targetAngle-currentAngle)/Mathf.Abs(targetAngle-currentAngle);
                currentAngle += angleChange;
                RotatePlayer(angleChange, newPosVal);
                
            }
            else
            {
                RotatePlayer(targetAngle - currentAngle, newPosVal);
                currentDirection = nextDirection;
                currentAngle = targetAngle;
                currentSpeed = movementSpeed;
                nextPlatformPositioning = Vector3.zero;

                float y = transform.position.y;
                float x = transform.position.x + (playerTransform.position.x - platforms[platforms.Count - 1].transform.position.x) * Mathf.Abs(currentDirection.z);
                float z = transform.position.z + (playerTransform.position.z - platforms[platforms.Count - 1].transform.position.z) * Mathf.Abs(currentDirection.x);

                alignedPosition = new Vector3(x, y, z);
            }
        }
    }
    private void RotatePlayer(float angle,float newPosVal)
    {
        playerTransform.RotateAround(new Vector3(playerPosition.x + newPosVal * currentDirection.z, 0, playerPosition.z - newPosVal * currentDirection.x), Vector3.up, angle);
        IncreaseDistance();
    }
    private void PathAlignWithPlayer()
    {
        transform.position = Vector3.Lerp(transform.position,alignedPosition,Time.deltaTime*platformAlignSpeed);
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

            Vector3 newPos = new Vector3(x + xChange, y, z + zChange);
            platforms[i].transform.localPosition = newPos;

            if (!platforms[i].activeInHierarchy) platforms[i].SetActive(true);
        }
        IncreaseDistance();
    }
    private void IncreaseDistance()
    {
        distance += (int)(Time.deltaTime * movementSpeed);
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
        if (distance>pathsInBackAmount*pathSeperatorDistance&&currentSpeed!=0&&PathPlayerDistance(1) > 0)
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
            /*else if (randomVal < 18 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
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

            float xShift = _turnPlatformShift * nextPlatformPositioning.x;
            float zShift = _turnPlatformShift * nextPlatformPositioning.z;

            float newX = pathSeperatorDistance * nextDirection.x + platforms[platforms.Count - 1].transform.localPosition.x + xShift;
            float newZ = pathSeperatorDistance * nextDirection.z + platforms[platforms.Count - 1].transform.localPosition.z + zShift;

            Vector3 newPosition = new Vector3(newX, 0, newZ);

            GameObject addedPath = Instantiate(pathType, Vector3.zero, Quaternion.identity, transform);
            addedPath.transform.localPosition = newPosition;
            addedPath.GetComponent<PlatformHandler>().turnType = turnType;
            addedPath.transform.rotation = Quaternion.Euler(0, playerTransform.eulerAngles.y*turnFactor+nextDirection.x*90*(1-turnFactor), 0);
            if(turnType == TurnType.Straight)
            {
                addedPath.GetComponent<PlatformHandler>().PlaceObjectOnPath(playerTransform.eulerAngles.y * turnFactor + nextDirection.x * 90 * (turnFactor - 1));
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

            SetTurnPositiong();
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

            SetTurnPositiong();
        }
        else
        {
            nextPlatformPositioning = Vector3.zero;

            _turnPlatformShift = 0f;
        }
    }

    private void SetTurnPositiong()
    {
        nextPlatformPositioning = currentDirection + nextDirection;

        _turnPlatformShift = rotationRadius;
    }
}
