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
    bool zoomedOut = true;
    ZoomState zoomState = ZoomState.None;

    enum ZoomState
    {
        None,
        ZoomingIn,
        ZoomingOut
    }

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

        if (InputManager.GetKeyDown(KeyAction.CameraScale))
        {
            if (zoomedOut)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        }
        

        if (zoomState == ZoomState.ZoomingIn)
        {
            if (pixCamera.refResolutionX > 448)
            {
                pixCamera.refResolutionX -= 16;
                pixCamera.refResolutionY -= 9;
            }
            else
            {
                zoomState = ZoomState.None;
            }
        }
        else if (zoomState == ZoomState.ZoomingOut)
        {
            if (pixCamera.refResolutionX < 640)
            {
                pixCamera.refResolutionX += 16;
                pixCamera.refResolutionY += 9;
            }
            else
            {
                zoomState = ZoomState.None;
            }
        }
    }

    void FixedUpdate()
    {
        
    }

    public void ZoomIn()
    {
        zoomedOut = false;
        zoomState = ZoomState.ZoomingIn;
    }

    public void ZoomOut()
    {
        zoomedOut = true;
        zoomState = ZoomState.ZoomingOut;
    }
}
