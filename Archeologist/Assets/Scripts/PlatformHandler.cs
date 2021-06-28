using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> obstacles;

    System.Random random = new System.Random();
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        PlayerControls.playerMovementDistance = GetComponent<BoxCollider>().size.x / 3;
    }
    private void Update()
    {
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
            float obstaclePlacement = ((int)(random.NextDouble() * 3)-1)* PlayerControls.playerMovementDistance;
            Vector3 obstaclePosition = new Vector3(obstaclePlacement, transform.position.y, transform.position.z);
            int index = (int)(random.NextDouble() * obstacles.Count);
            Instantiate(obstacles[index], obstaclePosition, Quaternion.identity, transform);
        }
    }
}
