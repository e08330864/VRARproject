using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Leap.Unity;

// This script defines conditions that are necessary for the Leap player to grab a shared object

public class LeapGrab : MonoBehaviour 
{
    private Actor actor = null;
    private PinchDetector pinchDetectorLeft = null;
    private Vector3 pinchPosition;
    //private PinchDetector pinchDetectorRight;

    [HideInInspector]
    public Collider leftTouch, rightTouch;
    [HideInInspector]
    public bool leftPinch, rightPinch;

    [SerializeField]
    private List<GameObject> objectPrefab = null;

    private Collider colliderLeap = null;
    private bool isInCreation = false;
    private bool doObjectSwitching = false;
    private bool createNewObject = false;
    private int objectIndex = 0;
    private GameObject createdObject = null;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("LeftHandInteraction") != null) {
            pinchDetectorLeft = GameObject.FindGameObjectWithTag("LeftHandInteraction").GetComponent<PinchDetector>();
        }
        if ((actor = GetComponent<LocalPlayerController>().actor) == null)
        {
            Debug.LogError("actor is NULL in LeapGrab");
        }
    }

    // Update is called once per frame
    void Update()
    { 
        //if (leftTouch != null && leftTouch == rightTouch && rightPinch && leftPinch)
        //{
        //    if (colliderLeap == null)
        //    {
        //        colliderLeap = leftTouch;
        //        leftTouch.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = true;
        //    }
        //}
        //else if (colliderLeap != null && (!rightPinch || !leftPinch))
        //{
        //    colliderLeap.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = false;
        //    colliderLeap = null;
        //}
        
        CheckHoldingObject();
        CheckCreateObject();
    }

    private void CheckCreateObject()
    {
        if (createdObject != null && !isInCreation)
        {
            actor.CmdCreateObject(createdObject);
            createdObject = null;
        }
        createNewObject = false;
        if (!isInCreation && colliderLeap == null)   // only when no object is currently held and no creation process is currently running
        {
            if (leftPinch)
            {
                isInCreation = true;
                objectIndex = 0;
                createNewObject = true;
            }
        }
        else
        {
            if (isInCreation)   // creation process was running in last frame
            {
                if (!leftPinch)  // end of creation process
                {
                    isInCreation = false;
                }
                else    // creation process is further on running
                {
                    if (rightPinch && !doObjectSwitching)    // switch to next object
                    {
                        doObjectSwitching = true;
                        createNewObject = true;
                        objectIndex = (++objectIndex) % objectPrefab.Count;
                    } else if (!rightPinch)     // new switching enabled
                    {
                        doObjectSwitching = false;
                    }
                }
            }
        }
        if (createNewObject)
        {
            // destroy current object if exists
            if (createdObject != null)
            {
                Destroy(createdObject);
            }
            // create new object
            createdObject = (GameObject)Instantiate(objectPrefab[objectIndex]);
        }
        if (isInCreation && createdObject != null)
        {
            Vector3 leftPos = pinchPosition;
            createdObject.transform.position = leftPos;
        }
    }

    private void CheckHoldingObject()
    {
        if (leftTouch != null && leftTouch == rightTouch)
        {
            if (colliderLeap == null)
            {
                colliderLeap = leftTouch;
                leftTouch.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = true;
            }
        }
        else if (colliderLeap != null)
        {
            colliderLeap.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = false;
            colliderLeap = null;
        }
    }

    public void onPinchLeft()
    {
        if (pinchDetectorLeft == null && GameObject.FindGameObjectWithTag("LeftHandInteraction") != null)
        {
            pinchDetectorLeft = GameObject.FindGameObjectWithTag("LeftHandInteraction").GetComponent<PinchDetector>();
        }
        pinchPosition = pinchDetectorLeft.Position;
        Debug.Log("Left Pinch");
        leftPinch = true;
    }
    public void offPinchLeft()
    {
        leftPinch = false;
    }
    public void onPinchRight()
    {
        Debug.Log("Right Pinch");
        rightPinch = true;
    }
    public void offPinchRight()
    {
        rightPinch = false;
    }
}
