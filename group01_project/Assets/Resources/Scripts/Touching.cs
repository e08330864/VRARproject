using UnityEngine;

// TODO: this script CAN be used to detect the events of a left networked hand touching a shared object
// fill in the implementation and communicate touching events to either LeapGrab and ViveGrab by setting the rightHandTouching variable
// ALTERNATIVELY, implement the verification of the grabbing conditions in a way  your prefer
// TO REMEMBER: only the localPlayer (networked hands belonging to the localPlayer) should be able to "touch" shared objects

public class Touching : MonoBehaviour 
{
    ViveGrab viveGrab = null;
    LeapGrab leapGrab = null;

    void Start()
    {
        if (GameObject.Find("Leap") != null)
        {
            leapGrab = FindObjectOfType<LeapGrab>();
        }

        if (GameObject.Find("VIVE") != null)
        {
            viveGrab = FindObjectOfType<ViveGrab>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (leapGrab != null)
        {
            if (this.gameObject.tag == "left")
            {
                leapGrab.leftTouch = other;
            }
            else if (this.gameObject.tag == "right")
            {
                leapGrab.rightTouch = other;
            }
        } 
        else if (viveGrab != null)
        {
            if (this.gameObject.tag == "left")
            {
                viveGrab.leftTouch = other;
            }
            else if (this.gameObject.tag == "right")
            {
                viveGrab.rightTouch = other;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (leapGrab != null)
        {
            if (this.gameObject.tag == "left")
            {
                leapGrab.leftTouch = null;
            }
            else if (this.gameObject.tag == "right")
            {
                leapGrab.rightTouch = null;
            }
        }
        else if (viveGrab != null)
        {
            if (this.gameObject.tag == "left")
            {
                viveGrab.leftTouch = null;
            }
            else if (this.gameObject.tag == "right")
            {
                viveGrab.rightTouch = null;
            }
        }
    }

}
