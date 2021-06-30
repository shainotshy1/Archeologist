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
    [SerializeField] float pathSeperatorHeight;
    [SerializeField] int pathsInfrontAmount;
    [SerializeField] int minPlatformsBetweenTurns;

    List<GameObject> platforms = new List<GameObject>();
    System.Random random = new System.Random();
    int platformsSinceLastTurn = 0;
    enum CurrentAction
    {
        Straight,LeftTurn,RightTurn,Fork
    }
    CurrentAction currentAction;
    private void Start()
    {
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
            platforms.Add(addedPath);
        }
    }
    private void Update()
    {
        MovePaths();
        CreatePath();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void MovePaths()
    {
        Vector3 newPos = new Vector3(platforms[0].transform.localPosition.x, platforms[0].transform.localPosition.y, platforms[0].transform.localPosition.z - Time.deltaTime * movementSpeed);
        platforms[0].transform.localPosition = newPos;
        platforms[0].transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        if(!platforms[0].activeInHierarchy)platforms[0].SetActive(true);
        for (int i = 1; i < platforms.Count; i++)
        {
            newPos = new Vector3(platforms[i-1].transform.localPosition.x, platforms[i-1].transform.localPosition.y, platforms[i-1].transform.localPosition.z +pathSeperatorDistance);
            platforms[i].transform.localPosition = newPos;
            platforms[i].transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            if (!platforms[i].activeInHierarchy) platforms[i].SetActive(true);
        }
    }

    private void CreatePath()
    {
        if (platforms[0].transform.localPosition.z< transform.localPosition.z - 4*pathSeperatorDistance)
        {
            platforms[0].GetComponent<PlatformHandler>().RemovePlatform();
            platforms.RemoveAt(0);

            int randomVal = (int)(random.NextDouble() * 100);
            GameObject pathType;
            if (randomVal < 3 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
            {
                pathType = leftTurn;
                currentAction = CurrentAction.LeftTurn;
                platformsSinceLastTurn = 0;
            }
            else if(randomVal < 6 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
            {
                pathType = rightTurn;
                currentAction = CurrentAction.RightTurn;
                platformsSinceLastTurn = 0;
            }
            else if(randomVal < 9 && platformsSinceLastTurn >= minPlatformsBetweenTurns)
            {
                pathType = forkPath;
                currentAction = CurrentAction.Fork;
                platformsSinceLastTurn = 0;
            }
            else
            {
                pathType = straightPath;
                currentAction = CurrentAction.Straight;
                platformsSinceLastTurn++;
            }

            Vector3 newPosition = new Vector3(platforms[platforms.Count - 1].transform.localPosition.x, 0, pathSeperatorDistance + platforms[platforms.Count - 1].transform.localPosition.z);
            GameObject addedPath = Instantiate(pathType, newPosition, Quaternion.Euler(0, transform.eulerAngles.y, 0), transform);
            addedPath.GetComponent<PlatformHandler>().GenerateObstacle(transform.eulerAngles.y);
            addedPath.SetActive(false);
            platforms.Add(addedPath);
        }
    }
}
