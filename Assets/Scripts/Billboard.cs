using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Camera").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cam == null)
        {
            cam = GameObject.Find("Camera").transform;

            if (cam == null)
            {
                return;
            }
        }
        Vector3 lookAtPos = transform.position + cam.forward;
        lookAtPos.y = transform.position.y; // Lock rotation around the Y-axis
        transform.LookAt(lookAtPos);
    }
}
