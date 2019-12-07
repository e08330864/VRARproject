using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

/// <summary>
/// the movement of the leap player works like this:
/// - for starting movement both hand must make pistol gesture
/// - afterwards, at one and only one hand by triggering the thumb, the leap player is rotating: 
///         right thumb triggered --> turn right
///         left thumb triggered --> turn left
/// - if both thumbs are triggered simultaniously, the leap player is moving in viewing direction
/// </summary>
public class LeapMovement : MonoBehaviour
{
    [SerializeField]
    private SharedParameters sharedParameters = null;
    private LeapGameSpaceExtension gameSpaceExtension = null;

    public float movementDistancePerSecond = 2f;
    public float rotationAnglePerSecond = 30f;
    private CapsuleHand capsuleHandLeft = null;
    private CapsuleHand capsuleHandRight = null;
    private GameObject leftHandModel = null;
    private GameObject rightHandModel = null;
    private Hand handLeft = null;
    private Hand handRight = null;

    private bool allowGameSpaceExtension = false;
    private bool isFistThumbUp = false;

    private int gestureStatus = 0;      // 0...at least one hand with no pistol
                                        // 1...both hands pistol untriggered
                                        // 2...both hands pistol triggered --> movement in viewing direction
                                        // 3...left hand with pistol triggerd and right hand not triggered --> turn left
                                        // 4...right hand with pistol triggerd and left hand not triggered --> turn right


    // Start is called before the first frame update
    void Start()
    {
        if ((gameSpaceExtension = GetComponent<LeapGameSpaceExtension>()) == null)
        {
            Debug.LogError("gameSpaceExtension is NULL in LeapMovement");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (handLeft == null)
        {
            leftHandModel = GameObject.FindGameObjectWithTag("LeftHandInteraction");
            if (leftHandModel != null)
            {
                if ((capsuleHandLeft = leftHandModel.GetComponent<CapsuleHand>()) != null)
                {
                    handLeft = capsuleHandLeft.GetLeapHand();
                    if (handLeft.IsRight)
                    {
                        handRight = handLeft;
                        handLeft = null;
                    }
                }
            }
        }
        if (handRight == null)
        {
            rightHandModel = GameObject.FindGameObjectWithTag("RightHandInteraction");
            if (rightHandModel != null)
            {
                if ((capsuleHandRight = rightHandModel.GetComponent<CapsuleHand>()) != null)
                {
                    handRight = capsuleHandRight.GetLeapHand();
                    if (handRight.IsLeft)
                    {
                        handLeft = handRight;
                        handRight = null;
                    }
                    if (handRight == handLeft)
                    {
                        handRight = null;
                    }
                }
            }
        }
        // movement and rotation
        if (handLeft != null && handRight != null)
        {    
            CheckMovementRotation();
            DoMovementRotation();
        }
        // allow game space extension by right hand fist gesture
        if (gameSpaceExtension != null && handRight != null)
        {
            isFistThumbUp = CheckFistThumbUp(handRight);
        } else if (handRight == null && allowGameSpaceExtension)
        {
            isFistThumbUp = false;
        }
        if (gameSpaceExtension != null)
        { 
            Debug.Log("LeapMovement: isFist = " + isFistThumbUp);
            if (isFistThumbUp && !allowGameSpaceExtension)
            {
                gameSpaceExtension.SetAllowGameSpaceExtension(true);
                allowGameSpaceExtension = true;
            }
            if (!isFistThumbUp && allowGameSpaceExtension)
            {
                gameSpaceExtension.SetAllowGameSpaceExtension(false);
                allowGameSpaceExtension = false;
            }
        }
    }

    private void CheckMovementRotation()
    {
        int leftPG = CheckPistol(handLeft);
        int rightPG = CheckPistol(handRight);
        Debug.Log("LeapMovement: leftPG=" + leftPG + "  , rightPG=" + rightPG);
        if (leftPG == 2 && rightPG == 2) { gestureStatus = 2; return; }
        if (leftPG == 2 && rightPG == 1) { gestureStatus = 3; return; }
        if (leftPG == 1 && rightPG == 2) { gestureStatus = 4; return; }
        gestureStatus = 0;
    }

    private void DoMovementRotation()
    {
        Debug.Log("LeapMovement: gestureStatus=" + gestureStatus);
        switch (gestureStatus)
        {
            case 2: // forward movement
                // limit to game space extension
                Vector3 newPos = transform.position + transform.forward * movementDistancePerSecond * Time.deltaTime;
                if (sharedParameters != null && sharedParameters.GetPlaySpaceMeasures() != Vector2.zero)
                {
                    Debug.Log("space-dim=" + sharedParameters.GetPlaySpaceMeasures() + "  space-center=" + sharedParameters.GetNewPosition() + "  new-pos=" + newPos);
                    if (newPos.x < sharedParameters.GetNewPosition().x - sharedParameters.GetPlaySpaceMeasures().x / 2.0f ||
                        newPos.x > sharedParameters.GetNewPosition().x + sharedParameters.GetPlaySpaceMeasures().x / 2.0f ||
                        newPos.z < sharedParameters.GetNewPosition().z - sharedParameters.GetPlaySpaceMeasures().y / 2.0f ||
                        newPos.z > sharedParameters.GetNewPosition().z + sharedParameters.GetPlaySpaceMeasures().y / 2.0f)
                    {
                        break;
                    }
                }
                transform.position = newPos;
                break;
            case 3: // turn left
                transform.Rotate(Vector3.up * rotationAnglePerSecond * Time.deltaTime);
                break;
            case 4: // turn right
                transform.Rotate(-Vector3.up * rotationAnglePerSecond * Time.deltaTime);
                break;
            default:
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

    // returns  true = fist with thumb up
    //          false = no fist with thumb up
    private bool CheckFistThumbUp(Hand hand)
    {
        bool isFist = true;
        for (int f = 0; f < hand.Fingers.Count; f++)
        {
            Finger finger = hand.Fingers[f];
            switch (f)
            {
                case 0: // thump
                    if (!finger.IsExtended)
                        isFist = false;
                    break;
                case 1: // index
                case 2: // middle
                case 3: // ring
                case 4: // small
                    if (finger.IsExtended)
                        isFist = false;
                    break;
            }
        }
        return isFist;
    }
}
