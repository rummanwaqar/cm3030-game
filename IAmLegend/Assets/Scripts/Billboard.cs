using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }
    private void LateUpdate()
    {
        // Look at the camera all the time to prevent canvas rotations
        transform.LookAt(transform.position + cam.forward);
    }
}
