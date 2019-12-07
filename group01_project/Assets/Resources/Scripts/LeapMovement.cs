using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

/// <summary>
/// the movement of the leap player works like this:
/// - for starting movement
/// </summary>
public class LeapMovement : MonoBehaviour
{
    public float movmentStepDistance = 1f; 
    private CapsuleHand capsuleHandLeft = null;
    private CapsuleHand capsuleHandRight = null;
    private Hand handLeft = null;
    private Hand handRight = null;

    private int gestureStatus = 0;      // 0...at least one hand with no pistol
                                        // 1...both hands pistol untriggered
                                        // 2...both hands pistol triggered
    private bool doMove = false;


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
        CheckMovement();
        if (doMove)
        {
            DoMovement();
        }
    }

    private void DoMovement()
    {
        Vector3 direction = UnityVectorExtension.ToVector3(handLeft.Fingers[1].Direction + handRight.Fingers[1].Direction).normalized;

    }

    private void CheckMovement()
    {
        int leftPG = CheckPistol(handLeft);
        int rightPG = CheckPistol(handRight);
        switch (gestureStatus)
        {
            case 0: // at least one hand with no pistol
                if (leftPG == 1 && rightPG == 1)
                {
                    gestureStatus = 1;
                }
                if (leftPG == 2 && rightPG == 2)
                {
                    gestureStatus = 2;
                }
                break;
            case 1: // both hands pistol untriggered
                if (leftPG == 0 || rightPG == 0)
                {
                    gestureStatus = 0;
                }
                if (leftPG == 2 && rightPG == 2)
                {
                    gestureStatus = 2;
                    doMove = true;
                }
                break;
            case 2: // 2...both hands pistol triggered
                if (leftPG == 0 || rightPG == 0)
                {
                    gestureStatus = 0;
                }
                if (leftPG == 1 && rightPG == 1)
                {
                    gestureStatus = 1;
                    doMove = true;
                }
                break;
        }
    }

    // returns  0 = no pistol
    //          1 = pistol thumb finger untriggered
    //          2 = pistol thumb finger triggered
    private int CheckPistol(Hand hand)
    {
        int isPistol = 1;
        for (int f = 0; f < hand.Fingers.Count; f++)
        {
            Finger finger = hand.Fingers[f];
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
