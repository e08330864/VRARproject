using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LeapGameSpaceExtension: MonoBehaviour
{
    public ParametersAuthorityManager parametersAuthorityManager;
    public SharedParameters viveSharedScript;

    private bool allowGameSpaceExtension = false;
    private bool lastFrameallowGameSpaceExtension = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            allowGameSpaceExtension = !allowGameSpaceExtension;
            parametersAuthorityManager.SetGameSpaceExtensionPossible(allowGameSpaceExtension);
        }

        if(viveSharedScript != null)
        {
            //update center position of the leap player
            if(!allowGameSpaceExtension && lastFrameallowGameSpaceExtension)
            {
                transform.position = viveSharedScript.GetNewPosition();
            }   
        }

        lastFrameallowGameSpaceExtension = allowGameSpaceExtension;
    }
}
