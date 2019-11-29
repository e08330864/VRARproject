using UnityEngine;
using Valve.VR;

// This script defines conditions that are necessary for the Vive player to grab a shared object
// TODO: values of these four boolean variables can be changed either directly here or through other components
// AuthorityManager of a shared object should be notifyed from this script

public class ViveGrab : MonoBehaviour
{
    // to communicate the fulfillment of grabbing conditions
    public SteamVR_Action_Boolean grabPinch;
    public SteamVR_Behaviour_Pose leftPose;
    public SteamVR_Behaviour_Pose rightPose;
   
    [HideInInspector]
    public Collider leftTouch, rightTouch;
    [HideInInspector]
    public bool leftGrabPinch, rightGrabPinch;
    private bool lastLeftGrabPinch, lastRightGrabPinch;

    private Collider grabbedCollider;

    Transform leftViveController = null;
    Transform rightViveController = null;
    Vector3? rightControllerPositionStart = null;
    float gameSpaceExtensionSpeed = 0.0001f;

    private void Start()
    {
        leftViveController = GameObject.Find("Controller (left)").transform;
        rightViveController = GameObject.Find("Controller (right)").transform;
    }


    // Update is called once per frame
    void Update()
    {
        lastLeftGrabPinch = leftGrabPinch;
        lastRightGrabPinch = rightGrabPinch;
        leftGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.LeftHand);
        rightGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.RightHand);

        //game space extension
        Vector3 shift = calculateGameSpaceShift();
        transform.position += new Vector3(shift.x, 0.0f, shift.z);

        if (leftTouch != null && leftTouch == rightTouch && rightGrabPinch && leftGrabPinch)
        {
            if (grabbedCollider == null)
            {
                grabbedCollider = leftTouch;
                leftTouch.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = true;
            }
        }
        else if (grabbedCollider != null && (!rightGrabPinch || !leftGrabPinch))
        {
            grabbedCollider.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = false;
            grabbedCollider = null;
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
        if(lastRightGrabPinch && !rightGrabPinch)
        {
            shift = new Vector3(0.0f, 0.0f, 0.0f);
            rightControllerPositionStart = null;
        }

        return gameSpaceExtensionSpeed * shift;
    }
}
