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
    [SerializeField] float obstaclePlacementRange;
    [SerializeField] InputAction movement;
    [SerializeField] GameObject stairObject;
    [SerializeField] float stairSeperatorDistance;
    [SerializeField] float stairSeperatorHeight;
    [SerializeField] int stairsInfrontAmount;
    [SerializeField] List<GameObject> obstacles;

    List<GameObject> pastStairs = new List<GameObject>();
    System.Random random = new System.Random();
    Vector3 currentStairPosition = new Vector3(0, 0, 0);
    Vector3 movementDirection = new Vector3(0f, 0f, 1f);

    private void OnEnable()
    {
        movement.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
    }
    private void Start()
    {
        for(int i = 0; i < stairsInfrontAmount; i++)
        {
            Vector3 pos = new Vector3(movementDirection.x*-stairSeperatorDistance*i, -stairSeperatorHeight*i, movementDirection.z * -stairSeperatorDistance * i);
            pastStairs.Add(Instantiate(stairObject, pos, Quaternion.identity));
            if (i > 0)
            {
                pos = new Vector3(movementDirection.x * stairSeperatorDistance * i, stairSeperatorHeight * i, movementDirection.z * stairSeperatorDistance * i);
                pastStairs.Add(Instantiate(stairObject, pos, Quaternion.identity));
            }
        }
    }

    private void Update()
    {
        MovementHandler();
        StairHandler();
        if (transform.position.y <= -50)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIndex);
        }
    }
    private void CreateNewStairSet(float deltaZ,float deltaY,float deltaX)
    {
        float newX = currentStairPosition.x + deltaX*movementDirection.x;
        float newY = currentStairPosition.y + deltaY;
        float newZ = currentStairPosition.z + deltaZ*movementDirection.z;

        currentStairPosition.x = newX;
        currentStairPosition.y = newY;
        currentStairPosition.z = newZ;

        for (int i = 0; i < stairsInfrontAmount; i++)
        {
            Vector3 newPosition = new Vector3(newX + deltaX * i * movementDirection.x, newY + deltaY * i, newZ + deltaZ * i + movementDirection.z);
            bool exists = false;
            for(int k = 0; k < pastStairs.Count; k++)
            {
                if(pastStairs[k].transform.position == newPosition)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                Quaternion rotation = Quaternion.identity;
                pastStairs.Add(Instantiate(stairObject, newPosition, rotation));

                if (random.Next(0, 100) >80&&obstacles.Count>0)
                {
                    float obstaclePlacementAxis = ((int)(random.NextDouble() * 3) - 1) * obstaclePlacementRange;
                    Vector3 obstaclePosition = new Vector3(obstaclePlacementAxis*movementDirection.z+newPosition.x, newPosition.y+1f, obstaclePlacementAxis * movementDirection.x+newPosition.z);
                    int index = random.Next(0, obstacles.Count - 1);
                    Instantiate(obstacles[index], obstaclePosition, Quaternion.identity);
                }
            }
        }
    }
    private void StairHandler()
    {
        if (transform.position.x + transform.position.z > currentStairPosition.x + currentStairPosition.z + stairSeperatorDistance/2)
        {
            CreateNewStairSet(stairSeperatorDistance, stairSeperatorHeight,stairSeperatorDistance);
        }
        else if(transform.position.x + transform.position.z < currentStairPosition.x + currentStairPosition.z - stairSeperatorDistance/2)
        {
            CreateNewStairSet(-stairSeperatorDistance, -stairSeperatorHeight,-stairSeperatorDistance);
        }

        for (int i = 0; i < pastStairs.Count; i++)
        {
            Vector3 difference = new Vector3(pastStairs[i].transform.position.x - currentStairPosition.x,0, pastStairs[i].transform.position.z - currentStairPosition.z);
            if (difference.x*movementDirection.x+difference.z*movementDirection.z < -stairSeperatorDistance * 3*(movementDirection.x+movementDirection.z))
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
    private void MovementHandler()
    {
        float movementForward = Time.deltaTime * movementSpeed;
        if (Mathf.Abs(movementForward) > Mathf.Epsilon)
        {
            transform.Translate(movementForward*Vector3.forward);
        }
    }
}
