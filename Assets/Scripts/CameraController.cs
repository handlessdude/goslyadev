using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public PixelPerfectCamera pixCamera;

    bool zoomedOut = true;

    int resolutionX = 0;
    int resolutionY = 0;

    int refResX = 640;
    int refResY = 360;

    int zoomedRefResX = 448;
    int zoomedRefResY = 252;

    float z = -10f;
    float smoothingTime = 0.2f;
    Vector3 __currentVelocity;
    
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

    public bool ZoomedOut()
    {
        return zoomedOut;
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

        //точно есть способ обыграть это лучше, но пока так
        if ((resolutionX != Screen.width) || (resolutionY != Screen.height))
        {
            resolutionX = Screen.width;
            resolutionY = Screen.height;
            OnResolutionChanged();
        }
        
    }

    void OnResolutionChanged()
    {
        if (resolutionX % 640 == 0)
        {
            refResX = 640;
        }
        else
        {
            bool nSet = false;
            bool nPlusOneSet = false;
            int multiplier = (resolutionX / 640) + 1;
            for (int i = resolutionX; i > 0; i--)
            {
                if (i % multiplier == 0)
                {
                    refResX = i / multiplier;
                    nSet = true;
                }
                if (i % (multiplier + 1) == 0)
                {
                    zoomedRefResX = i / (multiplier + 1);
                    nPlusOneSet = true;
                }
                if (nPlusOneSet && nSet)
                {
                    break;
                }
            }

            nSet = false;
            nPlusOneSet = false;

            multiplier = (resolutionY / 360) + 1;
            for (int i = resolutionY; i > 0; i--)
            {
                if (i % multiplier == 0)
                {
                    refResY = i/multiplier;
                    nSet = true;
                }
                if (i % (multiplier + 1) == 0)
                {
                    zoomedRefResY = i / (multiplier + 1);
                    nPlusOneSet = true;
                }
                if (nPlusOneSet && nSet)
                {
                    break;
                }
            }

            if (zoomedOut)
            {
                pixCamera.refResolutionX = refResX;
                pixCamera.refResolutionY = refResY;
            }
            else
            {
                pixCamera.refResolutionX = zoomedRefResX;
                pixCamera.refResolutionY = zoomedRefResY;

            }
        }
    }

    void FixedUpdate()
    {
        
    }

    public void ZoomIn()
    {
        zoomedOut = false;
        pixCamera.refResolutionX = zoomedRefResX;
        pixCamera.refResolutionY = zoomedRefResY;
        //zoomState = ZoomState.ZoomingIn;
    }

    public void ZoomOut()
    {
        zoomedOut = true;
        pixCamera.refResolutionX = refResX;
        pixCamera.refResolutionY = refResY;
        //zoomState = ZoomState.ZoomingOut;
    }
}
