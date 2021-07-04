using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    [SerializeField] float period;
    [SerializeField] float amplitude;
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

        PickupBob();
    }

    private void PickupBob()
    {
        float yChange = Mathf.Sin(Time.timeSinceLevelLoad/period)*amplitude;

        if (Mathf.Abs(yChange) >= Mathf.Epsilon)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + yChange, transform.localPosition.z);
        }
    }

    public void SetXPosition(float xPos) 
    {
        transform.localPosition = new Vector3(xPos, 0, 0);
    }
}
