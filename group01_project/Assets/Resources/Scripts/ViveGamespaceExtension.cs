using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveGamespaceExtension : MonoBehaviour
{
    public SharedParameters sharedParameters;

    [HideInInspector]
    public bool rightGrabPinch;
    private bool lastRightGrabPinch;

    Transform rightViveController = null;
    Vector3? rightControllerPositionStart = null;
    float gameSpaceExtensionSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        rightViveController = GameObject.Find("Controller (right)").transform;
    }

    // Update is called once per frame
    void Update()
    {
        lastRightGrabPinch = rightGrabPinch;
        rightGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.RightHand);

        //game space extension
        Vector3 shift = calculateGameSpaceShift();

        //Debug.Log("Gamespace Extension Possible: " + sharedParameters.GameSpaceExtensionPossible());
        //ask if Leap Player also allows game space extension
        if (sharedParameters.GameSpaceExtensionPossible())
        {
            transform.position += new Vector3(shift.x, 0.0f, shift.z);
        }
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
