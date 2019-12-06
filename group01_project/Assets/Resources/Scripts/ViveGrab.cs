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
    public Collider leftTouchOtherCollider, rightTouchOtherCollider;
    [HideInInspector]
    public bool leftGrabPinch, rightGrabPinch;

    private Collider holdingObjectCollider = null;


    // Update is called once per frame
    void Update()
    {
        leftGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.LeftHand);
        rightGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.RightHand);

        CheckHoldingObject();
    }

    private void CheckHoldingObject()
    {
        if ((leftTouchOtherCollider != null && leftGrabPinch))   // grab with left hand (1.priority)
        {
            if (holdingObjectCollider == null)
            {
                Debug.Log("ViveGrab: Holding with LEFT hand");
                holdingObjectCollider = leftTouchOtherCollider;
                holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().SetLeftGrabbedNew(true);
                holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().playerGrabs = true;
            }
        }
        else
        {
            if ((rightTouchOtherCollider != null && rightGrabPinch))    // grab with right hand (2.priority)
            {
                if (holdingObjectCollider == null)
                {
                    Debug.Log("ViveGrab: Holding with RIGHT hand");
                    holdingObjectCollider = rightTouchOtherCollider;
                    holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().SetLeftGrabbedNew(false);
                    holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().playerGrabs = true;
                }
            }
            else
            {
                if (holdingObjectCollider != null)  // release if no grab
                {
                    Debug.Log("ViveGrab: Holding RELEASED");
                    holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().playerGrabs = false;
                    holdingObjectCollider = null;
                }
            }
        }
    }
}
