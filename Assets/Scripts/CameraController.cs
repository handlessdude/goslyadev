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
        int boundX = 840;
        int boundY = 360;

        Debug.Log(resolutionX + " " + resolutionY);

        bool hyperWide = (float)resolutionX / resolutionY >= (float)boundX / boundY;

        int ingamePixelMul = hyperWide ? (resolutionX / boundX + 1) : resolutionY / boundY + 1;

        bool isZoomMulSet = false;
        bool isMulSet = false;

        for (int i = resolutionX; i > 0; i--)
        {
            if (i % ingamePixelMul == 0 && !isMulSet)
            {
                refResX = i / ingamePixelMul;
                isMulSet = true;
            }
            else if (i % (ingamePixelMul + 1) == 0 && !isZoomMulSet)
            {
                zoomedRefResX = i / (ingamePixelMul + 1);
                isZoomMulSet = true;
            }

            if (isZoomMulSet && isMulSet)
            {
                break;
            }
        }

        isZoomMulSet = false;
        isMulSet = false;

        for (int i = resolutionY; i > 0; i--)
        {
            if (i % ingamePixelMul == 0 && !isMulSet)
            {
                refResY = i / ingamePixelMul;
                isMulSet = true;
            }
            else if (i % (ingamePixelMul + 1) == 0 && !isZoomMulSet)
            {
                zoomedRefResY = i / (ingamePixelMul + 1);
                isZoomMulSet = true;
            }

            if (isZoomMulSet && isMulSet)
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
