using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnType 
{ 
    Left,Right,Straight,Fork
}
public class PlatformHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> obstacles;
    public TurnType turnType;

    System.Random random = new System.Random();
    void Start()
    {
        if(PlayerControls.playerMovementDistance == 0 && GetComponent<BoxCollider>()!=null) PlayerControls.playerMovementDistance = GetComponent<BoxCollider>().size.z / 2.1f;
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
            Vector3 obstaclePosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            int index = (int)(random.NextDouble() * obstacles.Count);
            GameObject newObstacle = Instantiate(obstacles[index], obstaclePosition, Quaternion.Euler(0,angleY,0), transform);
            newObstacle.GetComponent<Pickups>().SetXPosition(obstaclePlacement);
        }
    }
}