using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> obstacles;
    [SerializeField] float obstaclePlacementRange;

    System.Random random = new System.Random();
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 90f, 0);
    }
    private void Update()
    {
        float newZ = transform.position.z - PathHandler.speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);

        if(transform.position.z < -30&&gameObject.name == "Start")
        {
            RemovePlatform();
        }
    }
    public void RemovePlatform()
    {
        Destroy(gameObject);
    }
    public void GenerateObstacle()
    {
        if (obstacles.Count > 0)
        {
            float obstaclePlacement = ((int)(random.NextDouble() * 3)-1)* obstaclePlacementRange;
            Vector3 obstaclePosition = new Vector3(transform.position.x, transform.position.y + 1f, obstaclePlacement);
            int index = (int)(random.NextDouble() * obstacles.Count);
            Instantiate(obstacles[index], obstaclePosition, Quaternion.identity, transform);
        }
    }
}
