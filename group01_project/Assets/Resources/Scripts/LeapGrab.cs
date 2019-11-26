using UnityEngine;

// This script defines conditions that are necessary for the Leap player to grab a shared object
// TODO: values of these four boolean variables can be changed either directly here or through other components
// AuthorityManager of a shared object should be notifyed from this script

public class LeapGrab : MonoBehaviour 
{
    [HideInInspector]
    public Collider leftTouch, rightTouch;
    [HideInInspector]
    public bool leftPinch, rightPinch;

    private Collider collider;

    // Update is called once per frame
    void Update()
    {
        if (leftTouch != null && leftTouch == rightTouch && rightPinch && leftPinch)
        {
            if (collider == null)
            {
                collider = leftTouch;
                leftTouch.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = true;
            }
        }
        else if (collider != null && (!rightPinch || !leftPinch))
        {
            collider.gameObject.GetComponent<AuthorityManager>().grabbedByPlayer = false;
            collider = null;
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
}
