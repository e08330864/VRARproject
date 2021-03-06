﻿using UnityEngine;

public class Touching : MonoBehaviour 
{
    ViveGrab viveGrab = null;
    LeapGrab leapGrab = null;
    ArdManager ardManager = null;

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

        if (GameObject.Find("ArdManager") != null)
        {
            ardManager = FindObjectOfType<ArdManager>();
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
                    Debug.Log("Touching: Leap left touch START");
                    leapGrab.leftTouchOtherCollider = other;
                }
                else if (this.gameObject.tag == "right")
                {
                    Debug.Log("Touching: Leap right touch START");
                    leapGrab.rightTouchOtherCollider = other;
                }
            }
            else if (viveGrab != null)
            {
                if (this.gameObject.tag == "left")
                {
                    Debug.Log("Touching: Vive left touch START");
                    viveGrab.leftTouchOtherCollider = other;
                }
                else if (this.gameObject.tag == "right")
                {
                    Debug.Log("Touching: Vive right touch START");
                    viveGrab.rightTouchOtherCollider = other;
                }
            }
        }
        if (other.gameObject.tag == "Station")
        {
            Debug.Log("in Arduino setup collider");
            if (!ardManager.getSetupModeStatus())
            {
                Debug.Log("Arduino setup done!!!!");
                ardManager.startSetupMode(other.gameObject.transform.parent.parent.Find("Torus").gameObject);
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
                    Debug.Log("Touching: Leap left touch END");
                    leapGrab.leftTouchOtherCollider = null;
                }
                else if (this.gameObject.tag == "right")
                {
                    Debug.Log("Touching: Leap right touch END");
                    leapGrab.rightTouchOtherCollider = null;
                }
            }
            else if (viveGrab != null)
            {
                if (this.gameObject.tag == "left")
                {
                    Debug.Log("Touching: Vive left touch END");
                    viveGrab.leftTouchOtherCollider = null;
                }
                else if (this.gameObject.tag == "right")
                {
                    Debug.Log("Touching: Vive right touch END");
                    viveGrab.rightTouchOtherCollider = null;
                }
            }
        }
    }

}
