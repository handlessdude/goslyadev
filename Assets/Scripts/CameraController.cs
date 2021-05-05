using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System.Linq;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    [HideInInspector]
    public PixelPerfectCamera pixCamera;
    public Transform cameraBoundsParent;
    public ScreenFader screenFader;
    Vector2 inboundTarget;
    public AudioSource phoneRing;
    SaveSystem Load;
    List<PolygonCollider2D> cameraBounds;
    PolygonCollider2D currentCollider;

    Vector2 rightUpperViewBorder;
    Vector2 leftDownViewBorder;

    int resolutionX = 0;
    int resolutionY = 0;

    int refResX = 640;
    int refResY = 360;

    static int boundX = 840;
    static int boundY = 360;

    bool zoomedOut = true;
    int zoomedRefResX = 448;
    int zoomedRefResY = 252;

    float z = -10f;
    float smoothingTime = 0.2f;
    Vector3 __currentVelocity;

    void Start()
    {
        if (!Load)
        {
            Load = GetComponent<SaveSystem>();
        }    
        if (!pixCamera)
        {
            pixCamera = GetComponent<PixelPerfectCamera>();
        }

        if (cameraBoundsParent)
        {
            cameraBounds = new List<PolygonCollider2D>();
            foreach (Transform gO in cameraBoundsParent)
            {
                cameraBounds.Add(gO.GetComponent<PolygonCollider2D>());
            }
            currentCollider = cameraBounds[0];
        } 

        if (screenFader)
        {
            screenFader.fadeState = ScreenFader.FadeState.OutEnd;
            screenFader.fadeState = ScreenFader.FadeState.Out;
        }

        if (SceneManager.GetSceneByBuildIndex(2).Equals(SceneManager.GetActiveScene()))
        {
            if (!GameplayState.isStartedDialogEnded)
                GameplayState.controllability = PlayerControllability.FirstDialog;
        }
    }

    public bool ZoomedOut()
    {
        return zoomedOut;
    }

    void FixedUpdate()
    {

    }


    void Update()
    {
		(keybinding menu added)
        if (phoneRing)
        {
            if (GameplayState.controllability == PlayerControllability.FirstDialog)
            {
                phoneRing.mute = false;
            }
            else
                phoneRing.mute = true;
        }  

        if (target)
        {
            //интерполируем к точке на периметре коллайдера-ограничителя
            Vector2 localTarget = currentCollider.ClosestPoint(target.transform.position);
            if (!InViewBorders(localTarget, target.transform.position))
            {
                UpdateCurrentCollider();
                localTarget = currentCollider.ClosestPoint(target.transform.position);
                transform.position = new Vector3(localTarget.x, localTarget.y, z);
            }
            else
            {
                Vector3 targetPosition = new Vector3(localTarget.x, localTarget.y, z);
                pixCamera.pixelSnapping = targetPosition == transform.position;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref __currentVelocity, smoothingTime);
            }
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

        UpdateViewBorders();
    }

    void UpdateViewBorders()
    {
        rightUpperViewBorder = new Vector2((float)refResX / pixCamera.assetsPPU / 2, (float) refResY / pixCamera.assetsPPU / 2);
        leftDownViewBorder = -rightUpperViewBorder;
    }

    bool InViewBorders(Vector2 inboundTargetPos, Vector2 targetPos)
    {
        return (inboundTargetPos.x + rightUpperViewBorder.x - targetPos.x > 0)
            && (inboundTargetPos.x - rightUpperViewBorder.x - targetPos.x < 0)
            && (inboundTargetPos.y + rightUpperViewBorder.y - targetPos.y > 0)
            && (inboundTargetPos.y - rightUpperViewBorder.y - targetPos.y < 0);
    }

    void UpdateCurrentCollider()
    {
        float min = float.MaxValue;
        foreach (PolygonCollider2D pc in cameraBounds)
        {
            float t = Vector2.SqrMagnitude(pc.ClosestPoint(target.transform.position) - (Vector2)target.transform.position);
            if (t < min)
            {
                currentCollider = pc;
                min = t;
            }
        }
    }


    public void ZoomIn()
    {
        zoomedOut = false;
        pixCamera.refResolutionX = zoomedRefResX;
        pixCamera.refResolutionY = zoomedRefResY;
    }

    public void ZoomOut()
    {
        zoomedOut = true;
        pixCamera.refResolutionX = refResX;
        pixCamera.refResolutionY = refResY;
    }
}
