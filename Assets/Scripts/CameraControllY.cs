using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllY : MonoBehaviour
{
    public float offsetY = 0;    

    void Update()
    {
        transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y+offsetY, transform.position.z);
    }
}
