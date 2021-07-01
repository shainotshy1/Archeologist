using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    public static float pathSeperatorDistance;
    public static float middleXVal;

    [SerializeField] float movementSpeed;
    [SerializeField] GameObject straightPath;
    [SerializeField] GameObject rightTurn;
    [SerializeField] GameObject leftTurn;
    [SerializeField] GameObject forkPath;
    [SerializeField] float pathSeperatorHeight;
    [SerializeField] int pathsInfrontAmount;
    [SerializeField] int minPlatformsBetweenTurns;
    [SerializeField] float rotationRadius;
    [SerializeField] float stopShift;

    Vector3 direction;
    List<GameObject> platforms = new List<GameObject>();
    System.Random random = new System.Random();
    float targetAngle;
    float currentAngle;
    int platformsSinceLastTurn = 0;
    bool turnMade;
    bool directionSet;
    float currentSpeed;
    TurnType currentTurnType;
    Transform playerTransform;
    private void Start()
    {
        directionSet = true;
        middleXVal = 0f;
        direction = new Vector3(0, 0, 1);
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
            pathSeperatorDistance = 160;
        }

        for (int i = -4; i <= pathsInfrontAmount; i++)
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
        MovePaths();
        CreatePath();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void TurnPlayer()
    {
        currentTurnType = platforms[4].GetComponent<PlatformHandler>().turnType;
        if (currentTurnType != TurnType.Straight)
        {
            if (platforms[4].GetComponent<Transform>().position.x + platforms[4].GetComponent<Transform>().position.z< playerTransform.position.x+playerTransform.position.z + stopShift && currentSpeed!=0)
            {
                directionSet = false;
                currentSpeed = 0;
            }
        }
        if ((currentTurnType == TurnType.Left ||currentTurnType==TurnType.Right)&& currentSpeed == 0)
        {
            if(!directionSet)
            {
                targetAngle = (currentTurnType==TurnType.Left)? currentAngle - 90f: currentAngle + 90f;
                SwitchParentChildPosition(currentTurnType);
                directionSet = true;
            }
            else if ((currentTurnType== TurnType.Left && currentAngle > targetAngle)|| (currentTurnType == TurnType.Right && currentAngle < targetAngle))
            {
                float angleChange = Time.deltaTime * movementSpeed*180/(Mathf.PI*rotationRadius)*(targetAngle-currentAngle)/Mathf.Abs(targetAngle-currentAngle);
                currentAngle += angleChange;
                playerTransform.Rotate(new Vector3(0, 1, 0), angleChange);
            }
            else
            {
                currentAngle = targetAngle;
            }
        }
    }

    private void SwitchParentChildPosition(TurnType turn)
    {
        if (turn == TurnType.Straight) return;
        float newX = (turn == TurnType.Right) ? rotationRadius : -rotationRadius;
        foreach (Transform child in playerTransform)
        {
            if (child.gameObject.tag == "Player")
            {
                child.transform.localPosition = new Vector3(-newX, child.transform.localPosition.y, child.transform.localPosition.z);
            }
        }
        playerTransform.localPosition = new Vector3(newX, transform.localPosition.y, transform.localPosition.z);
        middleXVal = newX;
    }

    private void MovePaths()
    {
        int frontPath = 0;
        float xChange = 0;
        float zChange = -Time.deltaTime * currentSpeed*direction.z;
        Vector3 newPos = new Vector3(platforms[frontPath].transform.localPosition.x + xChange, platforms[frontPath].transform.localPosition.y, platforms[frontPath].transform.localPosition.z + zChange);
        platforms[frontPath].transform.localPosition = newPos;
        platforms[frontPath].transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        if (!platforms[frontPath].activeInHierarchy) platforms[frontPath].SetActive(true);
        for (int i = frontPath + 1; i < platforms.Count; i++)
        {
            newPos = new Vector3(platforms[i - 1].transform.localPosition.x + pathSeperatorDistance*direction.x, platforms[i - 1].transform.localPosition.y, platforms[i - 1].transform.localPosition.z + pathSeperatorDistance*direction.z);

            platforms[i].transform.localPosition = newPos;
            platforms[i].transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            if (!platforms[i].activeInHierarchy) platforms[i].SetActive(true);
        }
    }

    private void CreatePath()
    {
        if (platforms[0].transform.localPosition.z< transform.localPosition.z - 4*pathSeperatorDistance && platforms.Count>5)
        {
            platforms[0].GetComponent<PlatformHandler>().RemovePlatform();
            platforms.RemoveAt(0);

            if (!turnMade)
            {
                int randomVal = (int)(random.NextDouble() * 100);
                GameObject pathType;
                TurnType turnType;

                if (randomVal < 3 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
                {
                    pathType = leftTurn;
                    turnType = TurnType.Left;
                    platformsSinceLastTurn = 0;
                    turnMade = true;
                }
                else if (randomVal < 21 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
                {
                    pathType = rightTurn;
                    turnType = TurnType.Right;
                    platformsSinceLastTurn = 0;
                    turnMade = true;
                }
                else if (randomVal < 9 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
                {
                    pathType = forkPath;
                    turnType = TurnType.Fork;
                    platformsSinceLastTurn = 0;
                    turnMade = true;
                }
                else
                {
                    pathType = straightPath;
                    turnType = TurnType.Straight;
                    platformsSinceLastTurn++;
                }

                Vector3 newPosition = new Vector3(platforms[platforms.Count - 1].transform.localPosition.x, 0, pathSeperatorDistance + platforms[platforms.Count - 1].transform.localPosition.z);
                GameObject addedPath = Instantiate(pathType, newPosition, Quaternion.Euler(0, transform.eulerAngles.y, 0), transform);
                addedPath.GetComponent<PlatformHandler>().turnType = turnType;
                if(turnType == TurnType.Straight)
                {
                    addedPath.GetComponent<PlatformHandler>().GenerateObstacle(transform.eulerAngles.y);
                }
                addedPath.SetActive(false);
                platforms.Add(addedPath);
            }   
        }
    }
}
