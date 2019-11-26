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

    private Collider grabbedCollider;
    // Update is called once per frame
    void Update()
    {
        leftGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.LeftHand);
        rightGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.RightHand);

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
}
