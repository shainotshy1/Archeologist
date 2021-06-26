using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropStair : MonoBehaviour
{
    public void Drop(bool useAnimation)
    {
        /*if (useAnimation)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
        else
        {
            Destroy(gameObject);
        }*/

        Destroy(gameObject);
    }
    private void Update()
    {
        if (gameObject.transform.position.y < -50)
        {
            Destroy(gameObject);
        }
    }
}
