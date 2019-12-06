using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class LeapGestures : MonoBehaviour
{
    private CapsuleHand capsuleHandLeft = null;
    private CapsuleHand capsuleHandRight = null;
    private Hand handLeft = null;
    private Hand handRight = null;

    // Start is called before the first frame update
    void Start()
    {
        if ((capsuleHandLeft = GameObject.FindGameObjectWithTag("LeftHandInteraction").GetComponent<CapsuleHand>()) == null)
        {
            Debug.LogError("capsuleHandLeft is NULL in LeapGesture");
        }
        if ((handLeft = capsuleHandLeft.GetLeapHand()) == null)
        {
            Debug.LogError("handLeft is NULL in LeapGesture");
        }
        if ((capsuleHandRight = GameObject.FindGameObjectWithTag("RightHandInteraction").GetComponent<CapsuleHand>()) == null)
        {
            Debug.LogError("capsuleHandRight is NULL in LeapGesture");
        }
        if ((handRight = capsuleHandRight.GetLeapHand()) == null)
        {
            Debug.LogError("handRight is NULL in LeapGesture");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // returns  0 = no pistol
    //          1 = pistol index finger ray untriggered
    //          2 = pistol index finger ray triggered
    //          3 = pistol palm ray untriggered
    //          4 = pistol palm ray triggered
    private int CheckPistolLeft()
    {
        int isPistol = 1;
        for (int f = 0; f < handLeft.Fingers.Count; f++)
        {
            Finger finger = handLeft.Fingers[f];
            switch (f)
            {
                case 0: // thump
                    if (!finger.IsExtended)
                        isPistol = 2;
                    break;
                case 1: // index
                    if (!finger.IsExtended)
                        isPistol = 0;
                    break;
                case 2: // middle
                case 3: // ring
                case 4: // small
                    if (finger.IsExtended)
                        isPistol = 0;
                    break;
            }
        }
        return isPistol;
    }
}
