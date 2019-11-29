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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            allowGameSpaceExtension = true;
            sharedParameters.AllowGameSpaceExtension();
        }
        else
        {
            allowGameSpaceExtension = false;
            sharedParameters.DenyGameSpaceExtension();
        }
    }
}
