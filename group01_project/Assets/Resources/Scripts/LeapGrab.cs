using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

// This script defines conditions that are necessary for the Leap player to grab a shared object

public class LeapGrab : MonoBehaviour 
{
    [HideInInspector]
    public Collider leftTouch, rightTouch;
    [HideInInspector]
    public bool leftPinch, rightPinch;

    [SerializeField]
    private List<GameObject> objectPrefab = null;

    private Collider colliderLeap = null;
    private bool isInCreation = false;
    private bool objectSwitchingEnabled = true;
    private bool createNewObject = false;
    private int objectIndex = 0;
    private GameObject createdObject = null;

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
    }

    private void CheckCreateObject()
    {
        if (!isInCreation && colliderLeap == null)   // only when no object is currently be holded and no creation process is currently running
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
                    if (rightPinch && objectSwitchingEnabled)    // switch to next object
                    {
                        objectSwitchingEnabled = false;
                        createNewObject = true;
                        objectIndex = (++objectIndex) % objectPrefab.Count;
                    } else if (!rightPinch)     // new switching enabled
                    {
                        objectSwitchingEnabled = true;
                    }
                }
            }
        }
        CreateObject();
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

    [Command]
    void CreateObject()
    {
        if (createNewObject)
        {
            // destroy current object if exists
            if (createdObject != null)
            {
                Destroy(createdObject);
            }
            // create new object
            createdObject = (GameObject)Instantiate(objectPrefab[objectIndex]);
            // spawn the object on the clients
            NetworkServer.Spawn(createdObject);
        }
        createNewObject = false;
    }
}
