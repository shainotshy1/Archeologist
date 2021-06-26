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
    float currentStairPositionX = 0f;
    float currentStairPositionY = 0f;

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
            Vector3 pos = new Vector3(-stairSeperatorDistance*i, -stairSeperatorHeight*i, 0);
            pastStairs.Add(Instantiate(stairObject, pos, Quaternion.identity));
            if (i > 0)
            {
                pos = new Vector3(stairSeperatorDistance * i, stairSeperatorHeight * i, 0);
                pastStairs.Add(Instantiate(stairObject, pos, Quaternion.identity));
            }
        }
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
        float newY = currentStairPositionY + deltaY;

        currentStairPositionX = newX;
        currentStairPositionY = newY;

        for (int i = 0; i < stairsInfrontAmount; i++)
        {
            bool exists = false;
            for(int k = 0; k < pastStairs.Count; k++)
            {
                if(pastStairs[k].transform.position.x == newX + deltaX * i)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                Vector3 newPosition = new Vector3(newX + deltaX * i, newY + deltaY * i, 0);
                Quaternion rotation = Quaternion.identity;
                pastStairs.Add(Instantiate(stairObject, newPosition, rotation));

                System.Random random = new System.Random();
                if (random.Next(0, 10) > 6&&obstacles.Count>0)
                {
                    Vector3 obstaclePosition = new Vector3(newPosition.x, newPosition.y+1f, ((int)(random.NextDouble()*3)-1)*obstaclePlacementRange);
                    int index = random.Next(0, obstacles.Count - 1);
                    Instantiate(obstacles[index], obstaclePosition, Quaternion.identity);
                }
            }
        }
        
        return new Vector2(newX, newY);
    }
    private void CreateNewStairs()
    {
        if (transform.position.x > currentStairPositionX + stairSeperatorDistance/2)
        {
            CreateNewStair(stairSeperatorDistance, stairSeperatorHeight);
        }
        else if(transform.position.x < currentStairPositionX - stairSeperatorDistance/2)
        {
            CreateNewStair(-stairSeperatorDistance, -stairSeperatorHeight);
        }

        for (int i = 0; i < pastStairs.Count; i++)
        {
            if (Mathf.Abs(pastStairs[i].transform.position.x - currentStairPositionX) > stairSeperatorDistance*stairsInfrontAmount|| pastStairs[i].transform.position.x - currentStairPositionX < -stairSeperatorDistance * 10)
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

        float xDelta = Time.deltaTime * movementSpeed;
        if (Mathf.Abs(xDelta) > Mathf.Epsilon)
        {
            transform.Translate(xDelta, 0, 0);
        }

    }
}
