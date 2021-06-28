using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{
    private void Update()
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
    public void SetXPosition(float xPos) 
    {
        transform.localPosition = new Vector3(xPos, 0, 0);
    }
}
