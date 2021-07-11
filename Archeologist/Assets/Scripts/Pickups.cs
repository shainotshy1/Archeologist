using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    [SerializeField] float period;
    [SerializeField] float amplitude;
    [SerializeField] float boxColliderHeight;
    [SerializeField] float rotationSpeed;
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + boxColliderHeight, transform.position.z);
    }
    private void Update()
    {
        if(GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.GetComponent<BoxCollider>().size = new Vector3(0.5f, boxColliderHeight, 0.5f);
        }

        PickupBob();
    }

    private void PickupBob()
    {
        float yChange = Mathf.Sin(Time.timeSinceLevelLoad/period)*amplitude;

        if (Mathf.Abs(yChange) >= Mathf.Epsilon)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y+yChange, transform.position.z);
            gameObject.GetComponent<BoxCollider>().center = new Vector3(0, gameObject.GetComponent<BoxCollider>().center.x-yChange, 0);
        }

        transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * rotationSpeed);
    }

    public void SetXPosition(float xPos) 
    {
        transform.localPosition = new Vector3(xPos, 0, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Ground")
        {
            Destroy(gameObject);
        }
    }
}
