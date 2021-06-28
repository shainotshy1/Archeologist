using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] GameObject straightPath;
    [SerializeField] GameObject rightTurn;
    [SerializeField] GameObject leftTurn;
    [SerializeField] GameObject forkPath;
    [SerializeField] float pathSeperatorHeight;
    [SerializeField] int pathsInfrontAmount;

    public static float pathSeperatorDistance;
    List<GameObject> platforms = new List<GameObject>();
    private void Start()
    {
        pathSeperatorDistance = straightPath.GetComponent<BoxCollider>().size.z;

        for (int i = -4; i <= pathsInfrontAmount; i++)
        {
            Vector3 newPosition = new Vector3(0, 0, pathSeperatorDistance*i);
            GameObject addedPath = Instantiate(straightPath, newPosition, Quaternion.identity,transform);
            platforms.Add(addedPath);
        }
    }
    private void Update()
    {
        CreatePath();
        MovePaths();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void MovePaths()
    {
        Vector3 newPos = new Vector3(platforms[0].transform.position.x, platforms[0].transform.position.y, platforms[0].transform.position.z - Time.deltaTime * movementSpeed);
        platforms[0].transform.position = newPos;
        for(int i = 1; i < platforms.Count; i++)
        {
            newPos = new Vector3(platforms[i-1].transform.position.x, platforms[i-1].transform.position.y, platforms[i-1].transform.position.z + pathSeperatorDistance);
            platforms[i].transform.position = newPos;
        }
    }

    private void CreatePath()
    {
        if (platforms[0].transform.position.z < transform.position.z - 4*pathSeperatorDistance)
        {
            platforms[0].GetComponent<PlatformHandler>().RemovePlatform();
            platforms.RemoveAt(0);

            Vector3 newPosition = new Vector3(0, 0, pathSeperatorDistance+platforms[platforms.Count-1].transform.position.z);
            GameObject addedPath = Instantiate(straightPath, newPosition, Quaternion.identity, transform);
            addedPath.GetComponent<PlatformHandler>().GenerateObstacle();
            platforms.Add(addedPath);
        }
    }
}
