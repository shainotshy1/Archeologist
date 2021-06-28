using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{
    void Update()
    {
        if(transform.localPosition.z <= PathHandler.pathSeperatorDistance/2)
        {
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
                gameObject.GetComponent<BoxCollider>().size = new Vector3(3, 3, 3);

                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }    
    }
}
