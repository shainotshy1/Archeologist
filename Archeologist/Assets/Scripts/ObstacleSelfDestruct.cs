using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSelfDestruct : MonoBehaviour
{
    void Update()
    {
        if (transform.localPosition.y < 0)
        {
            Destroy(gameObject);
        }
    }
}
