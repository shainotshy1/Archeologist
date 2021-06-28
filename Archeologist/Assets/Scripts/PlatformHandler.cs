using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> obstacles;

    System.Random random = new System.Random();
    void Start()
    {
        PlayerControls.playerMovementDistance = GetComponent<BoxCollider>().size.x / 3;
    }
    private void Update()
    {
        if(transform.localPosition.z < -30&&gameObject.name == "Start")
        {
            RemovePlatform();
        }
    }
    public void RemovePlatform()
    {
        Destroy(gameObject);
    }
    public void GenerateObstacle(float angleY)
    {
        if (obstacles.Count > 0)
        {
            float obstaclePlacement = ((int)(random.NextDouble() * 3) - 1) * PlayerControls.playerMovementDistance;
            Vector3 obstaclePosition = new Vector3(transform.position.x, 0, transform.position.z);
            int index = (int)(random.NextDouble() * obstacles.Count);
            GameObject newObstacle = Instantiate(obstacles[index], obstaclePosition, Quaternion.Euler(0,angleY,0), transform);
            newObstacle.GetComponent<ObstacleHandler>().SetXPosition(obstaclePlacement);
        }
    }
}