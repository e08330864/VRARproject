using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveGamespaceExtension : MonoBehaviour
{
    [HideInInspector]
    public bool rightGrabPinch;
    private bool lastRightGrabPinch;

    Transform rightViveController = null;
    Vector3? rightControllerPositionStart = null;
    float gameSpaceExtensionSpeed = 0.01f;

    bool thisFrameExtensionPossible = false;
    bool lastFrameExtensionPossible = false;

    public SharedParameters sharedParameters;
    public ViveParamsAuthorityManager viveParamsAuthorityManager;
    private SteamVR_PlayArea playArea;
    bool GameSpaceMeasuresInitialized = false;


    // Start is called before the first frame update
    void Start()
    {
        playArea = GetComponent<SteamVR_PlayArea>();

        rightViveController = GameObject.Find("Controller (right)").transform;

        //set Leap Player to center of PlaySpace
        viveParamsAuthorityManager.SetPosition(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3? gameSpaceMeasures = playArea.GetGameSpaceMeasures();
        if (gameSpaceMeasures.HasValue && !GameSpaceMeasuresInitialized)
        {
            Debug.Log("INITIALIZE MEASURES: " + gameSpaceMeasures.Value.ToString());
            viveParamsAuthorityManager.SetPlaySpaceMeasures(gameSpaceMeasures.Value);
            GameSpaceMeasuresInitialized = true;
        }*/

        lastRightGrabPinch = rightGrabPinch;
        rightGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.RightHand);

        //game space extension
        Vector3 shift = calculateGameSpaceShift();
        

        //Debug.Log("Gamespace Extension Possible: " + sharedParameters.GameSpaceExtensionPossible());
        //ask if Leap Player also allows game space extension
        if (sharedParameters.GameSpaceExtensionPossible())
        {
            thisFrameExtensionPossible = true;
            transform.position += new Vector3(shift.x, 0.0f, shift.z);
        }
        else
        {
            thisFrameExtensionPossible = false;
        }

        //currently stopped extending game space
        if(!thisFrameExtensionPossible && lastFrameExtensionPossible)
        {
            viveParamsAuthorityManager.SetPosition(transform.position);
        }
        lastFrameExtensionPossible = thisFrameExtensionPossible;
    }

    Vector3 calculateGameSpaceShift()
    {
        Vector3 shift = new Vector3(0.0f, 0.0f, 0.0f);


        //right Hand Controller started Grab Pinch
        if (!lastRightGrabPinch && rightGrabPinch)
        {
            rightControllerPositionStart = rightViveController.position;

        }

        //right hands continus grab Pinch
        if (lastRightGrabPinch && rightGrabPinch)
        {
            if (rightControllerPositionStart.HasValue)
            {
                shift = rightViveController.position - rightControllerPositionStart.Value;
            }
            else
            {
                Debug.Log("Trying to calculate pinch, but not pinch start operation yet detected");
            }
        }

        //right hand stopped grab pinching
        if (lastRightGrabPinch && !rightGrabPinch)
        {
            shift = new Vector3(0.0f, 0.0f, 0.0f);
            rightControllerPositionStart = null;
        }

        return gameSpaceExtensionSpeed * shift;
    }
}
