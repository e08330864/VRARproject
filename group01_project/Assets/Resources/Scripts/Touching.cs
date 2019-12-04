using UnityEngine;

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
        if (other.gameObject.tag == "TowerElement")
        {
            if (leapGrab != null)
            {
                if (this.gameObject.tag == "left")
                {
                    Debug.Log("Touching: Leap left touch");
                    leapGrab.leftTouchOtherCollider = other;
                }
                else if (this.gameObject.tag == "right")
                {
                    Debug.Log("Touching: Leap right touch");
                    leapGrab.rightTouchOtherCollider = other;
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TowerElement")
        {
            if (leapGrab != null)
            {
                if (this.gameObject.tag == "left")
                {
                    leapGrab.leftTouchOtherCollider = null;
                }
                else if (this.gameObject.tag == "right")
                {
                    leapGrab.rightTouchOtherCollider = null;
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

}
