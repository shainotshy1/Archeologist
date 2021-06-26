using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSelfDestruct : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -50)
        {
            Destroy(gameObject);
        }
    }
}
