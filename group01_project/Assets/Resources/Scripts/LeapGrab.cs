﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Leap.Unity;

// This script defines conditions that are necessary for the Leap player to grab a shared object

public class LeapGrab : MonoBehaviour 
{
    private Actor actor = null;
    private Transform playerTransform = null;
    private PinchDetector pinchDetectorLeft = null;
    private PinchDetector pinchDetectorRight = null;
    private Vector3 pinchPositionLeft;
    private Vector3 pinchPositionRight;

    [HideInInspector]
    public Collider leftTouchOtherCollider, rightTouchOtherCollider;
    [HideInInspector]
    public bool leftPinch, rightPinch;

    [SerializeField]
    private List<GameObject> objectPrefab = null;
    [SerializeField]
    private float objectScale = 1.0f;

    private Collider holdingObjectCollider = null;
    private bool isInCreation = false;
    private bool objectSwitchingEnabled = false;
    private int objectIndex = 0;
    
    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (actor == null && (playerTransform = gameObject.transform.Find("Player")) != null)
        {
            actor = playerTransform.GetComponent<Actor>();
            Debug.Log("LeapGrab: Actor Initialized");
        }

        if (pinchDetectorLeft == null)
        {
            leftPinch = false;
        }
        if (pinchDetectorRight == null)
        {
            rightPinch = false;
        }

        CheckCreateObject();
        CheckHoldingObject();
    }

    //---------------------------------------------------------------------------------------------------------------------------
    // CREATING
    private void CheckCreateObject()
    {
        if (!isInCreation) // currently not in creation mode
        {
            if (holdingObjectCollider == null && rightPinch && leftPinch)    // currently not in creation mode --> start creating new object when l+r-pinch and no object is touched
            {
                isInCreation = true;
                objectIndex = 0;
                objectSwitchingEnabled = false;
                // new object is created on left hand pinch-position --> grapped by left hand afterwards
                string prefabName = objectPrefab[objectIndex].name;
                Debug.Log("LeapGrab: CreateObject calling for..." + prefabName);
                actor.CreateObject(prefabName, pinchPositionLeft, objectScale);
            }
        }
        else    // currently in creation mode
        {
            if (!leftPinch)   // stop creation mode
            {
                Debug.Log("LeapGrab: STOP creation mode ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                isInCreation = false;
            }
            else    // creation process is further on running
            {
                if (rightPinch && objectSwitchingEnabled)    // switch to next object: old object is destroyed
                {
                    objectSwitchingEnabled = false;
                    actor.CmdDestroyObject();
                    objectIndex = (++objectIndex) % objectPrefab.Count;
                    string prefabName = objectPrefab[objectIndex].name;
                    Debug.Log("LeapGrab: SWITCHING: CreateObject calling for..." + prefabName);
                    actor.CreateObject(prefabName, pinchPositionLeft, objectScale);
                }
                else if (!rightPinch)     // new switching enabled
                {
                    Debug.Log("LeapGrab: left pinch = false");
                    objectSwitchingEnabled = true;
                }
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------
    // HOLDING
    private void CheckHoldingObject()
    {
        if ((leftTouchOtherCollider != null && leftPinch))   // grab with left hand (1.priority)
        {
            if (holdingObjectCollider == null)
            {
                Debug.Log("LeapGrab: Holding with LEFT hand");
                holdingObjectCollider = leftTouchOtherCollider;
                holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().SetLeftGrabbedNew(true);  // sets leftGrabbed=true in AuthorityManager, only in case when not already grabbed
                holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().playerGrabs = true;
            }
        }
        else
        {
            if ((rightTouchOtherCollider != null && rightPinch))    // grab with right hand (2.priority)
            { 
                if (holdingObjectCollider == null)
                {
                    Debug.Log("LeapGrab: Holding with RIGHT hand");
                    holdingObjectCollider = rightTouchOtherCollider;
                    holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().SetLeftGrabbedNew(false);    // sets leftGrabbed=false in AuthorityManager, only in case when not already grabbed
                    holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().playerGrabs = true;
                }
            }
            else   
            {
                if (holdingObjectCollider != null)   // release if no grab
                {
                    Debug.Log("LeapGrab: Holding RELEASED");
                    holdingObjectCollider.gameObject.GetComponent<AuthorityManager>().playerGrabs = false;
                    holdingObjectCollider = null;
                }
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------
    // PINCHING
    public void onPinchLeft()
    {
        GameObject hand = null;
        if (pinchDetectorLeft == null && (hand = GameObject.FindGameObjectWithTag("LeftHandInteraction")) != null)
        {
            pinchDetectorLeft = hand.GetComponent<PinchDetector>();
        }
        pinchPositionLeft = pinchDetectorLeft.Position;
        Debug.Log("LeapGrab: Left Pinch with position = " + pinchPositionLeft);
        leftPinch = true;
    }

    public void offPinchLeft()
    {
        Debug.Log("LeapGrab: left pinch = false");
        leftPinch = false;
    }

    public void onPinchRight()
    {
        GameObject hand = null;
        if (pinchDetectorRight == null && (hand = GameObject.FindGameObjectWithTag("RightHandInteraction")) != null)
        {
            pinchDetectorRight = hand.GetComponent<PinchDetector>();
        }
        pinchPositionRight = pinchDetectorRight.Position;
        Debug.Log("LeapGrab: Right Pinch with position = " + pinchPositionRight);
        rightPinch = true;
    }

    public void offPinchRight()
    {
        Debug.Log("LeapGrab: right pinch = false");
        rightPinch = false;
    }
}
