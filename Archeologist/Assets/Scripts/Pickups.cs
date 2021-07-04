using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    [SerializeField] float period;
    [SerializeField] float amplitude;
    [SerializeField] float boxColliderHeight;
    private void Update()
    {
        if(GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.GetComponent<BoxCollider>().size = new Vector3(5, 5, 5);
            gameObject.GetComponent<BoxCollider>().center = new Vector3(0, boxColliderHeight, 0);

            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        PickupBob();
    }

    private void PickupBob()
    {
        float yChange = Mathf.Sin(Time.timeSinceLevelLoad/period)*amplitude;

        if (Mathf.Abs(yChange) >= Mathf.Epsilon)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, yChange, transform.localPosition.z);
        }
    }

    public void SetXPosition(float xPos) 
    {
        transform.localPosition = new Vector3(xPos, 0, 0);
    }
}
