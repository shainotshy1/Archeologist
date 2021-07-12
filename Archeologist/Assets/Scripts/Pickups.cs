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
    [SerializeField] int scoreValue;
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + boxColliderHeight, transform.position.z);
    }
    private void Update()
    {
        if(GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.GetComponent<BoxCollider>().size = new Vector3(1f, boxColliderHeight, 1f);
        }

        PickupBob();
    }

    private void PickupBob()
    {
        float yChange = Mathf.Sin(Time.timeSinceLevelLoad/period)*amplitude;

        if (Mathf.Abs(yChange) >= Mathf.Epsilon)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y+yChange, transform.localPosition.z);
            gameObject.GetComponent<BoxCollider>().center = new Vector3(0, gameObject.GetComponent<BoxCollider>().center.x-yChange, 0);
        }

        transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * rotationSpeed);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "JumpCenter")
        {
            ScoreHandler scoreHandler = new ScoreHandler();
            scoreHandler.ChangeScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
