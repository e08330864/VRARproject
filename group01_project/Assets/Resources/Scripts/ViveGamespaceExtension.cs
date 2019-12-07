using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveGamespaceExtension : MonoBehaviour
{
    [HideInInspector]
    public bool rightGrabPinch;
    private bool lastRightGrabPinch;

    Transform rightViveController = null;
    Vector3? rightControllerPositionStart = null;
    float gameSpaceExtensionSpeed = 0.01f;

    bool thisFrameExtensionPossible = false;
    bool lastFrameExtensionPossible = false;

    public SharedParameters sharedParameters;
    public ViveParamsAuthorityManager viveParamsAuthorityManager;
    private SteamVR_PlayArea playArea;
    bool GameSpaceMeasuresInitialized = false;


    // Start is called before the first frame update
    void Start()
    {
        playArea = GetComponent<SteamVR_PlayArea>();

        rightViveController = GameObject.Find("Controller (right)").transform;

        //set Leap Player to center of PlaySpace
        viveParamsAuthorityManager.SetPosition(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        var rect = new Valve.VR.HmdQuad_t();
        bool success = OpenVR.Chaperone.GetPlayAreaRect(ref rect);

        //initialize playspace measures and send them to leap
        if (success && !GameSpaceMeasuresInitialized)
        { 
            float[] x_values = { rect.vCorners0.v0, rect.vCorners1.v0, rect.vCorners2.v0, rect.vCorners3.v0 };
            float max_x = Mathf.Max(x_values);
            float min_x = Mathf.Min(x_values);

            /*GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere1.transform.parent = transform;
            sphere1.transform.localPosition = new Vector3(rect.vCorners0.v0, rect.vCorners0.v1, rect.vCorners0.v2);

            GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere2.transform.parent = transform;
            sphere2.transform.localPosition = new Vector3(rect.vCorners1.v0, rect.vCorners1.v1, rect.vCorners1.v2);

            GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere3.transform.parent = transform;
            sphere3.transform.localPosition = new Vector3(rect.vCorners2.v0, rect.vCorners2.v1, rect.vCorners2.v2);

            GameObject sphere4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere4.transform.parent = transform;
            sphere4.transform.localPosition = new Vector3(rect.vCorners3.v0, rect.vCorners3.v1, rect.vCorners3.v2);*/

            float[] z_values = { rect.vCorners0.v2, rect.vCorners1.v2, rect.vCorners2.v2, rect.vCorners3.v2 };
            float max_z = Mathf.Max(z_values);
            float min_z = Mathf.Min(z_values);

            Vector2 measures = new Vector2(max_x - min_x, max_z - min_z);

            //initialize playspace measures
            viveParamsAuthorityManager.SetPlaySpaceMeasures(measures);
            GameSpaceMeasuresInitialized = true;
        }

        lastRightGrabPinch = rightGrabPinch;
        rightGrabPinch = SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.RightHand);

        //game space extension
        Vector3 shift = calculateGameSpaceShift();
        

        //Debug.Log("Gamespace Extension Possible: " + sharedParameters.GameSpaceExtensionPossible());
        //ask if Leap Player also allows game space extension
        if (sharedParameters.GameSpaceExtensionPossible())
        {
            thisFrameExtensionPossible = true;
            transform.position += new Vector3(shift.x, 0.0f, shift.z);
        }
        else
        {
            thisFrameExtensionPossible = false;
        }

        //currently stopped extending game space
        if(!thisFrameExtensionPossible && lastFrameExtensionPossible)
        {
            viveParamsAuthorityManager.SetPosition(transform.position);
        }
        lastFrameExtensionPossible = thisFrameExtensionPossible;
    }

    Vector3 calculateGameSpaceShift()
    {
        Vector3 shift = new Vector3(0.0f, 0.0f, 0.0f);


        //right Hand Controller started Grab Pinch
        if (!lastRightGrabPinch && rightGrabPinch)
        {
            rightControllerPositionStart = rightViveController.position;

        }

        //right hands continus grab Pinch
        if (lastRightGrabPinch && rightGrabPinch)
        {
            if (rightControllerPositionStart.HasValue)
            {
                shift = rightViveController.position - rightControllerPositionStart.Value;
            }
            else
            {
                Debug.Log("Trying to calculate pinch, but not pinch start operation yet detected");
            }
        }

        //right hand stopped grab pinching
        if (lastRightGrabPinch && !rightGrabPinch)
        {
            shift = new Vector3(0.0f, 0.0f, 0.0f);
            rightControllerPositionStart = null;
        }

        return gameSpaceExtensionSpeed * shift;
    }
}
