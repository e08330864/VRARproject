using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapGameSpaceExtension : MonoBehaviour
{
    public SharedParameters sharedParameters;
    private bool allowGameSpaceExtension = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("Allow Gamespace extension from leap side");
            allowGameSpaceExtension = true;
            sharedParameters.CmdSetGameSpaceExtension(allowGameSpaceExtension);
        }
        else
        {
            Debug.Log("Deny Gamespace extension from leap side");
            allowGameSpaceExtension = false;
            sharedParameters.CmdSetGameSpaceExtension(allowGameSpaceExtension);
        }
    }
}
