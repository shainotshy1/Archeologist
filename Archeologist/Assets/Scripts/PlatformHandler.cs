using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnType 
{ 
    Left,Right,Straight,Fork
}
public class PlatformHandler : MonoBehaviour
{
    public static readonly System.Random random = new System.Random();
    public TurnType turnType;

    [SerializeField] List<GameObject> pickups;
    [SerializeField] List<GameObject> obstacles;

    static int lastPickupPosition = lastPickupPosition = (int)(random.NextDouble() * 3);
    static int pickupsInRow = 0;
    int minPickupsInRow = 5;
    void Start()
    {
        if(PlayerControls.playerMovementDistance == 0 && GetComponent<BoxCollider>()!=null) PlayerControls.playerMovementDistance = GetComponent<BoxCollider>().size.z / 2.1f;
    }
    public void RemovePlatform()
    {
        Destroy(gameObject);
    }
    public void PlaceObjectOnPath(float angleY)
    {
        int randomInt = random.Next(0, 100);
        if (randomInt<20)
        {
            GenerateObject(obstacles, angleY);
        }
        else
        {
            GenerateObject(pickups, angleY);
        }

    }
    public void GenerateObject(List<GameObject> gameObjects, float angleY)
    {
        if (gameObjects.Count > 0)
        {
            int index = (int)(random.NextDouble() * gameObjects.Count);
            int placement = (int)(random.NextDouble() * 3);

            if(gameObjects == obstacles)
            {
                pickupsInRow = minPickupsInRow;
            }
            else if (gameObjects==pickups)
            {
                if (random.NextDouble()*100 >70 || pickupsInRow < minPickupsInRow)
                {
                    placement = lastPickupPosition;
                    pickupsInRow++;
                }
                else
                {
                    if (lastPickupPosition != placement) pickupsInRow = 0;

                    lastPickupPosition = placement;
                }
            }

            Vector3 position = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);

            GameObject newObject = Instantiate(gameObjects[index], position, Quaternion.Euler(0, angleY, 0), transform);
            newObject.GetComponent<Transform>().localPosition = new Vector3((placement-1)*PlayerControls.playerMovementDistance, 0, 0);
        }
    }
}