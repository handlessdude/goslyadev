using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public PixelPerfectCamera pixCamera;
    
    float z = -10f;
    float smoothingTime = 0.2f;
    Vector3 __currentVelocity;

    void Start()
    {
        if (!pixCamera)
        {
            pixCamera = GetComponent<PixelPerfectCamera>();
        }
    }

    void Update()
    {
        if (target)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, z);
            pixCamera.pixelSnapping = targetPosition == transform.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref __currentVelocity, smoothingTime);
        }
    }
}
