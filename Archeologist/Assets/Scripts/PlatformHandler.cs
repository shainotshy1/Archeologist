using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [SerializeField] float rotation;
    void Start()
    {
        transform.Rotate(0, 0, rotation);
    }

}
